using System.Windows.Media;

namespace DormPuzzle.Helpers;

public static class BrushHelper
{
    public static SolidColorBrush FromHex(string hex)
    {
        return new SolidColorBrush((Color)ColorConverter.ConvertFromString(hex));
    }
}
