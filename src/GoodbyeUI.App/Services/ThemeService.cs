using GoodbyeUI.Core.Abstractions;
using GoodbyeUI.Core.Models;
using Wpf.Ui.Appearance;
using Wpf.Ui.Controls;

namespace GoodbyeUI.App.Services;

/// <summary>
/// <see cref="IThemeService"/> için WPF-UI tabanlı implementasyon.
/// Temayı anlık uygular (yeniden başlatma yok) ve Mica arka planını kullanır.
/// Bu sınıf App katmanındadır çünkü yalnızca burada WPF-UI'ye bağımlılık vardır.
/// </summary>
public sealed class ThemeService : IThemeService
{
    public AppTheme CurrentTheme { get; private set; } = AppTheme.System;

    public void Apply(AppTheme theme)
    {
        CurrentTheme = theme;

        var resolved = theme switch
        {
            AppTheme.Light => ApplicationTheme.Light,
            AppTheme.Dark => ApplicationTheme.Dark,
            _ => ApplicationThemeManager.GetSystemTheme() == SystemTheme.Dark
                ? ApplicationTheme.Dark
                : ApplicationTheme.Light
        };

        ApplicationThemeManager.Apply(resolved, WindowBackdropType.Mica, updateAccent: true);
    }
}
