using GoodbyeUI.Core.Abstractions;
using GoodbyeUI.Core.Models;

namespace GoodbyeUI.Infrastructure.GoodbyeDpi;

/// <summary>
/// GoodbyeDPI yaşam döngüsünü profil bazında yöneten üst düzey servis.
/// İkili çıkarma (<see cref="IGoodbyeDpiBinaryProvider"/>) ve process yönetimini
/// (<see cref="IProcessManager"/>) birleştirir; UI'ya sade bir durum makinesi sunar.
///
/// Dayanıklılık: kullanıcı bağlı kalmak isterken GoodbyeDPI beklenmedik şekilde kapanırsa
/// sınırlı sayıda otomatik yeniden başlatılır (arka planda kendi kendini onarma). Kısa sürede
/// ısrarla çökerse (ör. başka bir WinDivert örneğiyle çakışma) hata durumuna geçer.
/// </summary>
public sealed class GoodbyeDpiService : IGoodbyeDpiService
{
    private const int MaxRestarts = 3;
    private static readonly TimeSpan RestartDelay = TimeSpan.FromSeconds(2);
    private static readonly TimeSpan StableRunThreshold = TimeSpan.FromSeconds(30);

    private readonly IProcessManager _processManager;
    private readonly IGoodbyeDpiBinaryProvider _binaryProvider;
    private readonly IAppLogger _logger;
    private readonly SemaphoreSlim _gate = new(1, 1);

    private ConnectionState _state = ConnectionState.Disconnected;
    private Preset? _activePreset;
    private volatile bool _keepAlive;   // kullanıcı bağlı kalmak istiyor mu?
    private int _restartCount;
    private DateTime _lastStartUtc;

    public GoodbyeDpiService(
        IProcessManager processManager,
        IGoodbyeDpiBinaryProvider binaryProvider,
        IAppLogger logger)
    {
        _processManager = processManager;
        _binaryProvider = binaryProvider;
        _logger = logger;
        _processManager.Exited += OnProcessExited;
    }

    public event EventHandler<ConnectionStateChangedEventArgs>? StateChanged;

    public ConnectionState State => _state;

    public async Task ConnectAsync(Preset preset, CancellationToken cancellationToken = default)
    {
        await _gate.WaitAsync(cancellationToken).ConfigureAwait(false);
        try
        {
            if (_state is ConnectionState.Connected or ConnectionState.Connecting)
                return;

            SetState(ConnectionState.Connecting);

            // Önceki oturumdan kalan (GUI zorla kapatılınca oluşan) orphan goodbyedpi'leri
            // temizle; aksi halde yeni örnek WinDivert'te çakışıp "çıkış kodu 1" ile çöker.
            WinDivertCleaner.KillOrphanProcesses(_logger);

            _activePreset = preset;
            _restartCount = 0;
            _keepAlive = true;

            var started = await StartProcessAsync(preset, cancellationToken).ConfigureAwait(false);
            if (!started)
            {
                _keepAlive = false;
                SetState(ConnectionState.Error, "GoodbyeDPI başlatılamadı veya beklenmedik şekilde kapandı.");
                return;
            }

            SetState(ConnectionState.Connected);
        }
        catch (OperationCanceledException)
        {
            _keepAlive = false;
            SetState(ConnectionState.Disconnected);
            throw;
        }
        catch (Exception ex)
        {
            _keepAlive = false;
            _logger.Error("Bağlantı sırasında hata.", ex);
            SetState(ConnectionState.Error, "Bağlantı kurulamadı. Ayrıntılar için günlükleri inceleyin.");
        }
        finally
        {
            _gate.Release();
        }
    }

    public async Task DisconnectAsync(CancellationToken cancellationToken = default)
    {
        _keepAlive = false; // otomatik yeniden başlatmayı iptal et

        await _gate.WaitAsync(cancellationToken).ConfigureAwait(false);
        try
        {
            if (_state is ConnectionState.Disconnected or ConnectionState.Disconnecting)
                return;

            SetState(ConnectionState.Disconnecting);
            await _processManager.StopAsync(cancellationToken).ConfigureAwait(false);
            await WinDivertCleaner.StopDriverAsync(_logger, cancellationToken).ConfigureAwait(false);
            SetState(ConnectionState.Disconnected);
        }
        catch (Exception ex)
        {
            _logger.Error("Bağlantı kesilirken hata.", ex);
            SetState(ConnectionState.Error, "Bağlantı düzgün kesilemedi.");
        }
        finally
        {
            _gate.Release();
        }
    }

