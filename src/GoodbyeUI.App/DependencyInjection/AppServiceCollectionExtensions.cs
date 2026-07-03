using GoodbyeUI.App.Services;
using GoodbyeUI.App.ViewModels;
using GoodbyeUI.App.Views;
using GoodbyeUI.Core.Abstractions;
using Microsoft.Extensions.DependencyInjection;

namespace GoodbyeUI.App.DependencyInjection;

/// <summary>App (sunum) katmanı servislerini, ViewModel'leri ve View'ları DI kabına kaydeder.</summary>
public static class AppServiceCollectionExtensions
{
    public static IServiceCollection AddGoodbyeUiApp(this IServiceCollection services)
    {
        // Sunum katmanına özgü servisler
        services.AddSingleton<IThemeService, ThemeService>();

        // ViewModel'ler (tekil: durum ve olay abonelikleri kararlı kalır, sızıntı olmaz)
        services.AddSingleton<MainViewModel>();
        services.AddSingleton<HomeViewModel>();
        services.AddSingleton<SettingsViewModel>();
        services.AddSingleton<LogsViewModel>();
        services.AddSingleton<AboutViewModel>();

        // View'lar (NavigationView bunları IServiceProvider üzerinden çözer)
        services.AddSingleton<HomePage>();
        services.AddSingleton<SettingsPage>();
        services.AddSingleton<LogsPage>();
        services.AddSingleton<AboutPage>();

        // Ana pencere
        services.AddSingleton<MainWindow>();

        return services;
    }
}
