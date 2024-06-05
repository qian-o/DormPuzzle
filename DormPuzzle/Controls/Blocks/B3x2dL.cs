using DormPuzzle.Helpers;

namespace DormPuzzle.Controls.Blocks;

public class B3x2dL : Block
{
    public B3x2dL() : base(5, 3, 2, BrushHelper.FromHex("#A8DD2C"))
    {
        _locations = [new(2, 0), new(2, 1), new(1, 1), new(0, 1)];

        AssignLocations();
    }
}
