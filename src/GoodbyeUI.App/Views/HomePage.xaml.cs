using System.Windows.Controls;
using GoodbyeUI.App.ViewModels;

namespace GoodbyeUI.App.Views;

/// <summary>Ana ekran (tek tıkla bağlan). DataContext DI'dan enjekte edilen ViewModel'dir.</summary>
public partial class HomePage : Page
{
    public HomePage(HomeViewModel viewModel)
    {
        DataContext = viewModel;
        InitializeComponent();
    }
}
