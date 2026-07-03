namespace GoodbyeUI.Core.Models;

/// <summary>Bağlantı durumu değiştiğinde yayınlanan olay verisi.</summary>
public sealed class ConnectionStateChangedEventArgs : EventArgs
{
    public ConnectionStateChangedEventArgs(ConnectionState oldState, ConnectionState newState, string? message = null)
    {
        OldState = oldState;
        NewState = newState;
        Message = message;
    }

    /// <summary>Önceki durum.</summary>
    public ConnectionState OldState { get; }

    /// <summary>Yeni durum.</summary>
    public ConnectionState NewState { get; }

    /// <summary>İsteğe bağlı, kullanıcı dostu açıklama (özellikle hata durumunda).</summary>
    public string? Message { get; }
}
