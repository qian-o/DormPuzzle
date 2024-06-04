using DormPuzzle.Helpers;
using DormPuzzle.Models;
using DormPuzzle.Polygons;

namespace DormPuzzle.Controls.Blocks;

public class B3x3 : Block
{
    public B3x3() : base(3, 3)
    {
        _locations = [new(1, 0), new(1, 1), new(1, 2), new(0, 1), new(2, 1)];
        _startLocation = _locations[3];

        foreach (Location location in _locations)
        {
            Children.Add(new Quad(location.Row, location.Column) { Fill = BrushHelper.FromHex("#C0E82E") });
        }
    }
}
