using DormPuzzle.Helpers;

namespace DormPuzzle.Controls.Blocks;

public class Block09 : Block
{
    public Block09() : base(9, 1, 1, BrushHelper.FromHex("#D354AB"))
    {
        _locations = [new(0, 0)];

        AssignLocations();
    }
}
