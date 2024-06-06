using DormPuzzle.Helpers;

namespace DormPuzzle.Controls.Blocks;

public class Block11 : Block
{
    public Block11() : base(11, 2, 2, BrushHelper.FromHex("#6A7CB0"))
    {
        _locations = [new(0, 0), new(1, 0), new(1, 1)];

        AssignLocations();
    }
}
