using System.Diagnostics;
using GoodbyeUI.Core.Abstractions;
using GoodbyeUI.Core.Models;

namespace GoodbyeUI.Infrastructure.GoodbyeDpi;

/// <summary>
/// Tek bir harici process'i (GoodbyeDPI) hiç pencere göstermeden yöneten sınıf.
/// CMD penceresi asla görünmez (<c>CreateNoWindow</c> + çıktı yönlendirme).
/// İş parçacığı güvenliği için basit bir kilit kullanılır; kaynaklar <see cref="Dispose"/> ile temizlenir.
/// </summary>
public sealed class ProcessManager : IProcessManager
{
    private readonly IAppLogger _logger;
    private readonly object _sync = new();

    private Process? _process;
    private bool _stopRequested;
    private bool _disposed;

    public ProcessManager(IAppLogger logger) => _logger = logger;

    public event EventHandler<ProcessExitedEventArgs>? Exited;

    public bool IsRunning
    {
        get
        {
            lock (_sync)
            {
                return _process is { HasExited: false };
            }
        }
    }

    public Task StartAsync(string executablePath, string arguments, CancellationToken cancellationToken = default)
    {
        ObjectDisposedException.ThrowIf(_disposed, this);

        if (!File.Exists(executablePath))
            throw new FileNotFoundException("GoodbyeDPI çalıştırılabilir dosyası bulunamadı.", executablePath);

        lock (_sync)
        {
            if (_process is { HasExited: false })
            {
                _logger.Warning("ProcessManager.StartAsync çağrıldı ancak process zaten çalışıyor.");
                return Task.CompletedTask;
            }

            var startInfo = new ProcessStartInfo
            {
                FileName = executablePath,
                Arguments = arguments,
                WorkingDirectory = Path.GetDirectoryName(executablePath) ?? Environment.CurrentDirectory,
                UseShellExecute = false,   // pencere göstermemek ve çıktı yönlendirmek için gerekli
                CreateNoWindow = true,
                WindowStyle = ProcessWindowStyle.Hidden,
                RedirectStandardOutput = true,
                RedirectStandardError = true
            };

            _stopRequested = false;
            _process = new Process { StartInfo = startInfo, EnableRaisingEvents = true };
            _process.OutputDataReceived += OnOutputData;
            _process.ErrorDataReceived += OnErrorData;
            _process.Exited += OnProcessExited;

            _process.Start();
            _process.BeginOutputReadLine();
            _process.BeginErrorReadLine();

            _logger.Info($"GoodbyeDPI başlatıldı (PID {_process.Id}): {Path.GetFileName(executablePath)} {arguments}");
        }

        return Task.CompletedTask;
    }

    public async Task StopAsync(CancellationToken cancellationToken = default)
    {
        Process? proc;
        lock (_sync)
        {
            proc = _process;
            if (proc is null || proc.HasExited)
                return;
            _stopRequested = true;
        }

        try
        {
            _logger.Info("GoodbyeDPI durduruluyor...");
            proc.Kill(entireProcessTree: true);
            await proc.WaitForExitAsync(cancellationToken).ConfigureAwait(false);
        }
        catch (Exception ex)
        {
            _logger.Error("GoodbyeDPI durdurulurken hata oluştu.", ex);
        }
    }

    private void OnProcessExited(object? sender, EventArgs e)
    {
        int exitCode;
        bool expected;

        lock (_sync)
        {
            exitCode = SafeExitCode(_process);
            expected = _stopRequested;
        }

        if (expected)
            _logger.Info($"GoodbyeDPI beklenen şekilde sonlandı (çıkış kodu {exitCode}).");
        else
            _logger.Warning($"GoodbyeDPI beklenmeden sonlandı (çıkış kodu {exitCode}).");

        Exited?.Invoke(this, new ProcessExitedEventArgs(exitCode, expected));
    }

    private void OnOutputData(object? sender, DataReceivedEventArgs e)
    {
        if (!string.IsNullOrWhiteSpace(e.Data))
            _logger.Debug($"[goodbyedpi] {e.Data}");
    }

    private void OnErrorData(object? sender, DataReceivedEventArgs e)
    {
        if (!string.IsNullOrWhiteSpace(e.Data))
            _logger.Warning($"[goodbyedpi:err] {e.Data}");
    }

    private static int SafeExitCode(Process? process)
    {
        try
        {
            return process?.HasExited == true ? process.ExitCode : -1;
        }
        catch
        {
            return -1;
        }
    }

    public void Dispose()
    {
        if (_disposed)
            return;
        _disposed = true;

        lock (_sync)
        {
            if (_process is null)
                return;

            try
            {
                _process.OutputDataReceived -= OnOutputData;
                _process.ErrorDataReceived -= OnErrorData;
                _process.Exited -= OnProcessExited;

                if (!_process.HasExited)
                {
                    _stopRequested = true;
                    _process.Kill(entireProcessTree: true);
                    _process.WaitForExit(3000);
                }
            }
            catch (Exception ex)
            {
                _logger.Error("ProcessManager.Dispose sırasında hata.", ex);
            }
            finally
            {
                _process.Dispose();
                _process = null;
            }
        }
    }
}
