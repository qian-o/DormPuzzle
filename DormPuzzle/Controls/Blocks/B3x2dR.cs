using DormPuzzle.Helpers;

namespace DormPuzzle.Controls.Blocks;

public class B3x2dR : Block
{
    public B3x2dR() : base(6, 3, 2, BrushHelper.FromHex("#60D32A"))
    {
        _locations = [new(0, 0), new(1, 0), new(2, 0), new(2, 1)];

        AssignLocations();
    }
}
