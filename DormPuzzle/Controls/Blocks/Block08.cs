using DormPuzzle.Helpers;

namespace DormPuzzle.Controls.Blocks;

public class Block08 : Block
{
    public Block08() : base(8, 3, 3, BrushHelper.FromHex("#C0E82E"))
    {
        _locations = [new(1, 0), new(1, 1), new(1, 2), new(0, 1), new(2, 1)];
        _orderLocations = [new(0, 1), new(1, 1), new(2, 1)];

        AssignLocations();
    }
}
