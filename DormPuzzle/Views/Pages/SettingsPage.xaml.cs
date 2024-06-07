using DormPuzzle.Contracts.Views;
using DormPuzzle.ViewModels.Pages;

namespace DormPuzzle.Views.Pages;

/// <summary>
/// SettingsPage.xaml 的交互逻辑
/// </summary>
public partial class SettingsPage : UPage
{
    public SettingsPage()
    {
        InitializeComponent();

        DataContext = new SettingsViewModel(this);
    }
}
