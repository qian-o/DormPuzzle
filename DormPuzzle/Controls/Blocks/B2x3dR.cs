using DormPuzzle.Helpers;

namespace DormPuzzle.Controls.Blocks;

public class B2x3dR : Block
{
    public B2x3dR() : base(4, 2, 3, BrushHelper.FromHex("#2BD7C6"))
    {
        _locations = [new(1, 0), new(1, 1), new(0, 1), new(0, 2)];

        AssignLocations();
    }
}
