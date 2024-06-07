using System.Collections.ObjectModel;
using System.Windows.Media;
using DormPuzzle.Models;
using DormPuzzle.Polygons;

namespace DormPuzzle.Controls.Blocks;

public abstract class Block : PolygonContainer
{
    protected Location[] _locations = [];
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

    public Location StartLocation { get; private set; }

    public Degrees Degrees { get; private set; }

    public int Count { get => count; set => SetProperty(ref count, value); }

    protected override bool RenderInternal(double width, double height, DrawingContext drawingContext)
    {
        if (!base.RenderInternal(width, height, drawingContext))
        {
            return false;
        }

        // TODO: Render Order.

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
