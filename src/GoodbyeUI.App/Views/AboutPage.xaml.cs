using System.Windows.Controls;
using GoodbyeUI.App.ViewModels;

namespace GoodbyeUI.App.Views;

/// <summary>Hakkında ekranı — sürüm ve lisans atıfları.</summary>
public partial class AboutPage : Page
{
    public AboutPage(AboutViewModel viewModel)
    {
        DataContext = viewModel;
        InitializeComponent();
    }
}
