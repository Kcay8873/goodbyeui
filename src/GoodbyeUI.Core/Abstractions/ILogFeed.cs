using GoodbyeUI.Core.Models;

namespace GoodbyeUI.Core.Abstractions;

/// <summary>
/// Canlı log akışı. Logs ekranı bunun anlık görüntüsünü alır ve yeni kayıtları dinler.
/// (Bellek-içi halka tampon; dosya loglaması ayrıca Serilog dosya sink'i ile yapılır.)
/// </summary>
public interface ILogFeed
{
    /// <summary>Tamponaki mevcut kayıtların anlık kopyası (eskiden yeniye).</summary>
    IReadOnlyList<LogEntry> Snapshot();

    /// <summary>Yeni bir kayıt eklendiğinde tetiklenir.</summary>
    event EventHandler<LogEntry>? Emitted;
}
