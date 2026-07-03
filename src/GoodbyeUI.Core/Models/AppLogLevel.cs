namespace GoodbyeUI.Core.Models;

/// <summary>
/// Uygulama log seviyesi. (Serilog'a özel bağımlılık bırakmamak için Core'da tanımlı.)
/// </summary>
public enum AppLogLevel
{
    Debug = 0,
    Info = 1,
    Warning = 2,
    Error = 3
}
