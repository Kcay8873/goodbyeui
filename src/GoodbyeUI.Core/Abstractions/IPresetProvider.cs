using GoodbyeUI.Core.Models;

namespace GoodbyeUI.Core.Abstractions;

/// <summary>
/// Profil → argüman eşlemesinin TEK kaynağı. Ham GoodbyeDPI parametreleri
/// kod tabanının hiçbir yerine gömülmez; yalnızca bu servisin implementasyonunda tanımlıdır.
/// </summary>
public interface IPresetProvider
{
    /// <summary>Kullanıcıya sunulacak tüm hazır profiller.</summary>
    IReadOnlyList<Preset> GetAll();

    /// <summary>Belirli bir profil türünü döndürür.</summary>
    Preset Get(PresetKind kind);

    /// <summary>Kullanıcının girdiği ham argümanlardan özel bir profil oluşturur.</summary>
    Preset BuildCustom(string arguments);
}
