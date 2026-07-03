using GoodbyeUI.Core.Models;

namespace GoodbyeUI.Core.Abstractions;

/// <summary>
/// GoodbyeDPI yaşam döngüsünü profil bazında yöneten üst düzey servis.
/// UI yalnızca bu servisle konuşur; process/dosya ayrıntılarını bilmez.
/// </summary>
public interface IGoodbyeDpiService : IAsyncDisposable
{
    /// <summary>Anlık bağlantı durumu.</summary>
    ConnectionState State { get; }

    /// <summary>Durum her değiştiğinde tetiklenir.</summary>
    event EventHandler<ConnectionStateChangedEventArgs>? StateChanged;

    /// <summary>Verilen profil ile GoodbyeDPI'yi başlatır.</summary>
    Task ConnectAsync(Preset preset, CancellationToken cancellationToken = default);

    /// <summary>GoodbyeDPI'yi durdurur.</summary>
    Task DisconnectAsync(CancellationToken cancellationToken = default);
}
