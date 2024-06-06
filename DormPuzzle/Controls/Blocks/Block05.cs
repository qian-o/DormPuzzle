using DormPuzzle.Helpers;

namespace DormPuzzle.Controls.Blocks;

public class Block05 : Block
{
    public Block05() : base(5, 2, 3, BrushHelper.FromHex("#A8DD2C"))
    {
        _locations = [new(0, 0), new(1, 0), new(1, 1), new(1, 2)];

        AssignLocations();
    }
}
