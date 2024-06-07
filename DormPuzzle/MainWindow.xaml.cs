using DormPuzzle.Views.Pages;
using Wpf.Ui.Appearance;
using Wpf.Ui.Controls;

namespace DormPuzzle;

public partial class MainWindow : FluentWindow
{
    public MainWindow()
    {
        InitializeComponent();

        SystemThemeWatcher.Watch(this);

        Loaded += (_, _) => RootNavigation.Navigate(typeof(PuzzlePage));
    }
}