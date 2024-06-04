using System.Windows;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using DormPuzzle.Controls.Blocks;
using DormPuzzle.Models;

namespace DormPuzzle;

public partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();
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