using GoodbyeUI.Core.Models;

namespace GoodbyeUI.Core.Abstractions;

/// <summary>Kullanıcı ayarlarını yükleyen, saklayan ve değişiklikleri duyuran servis.</summary>
public interface ISettingsService
{
    /// <summary>Bellekteki güncel ayarlar.</summary>
    AppSettings Current { get; }

    /// <summary>Ayarlar diskten yüklenir (yoksa varsayılanlar kullanılır).</summary>
    Task LoadAsync(CancellationToken cancellationToken = default);

    /// <summary>Güncel ayarları diske kaydeder ve <see cref="Changed"/> olayını tetikler.</summary>
    Task SaveAsync(CancellationToken cancellationToken = default);

    /// <summary>Ayarlar kaydedildiğinde tetiklenir.</summary>
    event EventHandler? Changed;
}
