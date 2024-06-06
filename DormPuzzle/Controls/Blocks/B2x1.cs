using DormPuzzle.Helpers;

namespace DormPuzzle.Controls.Blocks;

public class B2x1 : Block
{
    public B2x1() : base(10, 2, 1, BrushHelper.FromHex("#CB6573"))
    {
        _locations = [new(0, 0), new(1, 0)];

        AssignLocations();
    }
}
