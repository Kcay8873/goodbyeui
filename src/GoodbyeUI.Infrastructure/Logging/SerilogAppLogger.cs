using GoodbyeUI.Core.Abstractions;
using GoodbyeUI.Core.Models;
using GoodbyeUI.Infrastructure.Common;
using Serilog;
using Serilog.Core;
using Serilog.Events;

namespace GoodbyeUI.Infrastructure.Logging;

/// <summary>
/// <see cref="IAppLogger"/> için Serilog tabanlı somut implementasyon.
/// Dosya (günlük döngü), Debug çıktısı ve bellek-içi (UI) sink'lerine yazar.
/// Log seviyesi çalışma zamanında <see cref="SetLevel"/> ile değiştirilebilir.
/// </summary>
public sealed class SerilogAppLogger : IAppLogger, ILogLevelSwitch, IDisposable
{
    private readonly Logger _logger;
    private readonly LoggingLevelSwitch _levelSwitch;

    public SerilogAppLogger(InMemoryLogSink memorySink)
    {
        AppPaths.EnsureFolder(AppPaths.LogsFolder);

        _levelSwitch = new LoggingLevelSwitch(LogEventLevel.Information);

        _logger = new LoggerConfiguration()
            .MinimumLevel.ControlledBy(_levelSwitch)
            .Enrich.FromLogContext()
            .WriteTo.File(
                path: Path.Combine(AppPaths.LogsFolder, "goodbyeui-.log"),
                rollingInterval: RollingInterval.Day,
                retainedFileCountLimit: 7,
                outputTemplate: "{Timestamp:yyyy-MM-dd HH:mm:ss.fff} [{Level:u3}] {Message:lj}{NewLine}{Exception}")
            .WriteTo.Debug()
            .WriteTo.Sink(memorySink)
            .CreateLogger();
    }

    public void Debug(string message) => _logger.Debug(message);

    public void Info(string message) => _logger.Information(message);

    public void Warning(string message) => _logger.Warning(message);

    public void Error(string message, Exception? exception = null) => _logger.Error(exception, message);

    public void SetLevel(AppLogLevel level) => _levelSwitch.MinimumLevel = Map(level);

    private static LogEventLevel Map(AppLogLevel level) => level switch
    {
        AppLogLevel.Debug => LogEventLevel.Debug,
        AppLogLevel.Info => LogEventLevel.Information,
        AppLogLevel.Warning => LogEventLevel.Warning,
        _ => LogEventLevel.Error
    };

    public void Dispose() => _logger.Dispose();
}
