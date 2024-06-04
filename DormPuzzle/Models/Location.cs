namespace DormPuzzle.Models;

public struct Location(int row, int column)
{
    public int Row = row;

    public int Column = column;

    public static Location operator +(Location left, Location right)
    {
        return new Location(left.Row + right.Row, left.Column + right.Column);
    }

    public static Location operator -(Location left, Location right)
    {
        return new Location(left.Row - right.Row, left.Column - right.Column);
    }
}
