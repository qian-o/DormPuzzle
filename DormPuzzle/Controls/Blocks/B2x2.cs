using DormPuzzle.Helpers;

namespace DormPuzzle.Controls.Blocks;

public class B2x2 : Block
{
    public B2x2() : base(1, 2, 2, BrushHelper.FromHex("#2E96E6"))
    {
        _locations = [new(0, 0), new(0, 1), new(1, 0), new(1, 1)];

        AssignLocations();
    }
}
