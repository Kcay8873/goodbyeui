using System.Windows.Controls;
using GoodbyeUI.App.ViewModels;

namespace GoodbyeUI.App.Views;

/// <summary>Günlük ekranı — canlı log akışını gösterir.</summary>
public partial class LogsPage : Page
{
    public LogsPage(LogsViewModel viewModel)
    {
        DataContext = viewModel;
        InitializeComponent();
    }
}
