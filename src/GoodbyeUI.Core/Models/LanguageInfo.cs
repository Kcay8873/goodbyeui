namespace GoodbyeUI.Core.Models;

/// <summary>
/// Desteklenen bir dilin kimliği. Yeni dil eklemek, yeni bir kaynak dosyası ve
/// bu listeye bir kayıt eklemekten ibarettir (kod değişikliği gerektirmez).
/// </summary>
/// <param name="Code">Kültür kodu (ör. "en", "tr").</param>
/// <param name="NativeName">Dilin kendi dilindeki adı (ör. "Türkçe", "English").</param>
public sealed record LanguageInfo(string Code, string NativeName);
