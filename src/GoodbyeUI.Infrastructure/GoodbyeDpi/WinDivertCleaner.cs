using System.Diagnostics;
using GoodbyeUI.Core.Abstractions;
using GoodbyeUI.Infrastructure.Common;

namespace GoodbyeUI.Infrastructure.GoodbyeDpi;

/// <summary>
/// GoodbyeDPI'nin yüklediği WinDivert çekirdek sürücüsünü durdurur.
///
/// goodbyedpi.exe normal (nazik) kapanışında WinDivert servisini durdurup siler; ancak biz
/// process'i <c>Kill</c> ile sonlandırdığımız için bu temizlik atlanır ve sürücü bellekte
/// kalır (kaynak sızıntısı + sonraki sürüm yükseltmelerinde .sys dosya kilidi). Bu yardımcı,
/// process durdurulduktan sonra sürücüyü nazikçe durdurarak o temizliği yerine getirir.
/// Uygulama Administrator çalıştığından <c>sc stop</c> yeterlidir. En iyi çaba: hatalar yutulur.
/// </summary>
internal static class WinDivertCleaner
{
    // GoodbyeDPI sürümlerine göre kullanılan servis adları.
    private static readonly string[] ServiceNames = { "WinDivert", "WinDivert1.4" };

    /// <summary>
    /// Uygulamanın kendi bin klasöründen çalışan artık (orphan) goodbyedpi process'lerini kapatır.
    /// Bunlar GUI zorla kapatıldığında geride kalır ve yeni bir örnekle WinDivert üzerinde
    /// çakışarak "çıkış kodu 1" ile çökmesine yol açar. Kullanıcının kendi (başka konumdaki)
    /// goodbyedpi'sine dokunulmaz — yalnızca bizim bin klasörümüzdekiler hedeflenir.
    /// </summary>
    public static void KillOrphanProcesses(IAppLogger logger)
    {
        foreach (var proc in Process.GetProcessesByName("goodbyedpi"))
        {
            try
            {
                var path = proc.MainModule?.FileName;
                if (path is not null && path.StartsWith(AppPaths.BinFolder, StringComparison.OrdinalIgnoreCase))
                {
                    proc.Kill(entireProcessTree: true);
                    proc.WaitForExit(2000);
                    logger.Info($"Artık (orphan) goodbyedpi kapatıldı: PID {proc.Id}");
                }
            }
            catch (Exception ex)
            {
                logger.Debug($"Orphan goodbyedpi kontrolünde hata: {ex.Message}");
            }
            finally
            {
                proc.Dispose();
            }
        }
    }

    public static async Task StopDriverAsync(IAppLogger logger, CancellationToken cancellationToken = default)
    {
        foreach (var service in ServiceNames)
        {
            // Durdur + sil: kaydın silinmiş bir .sys yoluna işaret edip kalmasını (ve sonraki
            // sürümde "driver not found" hatasını) önler. goodbyedpi de nazik kapanışta böyle yapar.
            await RunScAsync(logger, service, "stop", cancellationToken).ConfigureAwait(false);
            await RunScAsync(logger, service, "delete", cancellationToken).ConfigureAwait(false);
        }
    }

    private static async Task RunScAsync(IAppLogger logger, string service, string verb, CancellationToken cancellationToken)
    {
        try
        {
            using var process = new Process
            {
                StartInfo = new ProcessStartInfo
                {
                    FileName = "sc.exe",
                    Arguments = $"{verb} {service}",
                    UseShellExecute = false,
                    CreateNoWindow = true,
                    WindowStyle = ProcessWindowStyle.Hidden,
                    RedirectStandardOutput = true,
                    RedirectStandardError = true
                }
            };

            process.Start();
            await process.WaitForExitAsync(cancellationToken).ConfigureAwait(false);
            logger.Debug($"sc {verb} {service} -> çıkış {process.ExitCode}");
        }
        catch (Exception ex)
        {
            // Servis yoksa/çalışmıyorsa ya da başka bir uygulama kullanıyorsa sessizce geç.
            logger.Debug($"sc {verb} {service} başarısız: {ex.Message}");
        }
    }
}
