using DormPuzzle.Helpers;

namespace DormPuzzle.Controls.Blocks;

public class Block10 : Block
{
    public Block10() : base(10, 1, 2, BrushHelper.FromHex("#CB6573"))
    {
        _locations = [new(0, 0), new(0, 1)];
        _orderLocations = [new(0, 0), new(0, 1)];

        AssignLocations();
    }
}
