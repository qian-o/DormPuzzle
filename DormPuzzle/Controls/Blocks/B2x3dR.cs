using DormPuzzle.Helpers;
using DormPuzzle.Models;
using DormPuzzle.Polygons;

namespace DormPuzzle.Controls.Blocks;

public class B2x3dR : Block
{
    public B2x3dR() : base(2, 3)
    {
        _locations = [new(1, 0), new(1, 1), new(0, 1), new(0, 2)];
        _startLocation = _locations[2];

        foreach (Location location in _locations)
        {
            Children.Add(new Quad(location.Row, location.Column) { Fill = BrushHelper.FromHex("#2BD7C6") });
        }
    }
}
