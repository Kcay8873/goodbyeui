using System.Diagnostics;
using System.Reflection;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using GoodbyeUI.Core.Abstractions;

namespace GoodbyeUI.App.ViewModels;

/// <summary>Hakkında ekranının ViewModel'i: sürüm, kaynak/lisans atıfları ve bağlantılar.</summary>
public sealed partial class AboutViewModel : ObservableObject
{
    private const string GitHubUrl = "https://github.com/Kcay8873";
    private const string PatreonUrl = "https://www.patreon.com/c/kcay8873";

    private readonly IAppLogger _logger;

    public AboutViewModel(ILocalizationService loc, IAppLogger logger)
    {
        Loc = loc;
        _logger = logger;
        Version = Assembly.GetExecutingAssembly().GetName().Version?.ToString(3) ?? "1.0.0";
    }

    public ILocalizationService Loc { get; }

    public string Version { get; }

    [RelayCommand]
    private void OpenGitHub() => OpenUrl(GitHubUrl);

    [RelayCommand]
    private void OpenPatreon() => OpenUrl(PatreonUrl);

    private void OpenUrl(string url)
    {
        try
        {
            Process.Start(new ProcessStartInfo { FileName = url, UseShellExecute = true });
        }
        catch (Exception ex)
        {
            _logger.Error($"Bağlantı açılamadı: {url}", ex);
        }
    }
}
