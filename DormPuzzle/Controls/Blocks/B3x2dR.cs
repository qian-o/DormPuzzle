using DormPuzzle.Helpers;
using DormPuzzle.Models;
using DormPuzzle.Polygons;

namespace DormPuzzle.Controls.Blocks;

public class B3x2dR : Block
{
    public B3x2dR() : base(3, 2)
    {
        _locations = [new(0, 0), new(1, 0), new(2, 0), new(2, 1)];
        _startLocation = _locations[0];

        foreach (Location location in _locations)
        {
            Children.Add(new Quad(location.Row, location.Column) { Fill = BrushHelper.FromHex("#60D32A") });
        }
    }
}
