using CommunityToolkit.Mvvm.ComponentModel;
using GoodbyeUI.Core.Abstractions;

namespace GoodbyeUI.App.ViewModels;

/// <summary>
/// Bir ComboBox seçeneğini temsil eden, yerelleştirilmiş etiketli genel sarmalayıcı.
/// Enum değerlerini (tema, log seviyesi) dil değişiminde güncellenen metinlerle sunar.
/// </summary>
public sealed partial class SelectableOption<T> : ObservableObject
{
    private readonly ILocalizationService _loc;
    private readonly string _labelKey;

    public SelectableOption(T value, string labelKey, ILocalizationService loc)
    {
        Value = value;
        _labelKey = labelKey;
        _loc = loc;
    }

    public T Value { get; }

    public string Label => _loc.GetString(_labelKey);

    public void Refresh() => OnPropertyChanged(nameof(Label));
}
