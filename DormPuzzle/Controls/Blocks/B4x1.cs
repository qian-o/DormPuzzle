using DormPuzzle.Helpers;
using DormPuzzle.Models;
using DormPuzzle.Polygons;

namespace DormPuzzle.Controls.Blocks;

public class B4x1 : Block
{
    public B4x1() : base(4, 1)
    {
        _locations = [new(0, 0), new(1, 0), new(2, 0), new(3, 0)];
        _startLocation = _locations[0];

        foreach (Location location in _locations)
        {
            Children.Add(new Quad(location.Row, location.Column) { Fill = BrushHelper.FromHex("#5056C7") });
        }
    }
}
