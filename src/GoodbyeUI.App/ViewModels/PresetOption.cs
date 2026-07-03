using CommunityToolkit.Mvvm.ComponentModel;
using GoodbyeUI.Core.Abstractions;
using GoodbyeUI.Core.Models;

namespace GoodbyeUI.App.ViewModels;

/// <summary>
/// Bir profilin UI'da gösterilebilir hali. Ad/açıklama yerelleştirmeden çözülür ve
/// dil değişince <see cref="Refresh"/> ile güncellenir.
/// </summary>
public sealed partial class PresetOption : ObservableObject
{
    private readonly ILocalizationService _loc;
    private readonly Preset _preset;

    public PresetOption(Preset preset, ILocalizationService loc)
    {
        _preset = preset;
        _loc = loc;
    }

    public PresetKind Kind => _preset.Kind;

    public string Name => _loc.GetString(_preset.NameKey);

    public string Description => _loc.GetString(_preset.DescriptionKey);

    /// <summary>Dil değiştiğinde bağlı metinleri tazeler.</summary>
    public void Refresh()
    {
        OnPropertyChanged(nameof(Name));
        OnPropertyChanged(nameof(Description));
    }
}