    /// <summary>Process'i başlatır ve hızlı çökme kontrolü yapar. Çağıran <see cref="_gate"/>'i tutmalıdır.</summary>
    private async Task<bool> StartProcessAsync(Preset preset, CancellationToken cancellationToken)
    {
        var exePath = await _binaryProvider.EnsureExecutableAsync(cancellationToken).ConfigureAwait(false);
        _lastStartUtc = DateTime.UtcNow;
        await _processManager.StartAsync(exePath, preset.Arguments, cancellationToken).ConfigureAwait(false);

        // Başlatmadan hemen sonra hızlı çökme kontrolü.
        await Task.Delay(400, cancellationToken).ConfigureAwait(false);
        return _processManager.IsRunning;
    }

    private void OnProcessExited(object? sender, ProcessExitedEventArgs e)
    {
        // Beklenen (bizim durdurduğumuz) çıkışlar ya da kullanıcı bağlı kalmak istemiyorsa: yok say.
        if (e.WasExpected || !_keepAlive)
            return;

        // Stabil çalıştıktan sonra çöktüyse yeniden deneme sayacını sıfırla (yalnızca hızlı
        // ardışık çökmeler birikip pes etmeye yol açsın).
        if (DateTime.UtcNow - _lastStartUtc > StableRunThreshold)
            _restartCount = 0;

        _ = SuperviseRestartAsync();
    }

    /// <summary>Beklenmeyen çökme sonrası sınırlı, gecikmeli otomatik yeniden başlatma.</summary>
    private async Task SuperviseRestartAsync()
    {
        if (Interlocked.Increment(ref _restartCount) > MaxRestarts)
        {
            _keepAlive = false;
            await WinDivertCleaner.StopDriverAsync(_logger).ConfigureAwait(false);
            SetState(ConnectionState.Error,
                "GoodbyeDPI sürekli kapanıyor. Başka bir GoodbyeDPI/WinDivert örneği çalışıyor olabilir.");
            return;
        }

        _logger.Warning($"GoodbyeDPI beklenmeden kapandı; yeniden başlatılıyor ({_restartCount}/{MaxRestarts})...");
        SetState(ConnectionState.Connecting, "Bağlantı yeniden kuruluyor…");

        try
        {
            await Task.Delay(RestartDelay).ConfigureAwait(false);
            if (!_keepAlive)
                return;

            await _gate.WaitAsync().ConfigureAwait(false);
            try
            {
                if (!_keepAlive || _activePreset is null)
                    return;

                var started = await StartProcessAsync(_activePreset, CancellationToken.None).ConfigureAwait(false);
                if (started)
                    SetState(ConnectionState.Connected);
                // Başlamadıysa yeni bir Exited olayı tetiklenir ve bir sonraki deneme yapılır.
            }
            finally
            {
                _gate.Release();
            }
        }
        catch (Exception ex)
        {
            _keepAlive = false;
            _logger.Error("Yeniden başlatma sırasında hata.", ex);
            SetState(ConnectionState.Error, "Bağlantı yeniden kurulamadı.");
        }
    }

    private void SetState(ConnectionState newState, string? message = null)
    {
        var old = _state;
        if (old == newState && message is null)
            return;

        _state = newState;
        _logger.Debug($"Durum: {old} -> {newState}{(message is null ? string.Empty : $" ({message})")}");
        StateChanged?.Invoke(this, new ConnectionStateChangedEventArgs(old, newState, message));
    }

    public async ValueTask DisposeAsync()
    {
        _keepAlive = false;
        _processManager.Exited -= OnProcessExited;
        try
        {
            await _processManager.StopAsync().ConfigureAwait(false);
            await WinDivertCleaner.StopDriverAsync(_logger).ConfigureAwait(false);
        }
        catch
        {
            // yutulur; kapanış sırasında hata kullanıcıya gösterilmez, yalnızca temizlik amaçlı.
        }
        _processManager.Dispose();
        _gate.Dispose();
    }
}
