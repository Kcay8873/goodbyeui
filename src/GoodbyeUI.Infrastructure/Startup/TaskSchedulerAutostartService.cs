using GoodbyeUI.Core.Abstractions;
using Microsoft.Win32.TaskScheduler;

namespace GoodbyeUI.Infrastructure.Startup;

/// <summary>
/// Windows açılışında otomatik başlatmayı Görev Zamanlayıcı ile yönetir.
/// Uygulama Administrator olarak çalıştığından, normal "Startup" klasörü UAC nedeniyle
/// sessizce başlatamaz; bunun yerine "en yüksek ayrıcalık" ile bir logon görevi kullanılır.
/// </summary>
public sealed class TaskSchedulerAutostartService : IAutostartService
{
    private const string TaskName = "GoodbyeUI Autostart";

    private readonly IAppLogger _logger;

    public TaskSchedulerAutostartService(IAppLogger logger) => _logger = logger;

    public bool IsEnabled()
    {
        try
        {
            using var ts = new TaskService();
            return ts.GetTask(TaskName) is not null;
        }
        catch (Exception ex)
        {
            _logger.Warning($"Otomatik başlatma durumu okunamadı: {ex.Message}");
            return false;
        }
    }

    public void Enable()
    {
        try
        {
            var executablePath = Environment.ProcessPath
                ?? throw new InvalidOperationException("Çalıştırılabilir yol belirlenemedi.");

            using var ts = new TaskService();
            var definition = ts.NewTask();
            definition.RegistrationInfo.Description = "Starts GoodbyeUI at Windows logon.";
            definition.Principal.RunLevel = TaskRunLevel.Highest; // en yüksek ayrıcalık (UAC istemeden admin)
            definition.Settings.DisallowStartIfOnBatteries = false;
            definition.Settings.StopIfGoingOnBatteries = false;
            definition.Settings.ExecutionTimeLimit = TimeSpan.Zero; // süresiz

            definition.Triggers.Add(new LogonTrigger());
            definition.Actions.Add(new ExecAction(executablePath, null, null));

            ts.RootFolder.RegisterTaskDefinition(TaskName, definition);
            _logger.Info("Otomatik başlatma görevi oluşturuldu.");
        }
        catch (Exception ex)
        {
            _logger.Error("Otomatik başlatma etkinleştirilemedi.", ex);
        }
    }

    public void Disable()
    {
        try
        {
            using var ts = new TaskService();
            if (ts.GetTask(TaskName) is not null)
            {
                ts.RootFolder.DeleteTask(TaskName, exceptionOnNotExists: false);
                _logger.Info("Otomatik başlatma görevi kaldırıldı.");
            }
        }
        catch (Exception ex)
        {
            _logger.Error("Otomatik başlatma devre dışı bırakılamadı.", ex);
        }
    }
}
