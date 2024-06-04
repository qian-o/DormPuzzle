using System.Windows;
using System.Windows.Media;
using DormPuzzle.Helpers;

namespace DormPuzzle.Polygons;

public class Quad : Polygon
{
    public Quad(int row, int column)
    {
        Row = row;
        Column = column;
    }

    public Quad()
    {
    }

    public override void DrawFill(double width, double height, DrawingContext drawingContext)
    {
        if (width < 0.0 || height < 0.0)
        {
            return;
        }

        Brush? circleElevationBorderBrush = ResourceHelper.GetResource<Brush>("CircleElevationBorderBrush");

        drawingContext.PushOpacity(0.8);
        drawingContext.DrawRectangle(Fill, null, new Rect(0, 0, width, height));
        drawingContext.Pop();

        // Inner fill
        {
            double innerMargin = Math.Min(width, height) / 8.0;
            double halfInnerMargin = innerMargin / 2.0;

            double innerWidth = width - innerMargin;
            double innerHeight = height - innerMargin;

            drawingContext.DrawRectangle(circleElevationBorderBrush, null, new Rect(halfInnerMargin, halfInnerMargin, innerWidth, innerHeight));

            Geometry innerLeft = Geometry.Parse($"M 0,0 L {halfInnerMargin},{halfInnerMargin} {halfInnerMargin},{height - halfInnerMargin} 0,{height} Z");
            Geometry innerTop = Geometry.Parse($"M 0,0 L {halfInnerMargin},{halfInnerMargin} {width - halfInnerMargin},{halfInnerMargin} {width},0 Z");
            Geometry innerRight = Geometry.Parse($"M {width},0 L {width - halfInnerMargin},{halfInnerMargin} {width - halfInnerMargin},{height - halfInnerMargin} {width},{height} Z");
            Geometry innerBottom = Geometry.Parse($"M 0,{height} L {halfInnerMargin},{height - halfInnerMargin} {width - halfInnerMargin},{height - halfInnerMargin} {width},{height} Z");

            drawingContext.DrawDrawing(new GeometryDrawing(circleElevationBorderBrush, null, innerLeft));
            drawingContext.DrawDrawing(new GeometryDrawing(circleElevationBorderBrush, null, innerTop));
            drawingContext.DrawDrawing(new GeometryDrawing(circleElevationBorderBrush, null, innerRight));
            drawingContext.DrawDrawing(new GeometryDrawing(circleElevationBorderBrush, null, innerBottom));
        }
    }

    public override void DrawBorder(double width, double height, DrawingContext drawingContext)
    {
        if (width < 0.0 || height < 0.0)
        {
            return;
        }

        drawingContext.DrawLine(new Pen(BorderBrush, BorderThickness.Left),
                                new Point(0, 0),
                                new Point(0, height));

        drawingContext.DrawLine(new Pen(BorderBrush, BorderThickness.Top),
                                new Point(0, 0),
                                new Point(width, 0));

        drawingContext.DrawLine(new Pen(BorderBrush, BorderThickness.Right),
                                new Point(width, 0),
                                new Point(width, height));

        drawingContext.DrawLine(new Pen(BorderBrush, BorderThickness.Bottom),
                                new Point(0, height),
                                new Point(width, height));
    }

    public override Polygon Clone()
    {
        return new Quad(Row, Column)
        {
            Fill = Fill,
            BorderBrush = BorderBrush,
            BorderThickness = BorderThickness
        };
    }
}
