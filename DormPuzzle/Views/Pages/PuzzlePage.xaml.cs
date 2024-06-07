using DormPuzzle.Contracts.Views;
using DormPuzzle.ViewModels.Pages;

namespace DormPuzzle.Views.Pages;

/// <summary>
/// PuzzlePage.xaml 的交互逻辑
/// </summary>
public partial class PuzzlePage : UPage
{
    public PuzzlePage()
    {
        InitializeComponent();

        DataContext = new PuzzleViewModel(this);
    }
}
