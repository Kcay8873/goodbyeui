using GoodbyeUI.Core.Models;

namespace GoodbyeUI.Core.Abstractions;

/// <summary>Kullanıcıyı Windows bildirimleriyle (toast) bilgilendiren servis.</summary>
public interface INotificationService
{
    /// <summary>Bir bildirim gösterir.</summary>
    /// <param name="title">Başlık.</param>
    /// <param name="message">İçerik.</param>
    /// <param name="type">Önem türü.</param>
    void Show(string title, string message, NotificationType type = NotificationType.Info);
}
