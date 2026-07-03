using GoodbyeUI.Core.Abstractions;
using GoodbyeUI.Core.Models;
using CommunityToolkit.WinUI.Notifications;

namespace GoodbyeUI.Infrastructure.Notifications;

/// <summary>
/// Windows toast bildirimleri gösteren servis (CommunityToolkit üzerinden).
/// Bildirim gösterilemezse sessizce loglanır; kullanıcıya asla teknik hata yansıtılmaz.
/// </summary>
public sealed class ToastNotificationService : INotificationService
{
    private readonly IAppLogger _logger;

    public ToastNotificationService(IAppLogger logger) => _logger = logger;

    public void Show(string title, string message, NotificationType type = NotificationType.Info)
    {
        try
        {
            new ToastContentBuilder()
                .AddText(title)
                .AddText(message)
                .Show();
        }
        catch (Exception ex)
        {
            // Toast altyapısı (ör. paketlenmemiş ortamda) kullanılamıyorsa uygulamayı bozmadan devam et.
            _logger.Warning($"Bildirim gösterilemedi: {ex.Message}");
        }
    }
}
