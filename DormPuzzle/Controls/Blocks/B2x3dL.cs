using DormPuzzle.Helpers;

namespace DormPuzzle.Controls.Blocks;

public class B2x3dL : Block
{
    public B2x3dL() : base(3, 2, 3, BrushHelper.FromHex("#5294CE"))
    {
        _locations = [new(0, 0), new(0, 1), new(1, 1), new(1, 2)];

        AssignLocations();
    }
}
