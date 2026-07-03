using System.Collections.Concurrent;
using System.ComponentModel;
using System.Globalization;
using System.Reflection;
using System.Text.Json;
using GoodbyeUI.Core.Abstractions;
using GoodbyeUI.Core.Models;

namespace GoodbyeUI.Infrastructure.Localization;

/// <summary>
/// Gömülü JSON sözlüklerinden (her dil bir dosya) çeviri sağlayan servis.
/// Yeni dil eklemek = yeni bir <c>Localization\Languages\{kod}.json</c> dosyası eklemek
/// ve <see cref="NativeNames"/> haritasına bir satır yazmaktan ibarettir (kod değişikliği yok).
/// Dil değişince <see cref="INotifyPropertyChanged"/> ile UI bağlamaları otomatik güncellenir.
/// </summary>
public sealed class JsonLocalizationService : ILocalizationService
{
    private const string DefaultCulture = "en";
    private const string ResourceMarker = ".Localization.Languages.";

    // Dilin kendi dilindeki adı. Bilinmeyen kodlar için kodun kendisi kullanılır.
    private static readonly IReadOnlyDictionary<string, string> NativeNames =
        new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
        {
            ["en"] = "English",
            ["tr"] = "Türkçe"
        };

    private readonly ConcurrentDictionary<string, IReadOnlyDictionary<string, string>> _catalogs = new();
    private readonly List<LanguageInfo> _available = new();

    private IReadOnlyDictionary<string, string> _current = new Dictionary<string, string>();
    private IReadOnlyDictionary<string, string> _fallback = new Dictionary<string, string>();

    public JsonLocalizationService()
    {
        LoadAllCatalogs();
        SetLanguage(DefaultCulture);
    }

    public event PropertyChangedEventHandler? PropertyChanged;
    public event EventHandler? LanguageChanged;

    public IReadOnlyList<LanguageInfo> AvailableLanguages => _available;

    public LanguageInfo CurrentLanguage { get; private set; } = new(DefaultCulture, "English");

    public string this[string key] => GetString(key);

    public string GetString(string key)
    {
        if (string.IsNullOrEmpty(key))
            return string.Empty;

        if (_current.TryGetValue(key, out var value))
            return value;
        if (_fallback.TryGetValue(key, out var fallbackValue))
            return fallbackValue;

        return key; // eksik çeviri: anahtarı göster (geliştirme sırasında fark edilir)
    }

    public void SetLanguage(string cultureCode)
    {
        if (string.IsNullOrWhiteSpace(cultureCode))
            cultureCode = DefaultCulture;

        if (!_catalogs.TryGetValue(cultureCode, out var catalog))
        {
            cultureCode = DefaultCulture;
            _catalogs.TryGetValue(cultureCode, out catalog);
        }

        _current = catalog ?? new Dictionary<string, string>();
        CurrentLanguage = _available.FirstOrDefault(l => l.Code == cultureCode)
            ?? new LanguageInfo(cultureCode, ResolveNativeName(cultureCode));

        try
        {
            var culture = CultureInfo.GetCultureInfo(cultureCode);
            CultureInfo.CurrentUICulture = culture;
            CultureInfo.CurrentCulture = culture;
            CultureInfo.DefaultThreadCurrentUICulture = culture;
            CultureInfo.DefaultThreadCurrentCulture = culture;
        }
        catch (CultureNotFoundException)
        {
            // yalnızca çeviri sözlüğü kullanılır; sistem kültürü değiştirilemezse yoksay.
        }

        // "Item[]" bildirimi, XAML'deki indeksleyici bağlamalarını ({Binding [Key]}) tazeler.
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("Item[]"));
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(CurrentLanguage)));
        LanguageChanged?.Invoke(this, EventArgs.Empty);
    }

    private void LoadAllCatalogs()
    {
        var assembly = typeof(JsonLocalizationService).Assembly;

        foreach (var resourceName in assembly.GetManifestResourceNames())
        {
            var markerIndex = resourceName.IndexOf(ResourceMarker, StringComparison.OrdinalIgnoreCase);
            if (markerIndex < 0 || !resourceName.EndsWith(".json", StringComparison.OrdinalIgnoreCase))
                continue;

            var codeStart = markerIndex + ResourceMarker.Length;
            var code = resourceName[codeStart..^".json".Length];

            var dictionary = ReadCatalog(assembly, resourceName);
            if (dictionary is null)
                continue;

            _catalogs[code] = dictionary;
            _available.Add(new LanguageInfo(code, ResolveNativeName(code)));

            if (code.Equals(DefaultCulture, StringComparison.OrdinalIgnoreCase))
                _fallback = dictionary;
        }

        _available.Sort((a, b) => string.Compare(a.NativeName, b.NativeName, StringComparison.CurrentCulture));
    }

    private static IReadOnlyDictionary<string, string>? ReadCatalog(Assembly assembly, string resourceName)
    {
        using var stream = assembly.GetManifestResourceStream(resourceName);
        if (stream is null)
            return null;

        try
        {
            var data = JsonSerializer.Deserialize<Dictionary<string, string>>(stream);
            return data ?? new Dictionary<string, string>();
        }
        catch (JsonException)
        {
            return new Dictionary<string, string>();
        }
    }

    private static string ResolveNativeName(string code) =>
        NativeNames.TryGetValue(code, out var name) ? name : code;
}
