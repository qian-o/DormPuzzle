using DormPuzzle.Helpers;

namespace DormPuzzle.Controls.Blocks;

public class B2x3 : Block
{
    public B2x3() : base(7, 2, 3, BrushHelper.FromHex("#CEA229"))
    {
        _locations = [new(0, 0), new(0, 1), new(0, 2), new(1, 1)];

        AssignLocations();
    }
}
