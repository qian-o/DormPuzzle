using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Markup;

namespace DormPuzzle.Controls.Blocks;

[ContentProperty(nameof(Block))]
public class BlockThumb : Thumb
{
    public static readonly DependencyProperty TitleProperty;
    public static readonly DependencyProperty DescriptionProperty;
    public static readonly DependencyProperty BlockProperty;
    public static readonly DependencyProperty CanvasProperty;

    static BlockThumb()
    {
        DefaultStyleKeyProperty.OverrideMetadata(typeof(BlockThumb), new FrameworkPropertyMetadata(typeof(BlockThumb)));


        TitleProperty = DependencyProperty.Register(nameof(Title),
                                                    typeof(object),
                                                    typeof(BlockThumb),
                                                    new PropertyMetadata(null));

        DescriptionProperty = DependencyProperty.Register(nameof(Description),
                                                          typeof(string),
                                                          typeof(BlockThumb),
                                                          new PropertyMetadata("This is a description"));

        BlockProperty = DependencyProperty.Register(nameof(Block),
                                                    typeof(Block),
                                                    typeof(BlockThumb),
                                                    new PropertyMetadata(null));

        CanvasProperty = DependencyProperty.Register(nameof(Canvas),
                                                     typeof(Canvas),
                                                     typeof(BlockThumb),
                                                     new PropertyMetadata(null));
    }

    public BlockThumb()
    {
        MouseDoubleClick += BlockThumb_MouseDoubleClick;
        DragStarted += BlockThumb_DragStarted;
        DragDelta += BlockThumb_DragDelta;
        DragCompleted += BlockThumb_DragCompleted;
    }

    public object? Title
    {
        get => GetValue(TitleProperty);
        set => SetValue(TitleProperty, value);
    }

    public string Description
    {
        get => (string)GetValue(DescriptionProperty);
        set => SetValue(DescriptionProperty, value);
    }

    public Block? Block
    {
        get => (Block)GetValue(BlockProperty);
        set => SetValue(BlockProperty, value);
    }

    public Canvas? Canvas
    {
        get => (Canvas)GetValue(CanvasProperty);
        set => SetValue(CanvasProperty, value);
    }

    private void BlockThumb_MouseDoubleClick(object sender, MouseButtonEventArgs e)
    {
        Block?.Rotate();
    }

    private void BlockThumb_DragStarted(object sender, DragStartedEventArgs e)
    {
        if (Block != null && Canvas != null)
        {
            AssemblyBlock();
        }
        else
        {
            CancelDrag();
        }
    }

    private void BlockThumb_DragDelta(object sender, DragDeltaEventArgs e)
    {
        UpdateBlock();
    }

    private void BlockThumb_DragCompleted(object sender, DragCompletedEventArgs e)
    {
        UnassemblyBlock();
    }

    private void AssemblyBlock()
    {
        UnassemblyBlock();

        if (Block != null && Canvas != null)
        {
            Block block = (Block)Block.Clone();
            block.Width = ActualWidth;
            block.Height = ActualHeight;

            Canvas.Children.Add(block);

            Tag = block;
        }

        UpdateBlock();
    }

    private void UpdateBlock()
    {
        if (Tag is Block block)
        {
            block.GetCellSize(out double cellWidth, out double cellHeight);
            block.GetOffset(block.StartLocation, out double offsetX, out double offsetY);

            Point point = Mouse.GetPosition(Canvas);

            Canvas.SetLeft(block, point.X - offsetX - (cellWidth / 2.0));
            Canvas.SetTop(block, point.Y - offsetY - (cellHeight / 2.0));
        }
    }

    private void UnassemblyBlock()
    {
        if (Block != null && Canvas != null)
        {
            if (Tag is Block block)
            {
                Canvas.Children.Remove(block);
            }
        }
    }
}
