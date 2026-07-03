using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;
using GoodbyeUI.Core.Models;

namespace GoodbyeUI.App.Converters;

/// <summary>
/// Bağlantı durumunu durum göstergesi rengine çevirir.
/// Yeşil = bağlı, kehribar = geçiş, kırmızı = hata, gri = bağlı değil.
/// </summary>
public sealed class ConnectionStateToBrushConverter : IValueConverter
{
    private static readonly Brush Connected = new SolidColorBrush(Color.FromRgb(0x2E, 0xCC, 0x71));
    private static readonly Brush Transitional = new SolidColorBrush(Color.FromRgb(0xF1, 0xC4, 0x0F));
    private static readonly Brush Error = new SolidColorBrush(Color.FromRgb(0xE7, 0x4C, 0x3C));
    private static readonly Brush Disconnected = new SolidColorBrush(Color.FromRgb(0x9E, 0x9E, 0x9E));

    static ConnectionStateToBrushConverter()
    {
        Connected.Freeze();
        Transitional.Freeze();
        Error.Freeze();
        Disconnected.Freeze();
    }

    public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture) => value switch
    {
        ConnectionState.Connected => Connected,
        ConnectionState.Connecting or ConnectionState.Disconnecting => Transitional,
        ConnectionState.Error => Error,
        _ => Disconnected
    };

    public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
        => throw new NotSupportedException();
}
