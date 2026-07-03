using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using GoodbyeUI.Core.Abstractions;
using GoodbyeUI.Core.Models;

namespace GoodbyeUI.App.ViewModels;

/// <summary>
/// Ayarlar ekranının ViewModel'i. Her değişiklik anında uygulanır (tema/dil/log)
/// ve kalıcı olarak kaydedilir. İlk yüklemede geri besleme döngüsü engellenir.
/// </summary>
public sealed partial class SettingsViewModel : ObservableObject
{
    private readonly ISettingsService _settings;
    private readonly IThemeService _theme;
    private readonly IAutostartService _autostart;
    private readonly ILogLevelSwitch _logLevel;
    private readonly IAppLogger _logger;

    private bool _isInitializing;

    public SettingsViewModel(
        ISettingsService settings,
        ILocalizationService loc,
        IThemeService theme,
        IAutostartService autostart,
        ILogLevelSwitch logLevel,
        IAppLogger logger)
    {
        _settings = settings;
        Loc = loc;
        _theme = theme;
        _autostart = autostart;
        _logLevel = logLevel;
        _logger = logger;

        Languages = new ObservableCollection<LanguageInfo>(loc.AvailableLanguages);

        Themes = new ObservableCollection<SelectableOption<AppTheme>>
        {
            new(AppTheme.System, "Theme_System", loc),
            new(AppTheme.Light, "Theme_Light", loc),
            new(AppTheme.Dark, "Theme_Dark", loc)
        };

        LogLevels = new ObservableCollection<SelectableOption<AppLogLevel>>
        {
            new(AppLogLevel.Debug, "LogLevel_Debug", loc),
            new(AppLogLevel.Info, "LogLevel_Info", loc),
            new(AppLogLevel.Warning, "LogLevel_Warning", loc),
            new(AppLogLevel.Error, "LogLevel_Error", loc)
        };

        Presets = new ObservableCollection<SelectableOption<PresetKind>>
        {
            new(PresetKind.Fast, "Preset_Fast_Name", loc),
            new(PresetKind.Balanced, "Preset_Balanced_Name", loc),
            new(PresetKind.MaximumCompatibility, "Preset_Max_Name", loc),
            new(PresetKind.Custom, "Preset_Custom_Name", loc)
        };

        LoadFromSettings();
        Loc.LanguageChanged += (_, _) => RefreshOptionLabels();
    }

    public ILocalizationService Loc { get; }

    public ObservableCollection<LanguageInfo> Languages { get; }
    public ObservableCollection<SelectableOption<AppTheme>> Themes { get; }
    public ObservableCollection<SelectableOption<AppLogLevel>> LogLevels { get; }
    public ObservableCollection<SelectableOption<PresetKind>> Presets { get; }

    [ObservableProperty] private LanguageInfo? _selectedLanguage;
    [ObservableProperty] private SelectableOption<AppTheme>? _selectedTheme;
    [ObservableProperty] private SelectableOption<AppLogLevel>? _selectedLogLevel;
    [ObservableProperty] private SelectableOption<PresetKind>? _selectedPreset;
    [ObservableProperty] private string _customArguments = string.Empty;
    [ObservableProperty] private bool _runAtStartup;
    [ObservableProperty] private bool _autoConnectOnStartup;
    [ObservableProperty] private bool _minimizeToTrayOnClose;

    /// <summary>Özel parametre kutusu yalnızca "Custom" profili seçiliyken etkindir.</summary>
    public bool IsCustomPresetSelected => SelectedPreset?.Value == PresetKind.Custom;

    private void LoadFromSettings()
    {
        _isInitializing = true;

        var current = _settings.Current;
        SelectedLanguage = Languages.FirstOrDefault(l => l.Code == current.LanguageCode) ?? Languages.FirstOrDefault();
        SelectedTheme = Themes.FirstOrDefault(t => t.Value == current.Theme);
        SelectedLogLevel = LogLevels.FirstOrDefault(l => l.Value == current.LogLevel);
        SelectedPreset = Presets.FirstOrDefault(p => p.Value == current.Preset);
        CustomArguments = current.CustomArguments;
        RunAtStartup = _autostart.IsEnabled();
        AutoConnectOnStartup = current.AutoConnectOnStartup;
        MinimizeToTrayOnClose = current.MinimizeToTrayOnClose;

        _isInitializing = false;
    }

    partial void OnSelectedLanguageChanged(LanguageInfo? value)
    {
        if (_isInitializing || value is null)
            return;
        Loc.SetLanguage(value.Code);
        _settings.Current.LanguageCode = value.Code;
        Persist();
    }

    partial void OnSelectedThemeChanged(SelectableOption<AppTheme>? value)
    {
        if (_isInitializing || value is null)
            return;
        _theme.Apply(value.Value);
        _settings.Current.Theme = value.Value;
        Persist();
    }

    partial void OnSelectedLogLevelChanged(SelectableOption<AppLogLevel>? value)
    {
        if (_isInitializing || value is null)
            return;
        _logLevel.SetLevel(value.Value);
        _settings.Current.LogLevel = value.Value;
        Persist();
    }

    partial void OnSelectedPresetChanged(SelectableOption<PresetKind>? value)
    {
        OnPropertyChanged(nameof(IsCustomPresetSelected));
        if (_isInitializing || value is null)
            return;
        _settings.Current.Preset = value.Value;
        Persist();
    }

    partial void OnCustomArgumentsChanged(string value)
    {
        if (_isInitializing)
            return;
        _settings.Current.CustomArguments = value ?? string.Empty;
        Persist();
    }

    partial void OnRunAtStartupChanged(bool value)
    {
        if (_isInitializing)
            return;

        if (value)
            _autostart.Enable();
        else
            _autostart.Disable();

        _settings.Current.RunAtStartup = value;
        Persist();
    }

    partial void OnAutoConnectOnStartupChanged(bool value)
    {
        if (_isInitializing)
            return;
        _settings.Current.AutoConnectOnStartup = value;
        Persist();
    }

    partial void OnMinimizeToTrayOnCloseChanged(bool value)
    {
        if (_isInitializing)
            return;
        _settings.Current.MinimizeToTrayOnClose = value;
        Persist();
    }

    private void RefreshOptionLabels()
    {
        foreach (var t in Themes) t.Refresh();
        foreach (var l in LogLevels) l.Refresh();
        foreach (var p in Presets) p.Refresh();
    }

    private void Persist() => _ = _settings.SaveAsync();
}
