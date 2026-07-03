using GoodbyeUI.Core.Abstractions;
using GoodbyeUI.Core.Models;

namespace GoodbyeUI.Infrastructure.GoodbyeDpi;

/// <summary>
/// Profil → GoodbyeDPI argümanları eşlemesinin TEK kaynağı. Ham GoodbyeDPI parametreleri
/// kod tabanının başka hiçbir yerine gömülmez.
///
/// Argümanlar, sahada çalıştığı doğrulanmış Türkiye/DNS-redirect komutlarına dayanır
/// (GoodbyeDPI 0.2.3rc3, resmi turkey_dnsredir preset'leri). Türkiye'deki ISS'ler DNS
/// zehirlemesi de yaptığından, DPI atlatmaya ek olarak DNS yönlendirmesi (Yandex DNS,
/// port 1253) tüm preset'lere dahildir — sade "-5" tek başına yeterli olmaz.
///
/// Değişiklik gerektiğinde SADECE burası düzenlenir.
/// </summary>
public sealed class PresetProvider : IPresetProvider
{
    // ISS DNS zehirlemesini aşan yönlendirme (GoodbyeDPI'nin dahili DNS proxy'sine).
    private const string DnsRedirect =
        "--dns-addr 77.88.8.8 --dns-port 1253 --dnsv6-addr 2a02:6b8::feed:0ff --dnsv6-port 1253";

    // Hızlı: düşük TTL (bazı ISS'lerde, ör. Superonline, daha iyi) + DNS yönlendirme.
    private static readonly Preset Fast = new(
        PresetKind.Fast, "Preset_Fast_Name", "Preset_Fast_Desc",
        $"--set-ttl 3 {DnsRedirect}");

    // Dengeli (varsayılan): sahada doğrulanmış ana Türkiye komutu (-5 + set-ttl 5 + DNS).
    private static readonly Preset Balanced = new(
        PresetKind.Balanced, "Preset_Balanced_Name", "Preset_Balanced_Desc",
        $"-5 --set-ttl 5 {DnsRedirect}");

    // Maksimum uyumluluk: daha agresif taban mod (-6) + DNS yönlendirme.
    private static readonly Preset MaximumCompatibility = new(
        PresetKind.MaximumCompatibility, "Preset_Max_Name", "Preset_Max_Desc",
        $"-6 --set-ttl 5 {DnsRedirect}");

    // Custom, ham argümanlar çalışma zamanında verildiği için sabit listede yer almaz
    // ama seçici için bir şablon kaydı bulundurulur.
    private static readonly Preset CustomTemplate = new(
        PresetKind.Custom, "Preset_Custom_Name", "Preset_Custom_Desc", string.Empty);

    private static readonly IReadOnlyList<Preset> All = new[]
    {
        Fast, Balanced, MaximumCompatibility, CustomTemplate
    };

    public IReadOnlyList<Preset> GetAll() => All;

    public Preset Get(PresetKind kind) => kind switch
    {
        PresetKind.Fast => Fast,
        PresetKind.Balanced => Balanced,
        PresetKind.MaximumCompatibility => MaximumCompatibility,
        PresetKind.Custom => CustomTemplate,
        _ => Balanced
    };

    public Preset BuildCustom(string arguments) =>
        CustomTemplate with { Arguments = arguments?.Trim() ?? string.Empty };
}
