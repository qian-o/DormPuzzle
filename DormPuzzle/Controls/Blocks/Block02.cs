using DormPuzzle.Helpers;

namespace DormPuzzle.Controls.Blocks;

public class Block02 : Block
{
    public Block02() : base(2, 1, 4, BrushHelper.FromHex("#5056C7"))
    {
        _locations = [new(0, 0), new(0, 1), new(0, 2), new(0, 3)];

        AssignLocations();
    }
}
