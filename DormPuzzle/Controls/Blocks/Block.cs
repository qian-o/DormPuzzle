using System.Collections.ObjectModel;
using System.Globalization;
using System.Windows;
using System.Windows.Media;
using DormPuzzle.Models;
using DormPuzzle.Polygons;

namespace DormPuzzle.Controls.Blocks;

public abstract class Block : PolygonContainer
{
    protected Location[] _locations = [];
    protected Location[] _orderLocations = [];
    protected Brush _fill;
    private int count = 1;

    protected Block(int order, int rows, int columns, Brush fill)
    {
        Order = order;
        Rows = rows;
        Columns = columns;

        _fill = fill;
    }

    public int Order { get; }

    public ReadOnlyCollection<Location> Locations => new(_locations);

    public ReadOnlyCollection<Location> OrderLocations => new(_orderLocations);

    public Location StartLocation { get; private set; }

    public Degrees Degrees { get; private set; }

    public int Count { get => count; set => SetProperty(ref count, value); }

    protected override bool RenderInternal(double width, double height, DrawingContext drawingContext)
    {
        if (!base.RenderInternal(width, height, drawingContext))
        {
            return false;
        }

        CalcLayout(width,
                   height,
                   out double offsetX,
                   out double offsetY,
                   out double actualWidth,
                   out double actualHeight,
                   out double cellWidth,
                   out double cellHeight);

        drawingContext.PushTransform(new TranslateTransform(offsetX, offsetY));
        {
            int startRow = _orderLocations.Min(l => l.Row);
            int startColumn = _orderLocations.Min(l => l.Column);
            int endRow = _orderLocations.Max(l => l.Row);
            int endColumn = _orderLocations.Max(l => l.Column);

            double x = startColumn * cellWidth;
            double y = startRow * cellHeight;
            double w = (endColumn - startColumn + 1) * cellWidth;
            double h = (endRow - startRow + 1) * cellHeight;

            drawingContext.PushTransform(new TranslateTransform(x, y));
            {
                double marginH = w * 0.1;
                double marginV = h * 0.1;
                w -= marginH * 2.0;
                h -= marginV * 2.0;

                drawingContext.PushTransform(new TranslateTransform(marginH, marginV));
                {
                    float pixelsPerDip = (float)VisualTreeHelper.GetDpi(Application.Current.MainWindow).PixelsPerDip;

                    FormattedText text = new(Order.ToString(CultureInfo.InvariantCulture),
                                             CultureInfo.InvariantCulture,
                                             FlowDirection.LeftToRight,
                                             FontFamily.GetTypefaces().First(),
                                             0.9 * Math.Min(w, h),
                                             Brushes.White,
                                             pixelsPerDip);

                    drawingContext.DrawGeometry(Brushes.White,
                                                null,
                                                text.BuildGeometry(new Point((w - text.Width) / 2, (h - text.Height) / 2)));

                }
                drawingContext.Pop();
            }
            drawingContext.Pop();
        }
        drawingContext.Pop();

        return true;
    }

    protected void AssignLocations()
    {
        if (_locations.Length == 0)
        {
            return;
        }

        foreach (Location location in _locations)
        {
            Children.Add(new Quad(location.Row, location.Column) { Fill = _fill });
        }

        StartLocation = _locations.OrderBy(l => l.Row).ThenBy(l => l.Column).First();
    }

    public override PolygonContainer Clone()
    {
        Block block = (Block)Activator.CreateInstance(GetType())!;
        block.Count = Count;
        block.Rotate((int)Degrees);

        return block;
    }

    public void Rotate()
    {
        Children.Clear();

        _locations = _locations.Select(item => new Location(item.Column, Rows - item.Row - 1)).ToArray();
        _orderLocations = _orderLocations.Select(item => new Location(item.Column, Rows - item.Row - 1)).ToArray();

        (Rows, Columns) = (Columns, Rows);

        AssignLocations();

        Degrees += 90;

        if ((int)Degrees == 360)
        {
            Degrees = 0;
        }
    }

    public void Rotate(int degrees)
    {
        if (degrees % 90 != 0)
        {
            throw new ArgumentException("Degrees must be a multiple of 90.");
        }

        int rotations = degrees / 90;

        for (int i = 0; i < rotations; i++)
        {
            Rotate();
        }
    }
}
