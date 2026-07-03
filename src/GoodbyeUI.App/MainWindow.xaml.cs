using System.ComponentModel;
using System.Windows;
using GoodbyeUI.App.ViewModels;
using GoodbyeUI.App.Views;
using GoodbyeUI.Core.Abstractions;
using Wpf.Ui.Appearance;
using Wpf.Ui.Controls;

namespace GoodbyeUI.App;

/// <summary>
/// Ana pencere (kabuk). NavigationView sayfaları DI'dan çözer; sistem teması izlenir.
/// Kapatıldığında (ayara göre) uygulamadan çıkmak yerine sistem tepsisine küçülür.
/// </summary>
public partial class MainWindow : FluentWindow
{
    private readonly MainViewModel _viewModel;
    private readonly ISettingsService _settings;
    private readonly IThemeService _theme;

    private bool _forceClose;

    public MainWindow(MainViewModel viewModel, ISettingsService settings, IThemeService theme, IServiceProvider services)
    {
        _viewModel = viewModel;
        _settings = settings;
        _theme = theme;
        DataContext = viewModel;

        InitializeComponent();

        // Tepsi menüsü ayrı bir görsel ağaçta olduğundan DataContext'i açıkça bağlanır
        // (aksi halde menü öğeleri boş kalır ve menü görünmeyebilir).
        if (TrayIcon.ContextMenu is not null)
            TrayIcon.ContextMenu.DataContext = viewModel;

        // NavigationView, sayfaları uygulama DI kabından oluşturur.
        RootNavigation.SetServiceProvider(services);

        _viewModel.ShowRequested += OnShowRequested;
        _viewModel.ExitRequested += OnExitRequested;

        SourceInitialized += OnSourceInitialized;
        Loaded += OnLoaded;
        Closing += OnClosing;
    }

    private void OnSourceInitialized(object? sender, EventArgs e)
    {
        // HWND artık hazır: temayı ve Mica arka planını ilk render'dan ÖNCE uygula.
        // (Kurucuda uygulanırsa arka plan pencere handle'ı olmadığından uygulanmaz ve ilk
        //  kare siyah/temasız gelir; kullanıcı ancak temayı değiştirince düzelirdi.)
        _theme.Apply(_settings.Current.Theme);
        SystemThemeWatcher.Watch(this, WindowBackdropType.Mica, updateAccents: true);
    }

    private void OnLoaded(object sender, RoutedEventArgs e)
        => RootNavigation.Navigate(typeof(HomePage));

    private void OnShowRequested(object? sender, EventArgs e)
    {
        Show();
        WindowState = WindowState.Normal;
        Activate();
        Topmost = true;
        Topmost = false;
    }

    private void OnExitRequested(object? sender, EventArgs e)
    {
        _forceClose = true;
        Application.Current.Shutdown();
    }

    private void OnClosing(object? sender, CancelEventArgs e)
    {
        // Ayar açıksa ve gerçek çıkış istenmediyse: kapatma yerine tepsiye küçül.
        if (!_forceClose && _settings.Current.MinimizeToTrayOnClose)
        {
            e.Cancel = true;
            Hide();
        }
    }
}
