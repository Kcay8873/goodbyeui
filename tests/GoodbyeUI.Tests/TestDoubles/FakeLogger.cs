using GoodbyeUI.Core.Abstractions;

namespace GoodbyeUI.Tests.TestDoubles;

/// <summary>Testlerde kullanılan, hiçbir şey yapmayan basit logger.</summary>
public sealed class FakeLogger : IAppLogger
{
    public void Debug(string message) { }
    public void Info(string message) { }
    public void Warning(string message) { }
    public void Error(string message, Exception? exception = null) { }
}
