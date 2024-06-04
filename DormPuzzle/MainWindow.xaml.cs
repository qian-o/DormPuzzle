using System.Windows;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using DormPuzzle.Controls.Blocks;
using DormPuzzle.Models;
using Wpf.Ui.Appearance;
using Wpf.Ui.Controls;

namespace DormPuzzle;

public partial class MainWindow : FluentWindow
{
    public MainWindow()
    {
        InitializeComponent();

        SystemThemeWatcher.Watch(this);
    }

    private void BlockThumb_DragCompleted(object sender, DragCompletedEventArgs e)
    {
        if (((BlockThumb)sender).Block is Block block)
        {
            Location location = BlockContainer.PointToLocation(Mouse.GetPosition(BlockContainer));
            location -= block.StartLocation;

            BlockContainer.TryAddBlock(location, block);
        }
    }

    private void Toggle_Click(object sender, RoutedEventArgs e)
    {
        BlockContainer.ClearBlocks();
    }

    private void BlockContainer_MouseUp(object sender, MouseButtonEventArgs e)
    {
        if (Toggle.IsChecked == true && e.ChangedButton == MouseButton.Left)
        {
            Location location = BlockContainer.PointToLocation(e.GetPosition(BlockContainer));

            if (!BlockContainer.DisabledLocations.Remove(location))
            {
                BlockContainer.DisabledLocations.Add(location);
            }
        }
    }
}