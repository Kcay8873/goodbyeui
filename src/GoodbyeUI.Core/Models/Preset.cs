namespace GoodbyeUI.Core.Models;

/// <summary>
/// Bir GoodbyeDPI profili: kullanıcıya gösterilecek yerelleştirme anahtarları ile
/// arka planda çalıştırılacak ham komut satırı argümanlarını birleştirir.
/// </summary>
/// <param name="Kind">Profil türü.</param>
/// <param name="NameKey">Görünen ad için yerelleştirme anahtarı (ör. "Preset_Balanced_Name").</param>
/// <param name="DescriptionKey">Açıklama için yerelleştirme anahtarı.</param>
/// <param name="Arguments">GoodbyeDPI'ye verilecek komut satırı argümanları.</param>
public sealed record Preset(
    PresetKind Kind,
    string NameKey,
    string DescriptionKey,
    string Arguments);
