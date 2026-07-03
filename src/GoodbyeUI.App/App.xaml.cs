using System.Threading.Tasks;
using System.Windows;
using System.Windows.Threading;
using GoodbyeUI.App.DependencyInjection;
using GoodbyeUI.Core.Abstractions;
using GoodbyeUI.Infrastructure.DependencyInjection;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace GoodbyeUI.App;

/// <summary>
/// Uygulama giriş noktası ve kompozisyon kökü.
/// Host + DI kurar, ayarları yükler, dil/tema/log seviyesini uygular ve ana pencereyi açar.
/// Tüm beklenmeyen hatalar global olarak yakalanır; kullanıcıya teknik ayrıntı gösterilmez.
/// </summary>
public partial class App : Application
{
    private static Mutex? _singleInstanceMutex;

    private IHost? _host;

    protected override async void OnStartup(StartupEventArgs e)
    {
        base.OnStartup(e);

        // Tek örnek: yönetici modunda birden fazla kopya çakışmayı önler.
        _singleInstanceMutex = new Mutex(initiallyOwned: true, "GoodbyeUI_SingleInstance_Mutex", out var isNew);
        if (!isNew)
        {
            Shutdown();
            return;
        }

        _host = Host.CreateDefaultBuilder()
            .ConfigureServices((_, services) =>
            {
                services.AddGoodbyeUiInfrastructure();
                services.AddGoodbyeUiApp();
            })
            .Build();

        RegisterGlobalExceptionHandlers();

        await InitializeAsync();

        var window = _host.Services.GetRequiredService<MainWindow>();
        MainWindow = window;
        window.Show();
    }

    /// <summary>Ayarları yükler ve dil/tema/log seviyesini uygular; isteğe bağlı otomatik bağlanır.</summary>
    private async Task InitializeAsync()
    {
        var services = _host!.Services;
        var logger = services.GetRequiredService<IAppLogger>();
        logger.Info("GoodbyeUI başlatılıyor.");

        var settings = services.GetRequiredService<ISettingsService>();
        await settings.LoadAsync().ConfigureAwait(true);

        services.GetRequiredService<ILocalizationService>().SetLanguage(settings.Current.LanguageCode);
        services.GetRequiredService<ILogLevelSwitch>().SetLevel(settings.Current.LogLevel);
        // Not: Tema, kaynak sözlüklerinin doğru yenilenmesi için MainWindow oluşturulduktan
        // sonra (pencere kurucusunda) uygulanır — aksi halde ilk karede metin renkleri hatalı olur.

        if (settings.Current.AutoConnectOnStartup)
            _ = AutoConnectAsync(services, logger);
    }

    private static async Task AutoConnectAsync(IServiceProvider services, IAppLogger logger)
    {
        try
        {
            var presets = services.GetRequiredService<IPresetProvider>();
            var settings = services.GetRequiredService<ISettingsService>();
            var dpi = services.GetRequiredService<IGoodbyeDpiService>();

            var kind = settings.Current.Preset;
            var preset = kind == Core.Models.PresetKind.Custom
                ? presets.BuildCustom(settings.Current.CustomArguments)
                : presets.Get(kind);

            await dpi.ConnectAsync(preset).ConfigureAwait(false);
        }
        catch (Exception ex)
        {
            logger.Error("Açılışta otomatik bağlanma başarısız.", ex);
        }
    }

    private void RegisterGlobalExceptionHandlers()
    {
        DispatcherUnhandledException += OnDispatcherUnhandledException;
        AppDomain.CurrentDomain.UnhandledException += OnDomainUnhandledException;
        TaskScheduler.UnobservedTaskException += OnUnobservedTaskException;
    }

    private void OnDispatcherUnhandledException(object sender, DispatcherUnhandledExceptionEventArgs e)
    {
        LogUnhandled("UI thread", e.Exception);
        ShowFriendlyError();
        e.Handled = true; // uygulama çökmesin
    }

    private void OnDomainUnhandledException(object sender, UnhandledExceptionEventArgs e)
        => LogUnhandled("AppDomain", e.ExceptionObject as Exception);

    private void OnUnobservedTaskException(object? sender, UnobservedTaskExceptionEventArgs e)
    {
        LogUnhandled("Task", e.Exception);
        e.SetObserved();
    }

    private void LogUnhandled(string source, Exception? ex)
    {
        try
        {
            _host?.Services.GetRequiredService<IAppLogger>()
                .Error($"Beklenmeyen hata ({source}).", ex);
        }
        catch
        {
            // loglama dahi başarısızsa yapılabilecek bir şey yok.
        }
    }

    private void ShowFriendlyError()
    {
        try
        {
            var loc = _host?.Services.GetService<ILocalizationService>();
            var title = loc?["Notif_Error_Title"] ?? "Something went wrong";
            MessageBox.Show(title, title, MessageBoxButton.OK, MessageBoxImage.Warning);
        }
        catch
        {
            // sessiz geç
        }
    }

    protected override async void OnExit(ExitEventArgs e)
    {
        try
        {
            if (_host is not null)
            {
                // GoodbyeDPI process'ini temiz sonlandır (kaynak sızıntısı bırakma).
                var dpi = _host.Services.GetService<IGoodbyeDpiService>();
                if (dpi is not null)
                    await dpi.DisposeAsync().ConfigureAwait(false);

                _host.Dispose();
            }
        }
        catch
        {
            // kapanışta hata yutulur
        }
        finally
        {
            _singleInstanceMutex?.Dispose();
            base.OnExit(e);
        }
    }
}
