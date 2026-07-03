namespace GoodbyeUI.Core.Models;

/// <summary>
/// GoodbyeDPI bağlantısının (arka plan process'inin) yaşam döngüsü durumu.
/// UI, bu durumu doğrudan görsel duruma (renk/animasyon/metin) eşler.
/// </summary>
public enum ConnectionState
{
    /// <summary>Bağlı değil — process çalışmıyor.</summary>
    Disconnected = 0,

    /// <summary>Bağlanıyor — process başlatılıyor.</summary>
    Connecting = 1,

    /// <summary>Bağlı — process sağlıklı çalışıyor.</summary>
    Connected = 2,

    /// <summary>Kesiliyor — process düzgün şekilde sonlandırılıyor.</summary>
    Disconnecting = 3,

    /// <summary>Hata — beklenmeyen bir durum oluştu (ör. process çöktü).</summary>
    Error = 4
}
