using System.Windows;
using System.Windows.Media;
using DormPuzzle.Controls;

namespace DormPuzzle.Polygons;

public abstract class Polygon : DependencyObject
{
    public static readonly DependencyProperty ColumnProperty;
    public static readonly DependencyProperty RowProperty;
    public static readonly DependencyProperty FillProperty;
    public static readonly DependencyProperty BorderThicknessProperty;
    public static readonly DependencyProperty BorderBrushProperty;

    static Polygon()
    {
        ColumnProperty = DependencyProperty.Register(nameof(Column),
                                                     typeof(int),
                                                     typeof(Quad),
                                                     new PropertyMetadata(0));

        RowProperty = DependencyProperty.Register(nameof(Row),
                                                  typeof(int),
                                                  typeof(Quad),
                                                  new PropertyMetadata(0));

        FillProperty = DependencyProperty.Register(nameof(DrawFill),
                                                   typeof(Brush),
                                                   typeof(Quad),
                                                   new PropertyMetadata(Brushes.Black));


        BorderThicknessProperty = DependencyProperty.Register(nameof(BorderThickness),
                                                              typeof(Thickness),
                                                              typeof(PolygonContainer),
                                                              new PropertyMetadata(new Thickness(1.0)));

        BorderBrushProperty = DependencyProperty.Register(nameof(BorderBrush),
                                                          typeof(Brush),
                                                          typeof(PolygonContainer),
                                                          new PropertyMetadata(Brushes.Gray));
    }

    public int Column
    {
        get { return (int)GetValue(ColumnProperty); }
        set { SetValue(ColumnProperty, value); }
    }

    public int Row
    {
        get { return (int)GetValue(RowProperty); }
        set { SetValue(RowProperty, value); }
    }

    public Brush Fill
    {
        get { return (Brush)GetValue(FillProperty); }
        set { SetValue(FillProperty, value); }
    }

    public Thickness BorderThickness
    {
        get { return (Thickness)GetValue(BorderThicknessProperty); }
        set { SetValue(BorderThicknessProperty, value); }
    }

    public Brush BorderBrush
    {
        get { return (Brush)GetValue(BorderBrushProperty); }
        set { SetValue(BorderBrushProperty, value); }
    }

    public abstract void DrawFill(double width, double height, DrawingContext drawingContext);

    public abstract void DrawBorder(double width, double height, DrawingContext drawingContext);

    public abstract Polygon Clone();
}
