using DormPuzzle.Helpers;

namespace DormPuzzle.Controls.Blocks;

public class B4x1 : Block
{
    public B4x1() : base(2, 4, 1, BrushHelper.FromHex("#5056C7"))
    {
        _locations = [new(0, 0), new(1, 0), new(2, 0), new(3, 0)];

        AssignLocations();
    }
}
