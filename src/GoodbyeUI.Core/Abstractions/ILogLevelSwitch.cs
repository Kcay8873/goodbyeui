using GoodbyeUI.Core.Models;

namespace GoodbyeUI.Core.Abstractions;

/// <summary>Çalışma zamanında log seviyesini değiştirmeyi sağlar (ayarlardan uygulanır).</summary>
public interface ILogLevelSwitch
{
    void SetLevel(AppLogLevel level);
}
