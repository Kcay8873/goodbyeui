using System.Reflection;
using System.Runtime.InteropServices;
using System.Security.Cryptography;
using GoodbyeUI.Core.Abstractions;
using GoodbyeUI.Infrastructure.Common;

namespace GoodbyeUI.Infrastructure.GoodbyeDpi;

/// <summary>
/// GoodbyeDPI ikili dosyalarını bu derlemeye gömülü kaynaklardan (EmbeddedResource)
/// doğru mimariye göre <see cref="AppPaths.BinFolder"/> altına çıkarır.
///
/// İkili dosyalar, gömülü sürümün içerik damgasını (goodbyedpi.exe hash'i) içeren bir alt
/// klasöre çıkarılır: <c>bin\{arch}\{stamp}</c>. Böylece yeni bir sürüm, önceki sürümün
/// yüklü WinDivert sürücüsü tarafından KİLİTLENMİŞ olabilecek eski <c>.sys</c> dosyasının
/// üzerine yazmaya çalışmaz; her zaman temiz bir klasöre yazar. Aynı sürüm ise mevcut
/// klasörü yeniden kullanır (boyut eşleşince yeniden yazma yapılmaz).
/// </summary>
public sealed class EmbeddedGoodbyeDpiBinaryProvider : IGoodbyeDpiBinaryProvider
{
    private const string ExecutableName = "goodbyedpi.exe";

    private readonly IAppLogger _logger;
    private readonly Assembly _assembly = typeof(EmbeddedGoodbyeDpiBinaryProvider).Assembly;
    private readonly SemaphoreSlim _gate = new(1, 1);

    private string? _cachedExecutablePath;

    public EmbeddedGoodbyeDpiBinaryProvider(IAppLogger logger) => _logger = logger;

    public async Task<string> EnsureExecutableAsync(CancellationToken cancellationToken = default)
    {
        if (_cachedExecutablePath is not null && File.Exists(_cachedExecutablePath))
            return _cachedExecutablePath;

        await _gate.WaitAsync(cancellationToken).ConfigureAwait(false);
        try
        {
            var archFolder = GetArchitectureFolder();
            var resourcePrefix = $"Assets.goodbyedpi.{archFolder}.";

            var resources = _assembly.GetManifestResourceNames()
                .Where(name => name.Contains(resourcePrefix, StringComparison.OrdinalIgnoreCase))
                .ToArray();

            if (resources.Length == 0)
            {
                throw new InvalidOperationException(
                    $"Gömülü GoodbyeDPI ikili dosyaları bulunamadı (mimari: {archFolder}). " +
                    "Derleme sırasında ikili dosyaların gömülmediği anlaşılıyor.");
            }

            var exeResource = resources.First(r => r.EndsWith("." + ExecutableName, StringComparison.OrdinalIgnoreCase));
            var stamp = ComputeStamp(exeResource);

            // İçeriğe özgü klasör: yeni sürüm kilitli eski dosyalarla çakışmaz.
            var targetDir = AppPaths.EnsureFolder(Path.Combine(AppPaths.BinFolder, archFolder, stamp));

            foreach (var resource in resources)
            {
                var index = resource.IndexOf(resourcePrefix, StringComparison.OrdinalIgnoreCase) + resourcePrefix.Length;
                var fileName = resource[index..];
                var destination = Path.Combine(targetDir, fileName);

                await ExtractIfNeededAsync(resource, destination, cancellationToken).ConfigureAwait(false);
            }

            var exePath = Path.Combine(targetDir, ExecutableName);
            if (!File.Exists(exePath))
                throw new FileNotFoundException("Çıkarma sonrası goodbyedpi.exe bulunamadı.", exePath);

            _cachedExecutablePath = exePath;
            _logger.Info($"GoodbyeDPI ikilileri hazır: {targetDir}");
            return exePath;
        }
        finally
        {
            _gate.Release();
        }
    }

    private async Task ExtractIfNeededAsync(string resourceName, string destination, CancellationToken cancellationToken)
    {
        await using var resourceStream = _assembly.GetManifestResourceStream(resourceName)
            ?? throw new InvalidOperationException($"Gömülü kaynak akışı açılamadı: {resourceName}");

        // Boyut aynıysa yeniden yazma (gereksiz disk I/O'dan ve olası dosya kilidinden kaçın).
        if (File.Exists(destination) && new FileInfo(destination).Length == resourceStream.Length)
            return;

        try
        {
            await using var fileStream = new FileStream(destination, FileMode.Create, FileAccess.Write, FileShare.None);
            await resourceStream.CopyToAsync(fileStream, cancellationToken).ConfigureAwait(false);
            _logger.Debug($"Çıkarıldı: {Path.GetFileName(destination)}");
        }
        catch (IOException) when (File.Exists(destination))
        {
            // Dosya başka bir process (ör. yüklü WinDivert sürücüsü) tarafından kilitliyse ve
            // zaten mevcutsa, mevcut sürümle devam et (bağlantıyı bu yüzden bozma).
            _logger.Warning($"Dosya kilitli, mevcut kopya kullanılıyor: {Path.GetFileName(destination)}");
        }
    }

    /// <summary>Gömülü goodbyedpi.exe'nin içerik damgası (kısa SHA-256).</summary>
    private string ComputeStamp(string exeResourceName)
    {
        using var stream = _assembly.GetManifestResourceStream(exeResourceName)
            ?? throw new InvalidOperationException($"Gömülü kaynak akışı açılamadı: {exeResourceName}");
        var hash = SHA256.HashData(ReadAll(stream));
        return Convert.ToHexString(hash, 0, 4).ToLowerInvariant(); // 8 karakter
    }

    private static byte[] ReadAll(Stream stream)
    {
        using var ms = new MemoryStream();
        stream.CopyTo(ms);
        return ms.ToArray();
    }

    private static string GetArchitectureFolder() => RuntimeInformation.ProcessArchitecture switch
    {
        Architecture.X64 => "x86_64",
        Architecture.Arm64 => "x86_64", // GoodbyeDPI ARM64 sağlamıyor; x64 emülasyonla çalışır
        _ => "x86"
    };
}
