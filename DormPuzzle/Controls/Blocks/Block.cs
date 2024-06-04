using System.Collections.ObjectModel;
using DormPuzzle.Models;

namespace DormPuzzle.Controls.Blocks;

public abstract class Block : PolygonContainer
{
    protected Location[] _locations;
    protected Location _startLocation;

    protected Block(int rows, int columns)
    {
        Rows = rows;
        Columns = columns;

        _locations = [];
    }

    public ReadOnlyCollection<Location> Locations => new(_locations);

    public Location StartLocation => _startLocation;

    public override PolygonContainer Clone()
    {
        Block block = (Block)Activator.CreateInstance(GetType())!;

        return block;
    }
}
