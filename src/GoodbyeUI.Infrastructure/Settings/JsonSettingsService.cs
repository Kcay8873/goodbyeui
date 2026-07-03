using System.Text.Json;
using System.Text.Json.Serialization;
using GoodbyeUI.Core.Abstractions;
using GoodbyeUI.Core.Models;
using GoodbyeUI.Infrastructure.Common;

namespace GoodbyeUI.Infrastructure.Settings;

/// <summary>
/// Kullanıcı ayarlarını %AppData%\GoodbyeUI\settings.json içinde tutan servis.
/// Bozuk/eksik dosya durumunda güvenli varsayılanlara döner (kullanıcıya hata gösterilmez).
/// </summary>
public sealed class JsonSettingsService : ISettingsService
{
    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        WriteIndented = true,
        Converters = { new JsonStringEnumConverter() }
    };

    private readonly IAppLogger _logger;
    private readonly SemaphoreSlim _gate = new(1, 1);

    public JsonSettingsService(IAppLogger logger)
    {
        _logger = logger;
        Current = new AppSettings();
    }

    public AppSettings Current { get; private set; }

    public event EventHandler? Changed;

    public async Task LoadAsync(CancellationToken cancellationToken = default)
    {
        await _gate.WaitAsync(cancellationToken).ConfigureAwait(false);
        try
        {
            if (!File.Exists(AppPaths.SettingsFile))
            {
                _logger.Info("Ayar dosyası yok; varsayılanlar kullanılıyor.");
                return;
            }

            await using var stream = File.OpenRead(AppPaths.SettingsFile);
            var loaded = await JsonSerializer.DeserializeAsync<AppSettings>(stream, JsonOptions, cancellationToken)
                .ConfigureAwait(false);

            if (loaded is not null)
                Current = loaded;
        }
        catch (Exception ex)
        {
            _logger.Error("Ayarlar okunamadı; varsayılanlara dönülüyor.", ex);
            Current = new AppSettings();
        }
        finally
        {
            _gate.Release();
        }
    }

    public async Task SaveAsync(CancellationToken cancellationToken = default)
    {
        await _gate.WaitAsync(cancellationToken).ConfigureAwait(false);
        try
        {
            AppPaths.EnsureFolder(AppPaths.DataFolder);

            // Atomik yazma: önce geçici dosyaya yaz, sonra taşı.
            var tempFile = AppPaths.SettingsFile + ".tmp";
            await using (var stream = File.Create(tempFile))
            {
                await JsonSerializer.SerializeAsync(stream, Current, JsonOptions, cancellationToken)
                    .ConfigureAwait(false);
            }

            File.Move(tempFile, AppPaths.SettingsFile, overwrite: true);
            _logger.Debug("Ayarlar kaydedildi.");
        }
        catch (Exception ex)
        {
            _logger.Error("Ayarlar kaydedilemedi.", ex);
        }
        finally
        {
            _gate.Release();
        }

        Changed?.Invoke(this, EventArgs.Empty);
    }
}
