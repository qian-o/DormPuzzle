using System.Windows;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using DormPuzzle.Controls.Blocks;
using DormPuzzle.Game.Tetris;
using DormPuzzle.Models;
using Wpf.Ui.Appearance;
using Wpf.Ui.Controls;
using Block = DormPuzzle.Controls.Blocks.Block;

namespace DormPuzzle;

public partial class MainWindow : FluentWindow
{
    // 所有的方块。
    private readonly Block[] _blocks;

    public MainWindow()
    {
        InitializeComponent();

        SystemThemeWatcher.Watch(this);

        _blocks = typeof(Block).Assembly.GetTypes()
                                        .Where(type => type.IsSubclassOf(typeof(Block)))
                                        .Select(Activator.CreateInstance)
                                        .OrderBy(block => ((Block)block!).Order)
                                        .Cast<Block>()
                                        .ToArray();

        Blocks.ItemsSource = _blocks;
    }

    private void BlockThumb_DragCompleted(object sender, DragCompletedEventArgs e)
    {
        if (((BlockThumb)sender).Block is Block block)
        {
            block = (Block)block.Clone();

            Location location = BlockContainer.PointToLocation(Mouse.GetPosition(BlockContainer));
            location -= block.StartLocation;

            BlockContainer.TryAddBlock(location, block);
        }
    }

    private void Clear_Click(object sender, RoutedEventArgs e)
    {
        BlockContainer.Clear();
    }

    private void BlockContainer_MouseUp(object sender, MouseButtonEventArgs e)
    {
        if (e.ChangedButton == MouseButton.Left)
        {
            Location location = BlockContainer.PointToLocation(e.GetPosition(BlockContainer));

            if (BlockContainer.IsEffectiveLocation(location))
            {
                BlockContainer.DisabledLocations.Add(location);
            }
            else
            {
                BlockContainer.DisabledLocations.Remove(location);
            }
        }
    }

    private Block? CreateBlock(BlockCells blk)
    {
        if (_blocks.FirstOrDefault(item => item.Order == blk.Type) is Block block)
        {
            block = (Block)block.Clone();
            block.Rotate(0);
            block.Rotate(blk.Rot * 90);

            return block;
        }

        return null;
    }

    private void Run_Click(object sender, RoutedEventArgs e)
    {
        BlockContainer.Clear();

        SolveOptions solveOptions = new(BlockContainer.Columns, BlockContainer.Rows, _blocks.OrderBy(block => block.Order).Select(block => block.Count).ToArray())
        {
            Walls = [.. BlockContainer.DisabledLocations]
        };

        List<Solution> solutions = SolveOptions.Solve(solveOptions);
        if (solutions.Count > 0)
        {
            var sln = solutions[0];
            foreach (var placement in sln.Placements)
            {
                BlockContainer.TryAddBlock(
                    new Location(placement.Y, placement.X),
                    CreateBlock(placement.Block)!
                );
            }
        }
        
    }
}