using GoodbyeUI.Core.Models;

namespace GoodbyeUI.Core.Abstractions;

/// <summary>Uygulama temasını uygulayan servis (anlık geçiş, yeniden başlatma gerektirmez).</summary>
public interface IThemeService
{
    /// <summary>Etkin tema tercihi.</summary>
    AppTheme CurrentTheme { get; }

    /// <summary>Temayı uygular.</summary>
    void Apply(AppTheme theme);
}
