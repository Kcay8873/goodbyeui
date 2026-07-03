using System.Collections.Concurrent;
using GoodbyeUI.Core.Abstractions;
using GoodbyeUI.Core.Models;
using Serilog.Core;
using Serilog.Events;

namespace GoodbyeUI.Infrastructure.Logging;

/// <summary>
/// Serilog kayıtlarını sınırlı bir halka tamponda tutan sink; aynı zamanda <see cref="ILogFeed"/>
/// olarak canlı akış sağlar. Tek örnek (singleton) hem Serilog yapılandırmasına hem de UI'ya verilir.
/// </summary>
public sealed class InMemoryLogSink : ILogEventSink, ILogFeed
{
    private const int Capacity = 500;

    private readonly ConcurrentQueue<LogEntry> _buffer = new();

    public event EventHandler<LogEntry>? Emitted;

    public IReadOnlyList<LogEntry> Snapshot() => _buffer.ToArray();

    public void Emit(LogEvent logEvent)
    {
        var entry = new LogEntry(
            logEvent.Timestamp,
            MapLevel(logEvent.Level),
            logEvent.RenderMessage());

        _buffer.Enqueue(entry);
        while (_buffer.Count > Capacity && _buffer.TryDequeue(out _))
        {
            // en eski kayıtları düşür
        }

        Emitted?.Invoke(this, entry);
    }

    private static AppLogLevel MapLevel(LogEventLevel level) => level switch
    {
        LogEventLevel.Verbose or LogEventLevel.Debug => AppLogLevel.Debug,
        LogEventLevel.Information => AppLogLevel.Info,
        LogEventLevel.Warning => AppLogLevel.Warning,
        _ => AppLogLevel.Error
    };
}
