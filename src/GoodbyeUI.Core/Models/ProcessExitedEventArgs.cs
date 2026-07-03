namespace GoodbyeUI.Core.Models;

/// <summary>Yönetilen arka plan process'i sonlandığında yayınlanan olay verisi.</summary>
public sealed class ProcessExitedEventArgs : EventArgs
{
    public ProcessExitedEventArgs(int exitCode, bool wasExpected)
    {
        ExitCode = exitCode;
        WasExpected = wasExpected;
    }

    /// <summary>Process çıkış kodu.</summary>
    public int ExitCode { get; }

    /// <summary>
    /// Sonlanma uygulama tarafından istendiyse <c>true</c>; process kendiliğinden/çökerek
    /// sonlandıysa <c>false</c> (bu durum kullanıcıya hata olarak bildirilir).
    /// </summary>
    public bool WasExpected { get; }
}
