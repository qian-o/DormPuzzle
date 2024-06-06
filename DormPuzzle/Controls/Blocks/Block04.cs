using DormPuzzle.Helpers;

namespace DormPuzzle.Controls.Blocks;

public class Block04 : Block
{
    public Block04() : base(4, 2, 3, BrushHelper.FromHex("#2BD7C6"))
    {
        _locations = [new(1, 0), new(1, 1), new(0, 1), new(0, 2)];

        AssignLocations();
    }
}
