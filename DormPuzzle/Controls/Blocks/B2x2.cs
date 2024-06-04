using DormPuzzle.Helpers;
using DormPuzzle.Models;
using DormPuzzle.Polygons;

namespace DormPuzzle.Controls.Blocks;

public class B2x2 : Block
{
    public B2x2() : base(2, 2)
    {
        _locations = [new(0, 0), new(0, 1), new(1, 0), new(1, 1)];
        _startLocation = _locations[0];

        foreach (Location location in _locations)
        {
            Children.Add(new Quad(location.Row, location.Column) { Fill = BrushHelper.FromHex("#2E96E6") });
        }
    }
}
