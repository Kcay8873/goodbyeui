namespace GoodbyeUI.Core.Abstractions;

/// <summary>
/// Windows açılışında otomatik başlatmayı yöneten servis.
/// Uygulama Administrator olarak çalıştığı için normal "Startup" yerine
/// Görev Zamanlayıcı (en yüksek ayrıcalıkla) kullanılır.
/// </summary>
public interface IAutostartService
{
    /// <summary>Otomatik başlatma etkin mi?</summary>
    bool IsEnabled();

    /// <summary>Otomatik başlatmayı etkinleştirir.</summary>
    void Enable();

    /// <summary>Otomatik başlatmayı devre dışı bırakır.</summary>
    void Disable();
}
