namespace GoodbyeUI.Core.Abstractions;

/// <summary>
/// Uygulama genelinde kullanılan sade log soyutlaması.
/// Somut implementasyon (Serilog) altyapı katmanındadır; Core loglama kütüphanesine bağımlı değildir.
/// </summary>
public interface IAppLogger
{
    void Debug(string message);
    void Info(string message);
    void Warning(string message);
    void Error(string message, Exception? exception = null);
}
