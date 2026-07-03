namespace GoodbyeUI.Infrastructure.Common;

/// <summary>
/// Uygulamanın kullandığı dosya sistemi konumlarının tek kaynağı.
/// Ayarlar/loglar %AppData%; çıkarılan ikili dosyalar %LocalAppData% altında tutulur.
/// </summary>
public static class AppPaths
{
    /// <summary>Kök veri klasörü: %AppData%\GoodbyeUI</summary>
    public static string DataFolder { get; } = Path.Combine(
        Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "GoodbyeUI");

    /// <summary>Yerel veri klasörü: %LocalAppData%\GoodbyeUI</summary>
    public static string LocalDataFolder { get; } = Path.Combine(
        Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "GoodbyeUI");

    /// <summary>Ayar dosyası yolu.</summary>
    public static string SettingsFile => Path.Combine(DataFolder, "settings.json");

    /// <summary>Log klasörü.</summary>
    public static string LogsFolder => Path.Combine(DataFolder, "logs");

    /// <summary>Çıkarılan GoodbyeDPI ikili dosyalarının bulunacağı klasör.</summary>
    public static string BinFolder => Path.Combine(LocalDataFolder, "bin");

    /// <summary>Verilen klasörün var olduğundan emin olur.</summary>
    public static string EnsureFolder(string path)
    {
        Directory.CreateDirectory(path);
        return path;
    }
}
