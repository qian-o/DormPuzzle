using DormPuzzle.Helpers;

namespace DormPuzzle.Controls.Blocks;

public class Block03 : Block
{
    public Block03() : base(3, 2, 3, BrushHelper.FromHex("#5294CE"))
    {
        _locations = [new(0, 0), new(0, 1), new(1, 1), new(1, 2)];

        AssignLocations();
    }
}
