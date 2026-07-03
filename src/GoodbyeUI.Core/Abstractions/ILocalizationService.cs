using System.ComponentModel;
using GoodbyeUI.Core.Models;

namespace GoodbyeUI.Core.Abstractions;

/// <summary>
/// Metin yerelleştirme servisi. Tüm görünen metinler bu servisten okunur (kodda sabit metin olmaz).
/// <see cref="INotifyPropertyChanged"/> uygular; böylece dil değişince UI bağlamaları otomatik güncellenir.
/// </summary>
public interface ILocalizationService : INotifyPropertyChanged
{
    /// <summary>Kullanılabilir diller.</summary>
    IReadOnlyList<LanguageInfo> AvailableLanguages { get; }

    /// <summary>Etkin dil.</summary>
    LanguageInfo CurrentLanguage { get; }

    /// <summary>Etkin dili değiştirir (yeniden başlatma gerektirmez).</summary>
    void SetLanguage(string cultureCode);

    /// <summary>Anahtara karşılık gelen çeviriyi döndürür; yoksa anahtarı aynen döndürür.</summary>
    string GetString(string key);

    /// <summary>Kısa erişim: <c>loc["Key"]</c>.</summary>
    string this[string key] { get; }

    /// <summary>Dil değiştiğinde tetiklenir.</summary>
    event EventHandler? LanguageChanged;
}
