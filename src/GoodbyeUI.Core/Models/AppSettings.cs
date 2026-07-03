namespace GoodbyeUI.Core.Models;

/// <summary>
/// Kalıcı kullanıcı ayarları. JSON olarak diske serileştirilir.
/// Varsayılan değerler, ilk açılışta "tek tıkla çalışır" deneyimi sağlar.
/// </summary>
public sealed class AppSettings
{
    /// <summary>Arayüz dili kültür kodu.</summary>
    public string LanguageCode { get; set; } = "en";

    /// <summary>Tema tercihi.</summary>
    public AppTheme Theme { get; set; } = AppTheme.System;

    /// <summary>Seçili GoodbyeDPI profili.</summary>
    public PresetKind Preset { get; set; } = PresetKind.Balanced;

    /// <summary>Özel profil için ham argümanlar (yalnızca <see cref="PresetKind.Custom"/>'da kullanılır).</summary>
    public string CustomArguments { get; set; } = string.Empty;

    /// <summary>Windows açılışında (Görev Zamanlayıcı ile) otomatik başlat.</summary>
    public bool RunAtStartup { get; set; }

    /// <summary>Uygulama açıldığında son profil ile otomatik bağlan.</summary>
    public bool AutoConnectOnStartup { get; set; }

    /// <summary>Pencere kapatıldığında uygulamayı kapatmak yerine sistem tepsisine küçült.</summary>
    public bool MinimizeToTrayOnClose { get; set; } = true;

    /// <summary>Log ayrıntı seviyesi.</summary>
    public AppLogLevel LogLevel { get; set; } = AppLogLevel.Info;

    /// <summary>Sığ bir kopya oluşturur (ayarları değiştirmeden karşılaştırma/geri alma için).</summary>
    public AppSettings Clone() => (AppSettings)MemberwiseClone();
}
