using System.Windows;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using GoodbyeUI.Core.Abstractions;
using GoodbyeUI.Core.Models;

namespace GoodbyeUI.App.ViewModels;

/// <summary>
/// Kabuk (shell) ViewModel'i: pencere başlığı, gezinme metinleri ve sistem tepsisi komutları.
/// Pencere işlemleri (göster/çık) olaylarla View'a bırakılır; VM UI türlerine bağlı kalmaz.
/// </summary>
public sealed partial class MainViewModel : ObservableObject, IDisposable
{
    private readonly IGoodbyeDpiService _dpi;
    private readonly ISettingsService _settings;
    private readonly IPresetProvider _presets;
    private readonly IAppLogger _logger;

    public MainViewModel(
        IGoodbyeDpiService dpi,
        ISettingsService settings,
        IPresetProvider presets,
        ILocalizationService loc,
        IAppLogger logger)
    {
        _dpi = dpi;
        _settings = settings;
        _presets = presets;
        Loc = loc;
        _logger = logger;

        _state = _dpi.State;
        _dpi.StateChanged += OnStateChanged;
        Loc.LanguageChanged += OnLanguageChanged;
    }

    public ILocalizationService Loc { get; }

    /// <summary>Ana pencerenin gösterilmesi/etkinleştirilmesi istendi.</summary>
    public event EventHandler? ShowRequested;

    /// <summary>Uygulamadan tamamen çıkılması istendi (tepsi menüsü → Çıkış).</summary>
    public event EventHandler? ExitRequested;

    [ObservableProperty]
    private ConnectionState _state;

    public bool IsConnected => State == ConnectionState.Connected;

    /// <summary>Tepsi menüsünde/ipucunda gösterilecek yerelleştirilmiş durum metni.</summary>
    public string StatusText => Loc.GetString(State switch
    {
        ConnectionState.Connected => "Status_Connected",
        ConnectionState.Connecting => "Status_Connecting",
        ConnectionState.Disconnecting => "Status_Disconnecting",
        ConnectionState.Error => "Status_Error",
        _ => "Status_Disconnected"
    });

    /// <summary>Tepsi ipucu metni: "GoodbyeUI — Bağlandı" gibi.</summary>
    public string TrayToolTip => $"{Loc["AppTitle"]} — {StatusText}";

    partial void OnStateChanged(ConnectionState value)
    {
        OnPropertyChanged(nameof(IsConnected));
        OnPropertyChanged(nameof(StatusText));
        OnPropertyChanged(nameof(TrayToolTip));
        ConnectCommand.NotifyCanExecuteChanged();
        DisconnectCommand.NotifyCanExecuteChanged();
    }

    [RelayCommand(CanExecute = nameof(CanConnect))]
    private async Task ConnectAsync()
    {
        var kind = _settings.Current.Preset;
        var preset = kind == PresetKind.Custom
            ? _presets.BuildCustom(_settings.Current.CustomArguments)
            : _presets.Get(kind);

        await _dpi.ConnectAsync(preset).ConfigureAwait(false);
    }

    private bool CanConnect() => State is ConnectionState.Disconnected or ConnectionState.Error;

    [RelayCommand(CanExecute = nameof(CanDisconnect))]
    private async Task DisconnectAsync() => await _dpi.DisconnectAsync().ConfigureAwait(false);

    private bool CanDisconnect() => State == ConnectionState.Connected;

    [RelayCommand]
    private void Show() => ShowRequested?.Invoke(this, EventArgs.Empty);

    [RelayCommand]
    private void Exit() => ExitRequested?.Invoke(this, EventArgs.Empty);

    private void OnStateChanged(object? sender, ConnectionStateChangedEventArgs e)
    {
        var dispatcher = Application.Current?.Dispatcher;
        if (dispatcher is null || dispatcher.CheckAccess())
            State = e.NewState;
        else
            dispatcher.Invoke(() => State = e.NewState);
    }

    private void OnLanguageChanged(object? sender, EventArgs e)
    {
        OnPropertyChanged(nameof(StatusText));
        OnPropertyChanged(nameof(TrayToolTip));
    }

    public void Dispose()
    {
        _dpi.StateChanged -= OnStateChanged;
        Loc.LanguageChanged -= OnLanguageChanged;
    }
}
