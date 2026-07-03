using GoodbyeUI.Core.Models;

namespace GoodbyeUI.Core.Abstractions;

/// <summary>
/// Tek bir harici konsol process'ini (GoodbyeDPI) hiç pencere göstermeden
/// güvenli şekilde başlatan, izleyen ve sonlandıran soyutlama.
/// </summary>
public interface IProcessManager : IDisposable
{
    /// <summary>Process şu anda çalışıyor mu?</summary>
    bool IsRunning { get; }

    /// <summary>Process sonlandığında (beklenen ya da beklenmeyen) tetiklenir.</summary>
    event EventHandler<ProcessExitedEventArgs>? Exited;

    /// <summary>Process'i gizli (penceresiz) olarak başlatır.</summary>
    /// <param name="executablePath">Çalıştırılabilir dosyanın tam yolu.</param>
    /// <param name="arguments">Komut satırı argümanları.</param>
    Task StartAsync(string executablePath, string arguments, CancellationToken cancellationToken = default);

    /// <summary>Çalışan process'i düzgün şekilde sonlandırır (yoksa sessizce döner).</summary>
    Task StopAsync(CancellationToken cancellationToken = default);
}
