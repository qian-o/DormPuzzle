using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Media;
using DormPuzzle.Helpers;
using DormPuzzle.Models;

namespace DormPuzzle.Controls.Blocks;

public class BlockContainer : FrameworkElement
{
    public static readonly DependencyProperty ColumnsProperty;
    public static readonly DependencyProperty RowsProperty;
    public static readonly DependencyProperty BorderThicknessProperty;
    public static readonly DependencyProperty BorderBrushProperty;

    static BlockContainer()
    {
        ColumnsProperty = DependencyProperty.Register(nameof(Columns),
                                                      typeof(int),
                                                      typeof(BlockContainer),
                                                      new PropertyMetadata(1, (a, b) => { ((BlockContainer)a).DisabledLocations.Clear(); ((BlockContainer)a).ClearBlocks(); }));

        RowsProperty = DependencyProperty.Register(nameof(Rows),
                                                   typeof(int),
                                                   typeof(BlockContainer),
                                                   new PropertyMetadata(1, (a, b) => { ((BlockContainer)a).DisabledLocations.Clear(); ((BlockContainer)a).ClearBlocks(); }));

        BorderThicknessProperty = DependencyProperty.Register(nameof(BorderThickness),
                                                              typeof(double),
                                                              typeof(BlockContainer),
                                                              new PropertyMetadata(1.0, (a, b) => { ((UIElement)a).InvalidateVisual(); }));

        BorderBrushProperty = DependencyProperty.Register(nameof(BorderBrush),
                                                          typeof(Brush),
                                                          typeof(BlockContainer),
                                                          new PropertyMetadata(Brushes.Black, (a, b) => { ((UIElement)a).InvalidateVisual(); }));
    }

    public BlockContainer()
    {
        this.SetDynamicResource(BorderBrushProperty, "ControlStrongStrokeColorDefaultBrush");

        DisabledLocations.CollectionChanged += (s, e) => InvalidateVisual();
        Blocks.CollectionChanged += (s, e) => InvalidateVisual();
    }

    public int Columns
    {
        get => (int)GetValue(ColumnsProperty);
        set => SetValue(ColumnsProperty, value);
    }

    public int Rows
    {
        get => (int)GetValue(RowsProperty);
        set => SetValue(RowsProperty, value);
    }

    public double BorderThickness
    {
        get => (double)GetValue(BorderThicknessProperty);
        set => SetValue(BorderThicknessProperty, value);
    }

    public Brush BorderBrush
    {
        get => (Brush)GetValue(BorderBrushProperty);
        set => SetValue(BorderBrushProperty, value);
    }

    public ObservableCollection<Location> DisabledLocations { get; } = [];

    private ObservableCollection<(Location, Block)> Blocks { get; } = [];

    public Location PointToLocation(Point point)
    {
        double size = Math.Min(ActualWidth / Columns, ActualHeight / Rows);

        double offsetX = (ActualWidth - size * Columns) / 2;
        double offsetY = (ActualHeight - size * Rows) / 2;

        double cellWidth = size;
        double cellHeight = size;

        int column = (int)((point.X - offsetX) / cellWidth);
        int row = (int)((point.Y - offsetY) / cellHeight);

        return new Location(row, column);
    }

    public bool IsEffectiveLocation(Location location)
    {
        return location.Row >= 0
               && location.Row < Rows
               && location.Column >= 0
               && location.Column < Columns
               && !DisabledLocations.Contains(location)
               && !GetBlocksLocations().Contains(location);
    }

    public bool TryAddBlock(Location location, Block block)
    {
        Location[] locations = block.Locations.Select(item => item + location).ToArray();

        if (locations.All(IsEffectiveLocation))
        {
            if (locations.Intersect(GetBlocksLocations()).Any())
            {
                return false;
            }

            Blocks.Add((location, block));

            return true;
        }

        return false;
    }

    public void ClearBlocks()
    {
        DisabledLocations.Clear();
        Blocks.Clear();
    }

    protected override void OnRender(DrawingContext drawingContext)
    {
        Brush? systemFillColorCriticalBrush = ResourceHelper.GetResource<Brush>("SystemFillColorCriticalBrush");

        double size = Math.Min(ActualWidth / Columns, ActualHeight / Rows);

        double offsetX = (ActualWidth - size * Columns) / 2;
        double offsetY = (ActualHeight - size * Rows) / 2;

        double cellWidth = size;
        double cellHeight = size;

        double actualWidth = cellWidth * Columns;
        double actualHeight = cellHeight * Rows;

        if (actualWidth <= 0.0 || actualHeight <= 0.0)
        {
            return;
        }

        drawingContext.PushTransform(new TranslateTransform(offsetX, offsetY));

        drawingContext.DrawRectangle(Brushes.Transparent, null, new Rect(0, 0, actualWidth, actualHeight));

        foreach ((Location location, Block block) in Blocks)
        {
            int column = block.Locations.Min(location => location.Column) + location.Column;
            int row = block.Locations.Min(location => location.Row) + location.Row;

            drawingContext.PushTransform(new TranslateTransform(column * cellWidth, row * cellHeight));

            block.Render(block.Columns * cellWidth, block.Rows * cellHeight, drawingContext);

            drawingContext.Pop();
        }

        // Draw the border of the container
        {
            foreach (Location location in DisabledLocations)
            {
                drawingContext.DrawLine(new Pen(systemFillColorCriticalBrush, BorderThickness),
                                        new Point(location.Column * cellWidth, location.Row * cellHeight),
                                        new Point((location.Column + 1) * cellWidth, (location.Row + 1) * cellHeight));

                drawingContext.DrawLine(new Pen(systemFillColorCriticalBrush, BorderThickness),
                                        new Point((location.Column + 1) * cellWidth, location.Row * cellHeight),
                                        new Point(location.Column * cellWidth, (location.Row + 1) * cellHeight));
            }

            for (int i = 0; i <= Columns; i++)
            {
                drawingContext.DrawLine(new Pen(BorderBrush, BorderThickness),
                                        new Point(i * actualWidth / Columns, 0),
                                        new Point(i * actualWidth / Columns, actualHeight));
            }

            for (int i = 0; i <= Rows; i++)
            {
                drawingContext.DrawLine(new Pen(BorderBrush, BorderThickness),
                                        new Point(0, i * actualHeight / Rows),
                                        new Point(actualWidth, i * actualHeight / Rows));
            }
        }

        drawingContext.Pop();
    }

    private Location[] GetBlocksLocations()
    {
        return Blocks.SelectMany((item) =>
        {
            (Location location, Block block) = item;

            return block.Locations.Select(item => item + location);

        }).ToArray();
    }
}
