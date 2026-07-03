namespace GoodbyeUI.Core.Models;

/// <summary>Tek bir log kaydı (Logs ekranında gösterim için).</summary>
/// <param name="Timestamp">Oluşma zamanı.</param>
/// <param name="Level">Seviye.</param>
/// <param name="Message">Biçimlendirilmiş mesaj.</param>
public sealed record LogEntry(DateTimeOffset Timestamp, AppLogLevel Level, string Message);
