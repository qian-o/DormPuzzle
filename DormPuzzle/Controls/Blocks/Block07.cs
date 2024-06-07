using DormPuzzle.Helpers;

namespace DormPuzzle.Controls.Blocks;

public class Block07 : Block
{
    public Block07() : base(7, 2, 3, BrushHelper.FromHex("#CEA229"))
    {
        _locations = [new(1, 0), new(1, 1), new(1, 2), new(0, 1)];
        _orderLocations = [new(0, 1), new(1, 1)];

        AssignLocations();
    }
}
