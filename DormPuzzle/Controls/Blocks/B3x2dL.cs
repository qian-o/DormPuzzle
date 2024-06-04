using DormPuzzle.Helpers;
using DormPuzzle.Models;
using DormPuzzle.Polygons;

namespace DormPuzzle.Controls.Blocks;

public class B3x2dL : Block
{
    public B3x2dL() : base(3, 2)
    {
        _locations = [new(2, 0), new(2, 1), new(1, 1), new(0, 1)];
        _startLocation = _locations[3];

        foreach (Location location in _locations)
        {
            Children.Add(new Quad(location.Row, location.Column) { Fill = BrushHelper.FromHex("#A8DD2C") });
        }
    }
}
