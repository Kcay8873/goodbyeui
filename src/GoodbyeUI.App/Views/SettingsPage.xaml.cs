using System.Windows.Controls;
using GoodbyeUI.App.ViewModels;

namespace GoodbyeUI.App.Views;

/// <summary>Ayarlar ekranı. Her değişiklik anında uygulanır ve kaydedilir.</summary>
public partial class SettingsPage : Page
{
    public SettingsPage(SettingsViewModel viewModel)
    {
        DataContext = viewModel;
        InitializeComponent();
    }
}
