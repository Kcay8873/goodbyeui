namespace GoodbyeUI.Core.Abstractions;

/// <summary>
/// Gömülü GoodbyeDPI ikili dosyalarını (doğru mimari için) yerel bir çalışma dizinine
/// çıkarır ve çalıştırılabilir dosyanın tam yolunu döndürür. Kullanıcı ayrıca indirme yapmaz.
/// </summary>
public interface IGoodbyeDpiBinaryProvider
{
    /// <summary>
    /// İkili dosyaların hazır olduğundan emin olur (gerekiyorsa çıkarır) ve
    /// <c>goodbyedpi.exe</c> tam yolunu döndürür.
    /// </summary>
    Task<string> EnsureExecutableAsync(CancellationToken cancellationToken = default);
}
