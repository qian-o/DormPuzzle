using DormPuzzle.Helpers;

namespace DormPuzzle.Controls.Blocks;

public class Block06 : Block
{
    public Block06() : base(6, 2, 3, BrushHelper.FromHex("#60D32A"))
    {
        _locations = [new(1, 0), new(1, 1), new(1, 2), new(0, 2)];
        _orderLocations = [new(0, 2), new(1, 2)];

        AssignLocations();
    }
}
