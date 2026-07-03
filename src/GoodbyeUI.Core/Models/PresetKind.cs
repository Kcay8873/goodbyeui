namespace GoodbyeUI.Core.Models;

/// <summary>
/// Kullanıcıya sunulan hazır GoodbyeDPI profilleri.
/// Ham CMD parametreleri kullanıcıdan gizlenir; her profil bir parametre setine karşılık gelir.
/// </summary>
public enum PresetKind
{
    /// <summary>Hafif mod — düşük gecikme, temel atlatma.</summary>
    Fast = 0,

    /// <summary>Dengeli mod — önerilen varsayılan.</summary>
    Balanced = 1,

    /// <summary>Maksimum uyumluluk — en agresif atlatma.</summary>
    MaximumCompatibility = 2,

    /// <summary>Özel — kullanıcının kendi girdiği ham parametreler.</summary>
    Custom = 3
}
