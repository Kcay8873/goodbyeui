using GoodbyeUI.Core.Abstractions;
using GoodbyeUI.Infrastructure.GoodbyeDpi;
using GoodbyeUI.Infrastructure.Localization;
using GoodbyeUI.Infrastructure.Logging;
using GoodbyeUI.Infrastructure.Notifications;
using GoodbyeUI.Infrastructure.Settings;
using GoodbyeUI.Infrastructure.Startup;
using Microsoft.Extensions.DependencyInjection;

namespace GoodbyeUI.Infrastructure.DependencyInjection;

/// <summary>Altyapı servislerini DI kabına kaydeden uzantı. Kompozisyon kökü App katmanındadır.</summary>
public static class InfrastructureServiceCollectionExtensions
{
    public static IServiceCollection AddGoodbyeUiInfrastructure(this IServiceCollection services)
    {
        // Loglama: tek InMemoryLogSink örneği hem Serilog'a hem de UI'ya (ILogFeed) verilir.
        services.AddSingleton<InMemoryLogSink>();
        services.AddSingleton<ILogFeed>(sp => sp.GetRequiredService<InMemoryLogSink>());

        services.AddSingleton<SerilogAppLogger>();
        services.AddSingleton<IAppLogger>(sp => sp.GetRequiredService<SerilogAppLogger>());
        services.AddSingleton<ILogLevelSwitch>(sp => sp.GetRequiredService<SerilogAppLogger>());

        // Ayarlar & yerelleştirme
        services.AddSingleton<ISettingsService, JsonSettingsService>();
        services.AddSingleton<ILocalizationService, JsonLocalizationService>();

        // GoodbyeDPI çekirdeği
        services.AddSingleton<IPresetProvider, PresetProvider>();
        services.AddSingleton<IProcessManager, ProcessManager>();
        services.AddSingleton<IGoodbyeDpiBinaryProvider, EmbeddedGoodbyeDpiBinaryProvider>();
        services.AddSingleton<IGoodbyeDpiService, GoodbyeDpiService>();

        // Bildirim & otomatik başlatma
        services.AddSingleton<INotificationService, ToastNotificationService>();
        services.AddSingleton<IAutostartService, TaskSchedulerAutostartService>();

        return services;
    }
}
