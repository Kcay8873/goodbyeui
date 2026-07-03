using System.Collections.ObjectModel;
using System.Windows;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using GoodbyeUI.Core.Abstractions;
using GoodbyeUI.Core.Models;

namespace GoodbyeUI.App.ViewModels;

/// <summary>
/// Ana ekranın ViewModel'i: büyük Connect butonu, durum göstergesi ve profil seçimi.
/// Tüm uzun işlemler async'tir; UI thread'i asla bloklanmaz.
/// </summary>
public sealed partial class HomeViewModel : ObservableObject, IDisposable
{
    private readonly IGoodbyeDpiService _dpi;
    private readonly ISettingsService _settings;
    private readonly IPresetProvider _presets;
    private readonly INotificationService _notifications;
    private readonly IAppLogger _logger;

    public HomeViewModel(
        IGoodbyeDpiService dpi,
        ISettingsService settings,
        IPresetProvider presets,
        ILocalizationService loc,
        INotificationService notifications,
        IAppLogger logger)
    {
        _dpi = dpi;
        _settings = settings;
        _presets = presets;
        Loc = loc;
        _notifications = notifications;
        _logger = logger;

        Presets = new ObservableCollection<PresetOption>(
            _presets.GetAll().Select(p => new PresetOption(p, loc)));

        _selectedPreset = Presets.FirstOrDefault(p => p.Kind == _settings.Current.Preset)
            ?? Presets.First();

        _state = _dpi.State;
        _dpi.StateChanged += OnStateChanged;
        Loc.LanguageChanged += OnLanguageChanged;
    }

    /// <summary>XAML bağlamaları için yerelleştirme kaynağı (ör. <c>{Binding Loc[Connect_Button]}</c>).</summary>
    public ILocalizationService Loc { get; }

    public ObservableCollection<PresetOption> Presets { get; }

    [ObservableProperty]
    private ConnectionState _state;

    [ObservableProperty]
    private PresetOption _selectedPreset;

    /// <summary>Bağlanıyor/kesiliyor arası — buton devre dışı, animasyon aktif.</summary>
    public bool IsBusy => State is ConnectionState.Connecting or ConnectionState.Disconnecting;

    public bool IsConnected => State == ConnectionState.Connected;

    /// <summary>Durum metni için yerelleştirme anahtarı (View bunu Loc üzerinden çözer).</summary>
    public string StatusKey => State switch
    {
        ConnectionState.Connected => "Status_Connected",
        ConnectionState.Connecting => "Status_Connecting",
        ConnectionState.Disconnecting => "Status_Disconnecting",
        ConnectionState.Error => "Status_Error",
        _ => "Status_Disconnected"
    };

    /// <summary>Bağlan/Kes butonunun metni için anahtar.</summary>
    public string ActionKey => IsConnected ? "Disconnect_Button" : "Connect_Button";

    /// <summary>Çözülmüş (yerelleştirilmiş) durum metni.</summary>
    public string StatusText => Loc.GetString(StatusKey);

    /// <summary>Çözülmüş (yerelleştirilmiş) buton metni.</summary>
    public string ActionText => Loc.GetString(ActionKey);

    partial void OnStateChanged(ConnectionState value)
    {
        OnPropertyChanged(nameof(IsBusy));
        OnPropertyChanged(nameof(IsConnected));
        OnPropertyChanged(nameof(StatusKey));
        OnPropertyChanged(nameof(ActionKey));
        OnPropertyChanged(nameof(StatusText));
        OnPropertyChanged(nameof(ActionText));
        ToggleConnectionCommand.NotifyCanExecuteChanged();
    }

    partial void OnSelectedPresetChanged(PresetOption value)
    {
        if (value is null || _settings.Current.Preset == value.Kind)
            return;

        _settings.Current.Preset = value.Kind;
        _ = _settings.SaveAsync();
    }

    [RelayCommand(CanExecute = nameof(CanToggle))]
    private async Task ToggleConnectionAsync()
    {
        if (IsConnected)
            await _dpi.DisconnectAsync().ConfigureAwait(false);
        else
            await _dpi.ConnectAsync(ResolveSelectedPreset()).ConfigureAwait(false);
    }

    private bool CanToggle() => !IsBusy;

    private Preset ResolveSelectedPreset()
    {
        var kind = SelectedPreset?.Kind ?? PresetKind.Balanced;
        return kind == PresetKind.Custom
            ? _presets.BuildCustom(_settings.Current.CustomArguments)
            : _presets.Get(kind);
    }

    private void OnStateChanged(object? sender, ConnectionStateChangedEventArgs e)
    {
        // Servis olayları arka plan thread'inden gelebilir; UI güncellemesini dispatcher'a al.
        RunOnUi(() =>
        {
            State = e.NewState;
            NotifyUser(e);
        });
    }

    private void NotifyUser(ConnectionStateChangedEventArgs e)
    {
        switch (e.NewState)
        {
            case ConnectionState.Connected:
                _notifications.Show(Loc["Notif_Connected_Title"], Loc["Notif_Connected_Msg"], NotificationType.Success);
                break;
            case ConnectionState.Disconnected when e.OldState is ConnectionState.Connected or ConnectionState.Disconnecting:
                _notifications.Show(Loc["Notif_Disconnected_Title"], Loc["Notif_Disconnected_Msg"], NotificationType.Info);
                break;
            case ConnectionState.Error:
                _notifications.Show(Loc["Notif_Error_Title"], e.Message ?? Loc["Status_Error"], NotificationType.Error);
                break;
        }
    }

    private void OnLanguageChanged(object? sender, EventArgs e)
    {
        foreach (var preset in Presets)
            preset.Refresh();

        OnPropertyChanged(nameof(StatusKey));
        OnPropertyChanged(nameof(ActionKey));
        OnPropertyChanged(nameof(StatusText));
        OnPropertyChanged(nameof(ActionText));
    }

    private static void RunOnUi(Action action)
    {
        var dispatcher = Application.Current?.Dispatcher;
        if (dispatcher is null || dispatcher.CheckAccess())
            action();
        else
            dispatcher.Invoke(action);
    }

    public void Dispose()
    {
        _dpi.StateChanged -= OnStateChanged;
        Loc.LanguageChanged -= OnLanguageChanged;
    }
}
