using DormPuzzle.Helpers;

namespace DormPuzzle.Controls.Blocks;

public class B1x1 : Block
{
    public B1x1() : base(9, 1, 1, BrushHelper.FromHex("#D354AB"))
    {
        _locations = [new(0, 0)];

        AssignLocations();
    }
}
