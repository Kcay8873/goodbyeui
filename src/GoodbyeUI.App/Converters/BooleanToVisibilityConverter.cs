using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace GoodbyeUI.App.Converters;

/// <summary>
/// Bool → Visibility dönüştürücü. <see cref="Invert"/> true iken mantık terslenir
/// (true → Collapsed). Tek sınıfla hem "göster" hem "gizle" senaryolarını karşılar.
/// </summary>
public sealed class BooleanToVisibilityConverter : IValueConverter
{
    public bool Invert { get; set; }

    public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        var flag = value is true;
        if (Invert)
            flag = !flag;
        return flag ? Visibility.Visible : Visibility.Collapsed;
    }

    public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
        => value is Visibility.Visible;
}
