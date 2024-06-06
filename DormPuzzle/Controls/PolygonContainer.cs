using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Markup;
using System.Windows.Media;
using DormPuzzle.Helpers;
using DormPuzzle.Models;
using DormPuzzle.Polygons;

namespace DormPuzzle.Controls;

[ContentProperty(nameof(Children))]
public class PolygonContainer : Control, INotifyPropertyChanged
{
    public static readonly DependencyProperty ColumnsProperty;
    public static readonly DependencyProperty RowsProperty;
    public static readonly DependencyProperty UniformProperty;

    static PolygonContainer()
    {
        ColumnsProperty = DependencyProperty.Register(nameof(Columns),
                                                      typeof(int),
                                                      typeof(PolygonContainer),
                                                      new PropertyMetadata(1, (a, b) => { ((UIElement)a).InvalidateVisual(); }));

        RowsProperty = DependencyProperty.Register(nameof(Rows),
                                                   typeof(int),
                                                   typeof(PolygonContainer),
                                                   new PropertyMetadata(1, (a, b) => { ((UIElement)a).InvalidateVisual(); }));

        UniformProperty = DependencyProperty.Register(nameof(Uniform),
                                                      typeof(bool),
                                                      typeof(PolygonContainer),
                                                      new PropertyMetadata(true, (a, b) => { ((UIElement)a).InvalidateVisual(); }));

        BorderThicknessProperty.OverrideMetadata(typeof(PolygonContainer),
                                                 new FrameworkPropertyMetadata(new Thickness(2)));

        BorderBrushProperty.OverrideMetadata(typeof(PolygonContainer),
                                             new FrameworkPropertyMetadata(Brushes.Black));
    }

    public event PropertyChangedEventHandler? PropertyChanged;

    public PolygonContainer()
    {
        this.SetDynamicResource(BorderBrushProperty, "ControlStrokeColorForStrongFillWhenOnImageBrush");

        Children.CollectionChanged += (s, e) => InvalidateVisual();
    }

    public int Columns
    {
        get => (int)GetValue(ColumnsProperty);
        set => SetValue(ColumnsProperty, value);
    }

    public int Rows
    {
        get => (int)GetValue(RowsProperty);
        set => SetValue(RowsProperty, value);
    }

    public bool Uniform
    {
        get => (bool)GetValue(UniformProperty);
        set => SetValue(UniformProperty, value);
    }

    public ObservableCollection<Polygon> Children { get; } = [];

    public virtual PolygonContainer Clone()
    {
        PolygonContainer container = new()
        {
            Columns = Columns,
            Rows = Rows,
            Uniform = Uniform,
            BorderBrush = BorderBrush,
            BorderThickness = BorderThickness
        };

        foreach (Polygon child in Children)
        {
            container.Children.Add(child.Clone());
        }

        return container;
    }

    /// <summary>
    /// Get the size of each cell.
    /// </summary>
    /// <param name="cellWidth"></param>
    /// <param name="cellHeight"></param>
    public void GetCellSize(out double cellWidth, out double cellHeight)
    {
        cellWidth = ActualWidth / Columns;
        cellHeight = ActualHeight / Rows;

        if (Uniform)
        {
            double size = Math.Min(cellWidth, cellHeight);

            cellWidth = size;
            cellHeight = size;
        }
    }

    /// <summary>
    /// Get the offset of the location.
    /// </summary>
    /// <param name="location"></param>
    /// <param name="offsetX"></param>
    /// <param name="offsetY"></param>
    public void GetOffset(Location location, out double offsetX, out double offsetY)
    {
        GetCellSize(out double cellWidth, out double cellHeight);

        offsetX = (ActualWidth - cellWidth * Columns) / 2 + location.Column * cellWidth;
        offsetY = (ActualHeight - cellHeight * Rows) / 2 + location.Row * cellHeight;
    }

    public void Render(double width, double height, DrawingContext drawingContext)
    {
        RenderInternal(width, height, drawingContext);
    }

    protected override void OnRender(DrawingContext drawingContext)
    {
        RenderInternal(ActualWidth, ActualHeight, drawingContext);
    }

    private void RenderInternal(double width, double height, DrawingContext drawingContext)
    {
        double cellWidth = width / Columns;
        double cellHeight = height / Rows;

        double offsetX = 0.0;
        double offsetY = 0.0;

        if (Uniform)
        {
            double size = Math.Min(cellWidth, cellHeight);

            offsetX = (width - size * Columns) / 2;
            offsetY = (height - size * Rows) / 2;

            cellWidth = size;
            cellHeight = size;
        }

        double actualWidth = cellWidth * Columns;
        double actualHeight = cellHeight * Rows;

        if (actualWidth <= 0.0 || actualHeight <= 0.0 || Children.Count == 0)
        {
            return;
        }

        drawingContext.PushTransform(new TranslateTransform(offsetX, offsetY));

        // Draw the fill and border of the polygons
        {
            // Because we need to draw lines on the boundary of each polygon,
            // we need to draw the fill first and then the border.

            foreach (Polygon child in Children)
            {
                if (Contains(child))
                {
                    drawingContext.PushTransform(new TranslateTransform(child.Column * cellWidth, child.Row * cellHeight));

                    child.DrawFill(cellWidth, cellHeight, drawingContext);

                    drawingContext.Pop();
                }
            }

            foreach (Polygon child in Children)
            {
                if (Contains(child))
                {
                    drawingContext.PushTransform(new TranslateTransform(child.Column * cellWidth, child.Row * cellHeight));

                    child.DrawBorder(cellWidth, cellHeight, drawingContext);

                    drawingContext.Pop();
                }
            }
        }

        // Draw the outer border
        {
            PathGeometry geometryLeft = new();
            PathGeometry geometryTop = new();
            PathGeometry geometryRight = new();
            PathGeometry geometryBottom = new();

            for (int i = 0; i < Columns; i++)
            {
                for (int j = 0; j < Rows; j++)
                {
                    foreach (Polygon child in Children)
                    {
                        if (child.Column == i && child.Row == j)
                        {
                            if (IsOnBoundary(child, Direction.Left))
                            {
                                geometryLeft.AddGeometry(new LineGeometry(new Point(i * cellWidth, j * cellHeight),
                                                                          new Point(i * cellWidth, (j + 1) * cellHeight)));
                            }

                            if (IsOnBoundary(child, Direction.Top))
                            {
                                geometryTop.AddGeometry(new LineGeometry(new Point(i * cellWidth, j * cellHeight),
                                                                         new Point((i + 1) * cellWidth, j * cellHeight)));
                            }

                            if (IsOnBoundary(child, Direction.Right))
                            {
                                geometryRight.AddGeometry(new LineGeometry(new Point((i + 1) * cellWidth, j * cellHeight),
                                                                           new Point((i + 1) * cellWidth, (j + 1) * cellHeight)));
                            }

                            if (IsOnBoundary(child, Direction.Bottom))
                            {
                                geometryBottom.AddGeometry(new LineGeometry(new Point(i * cellWidth, (j + 1) * cellHeight),
                                                                            new Point((i + 1) * cellWidth, (j + 1) * cellHeight)));
                            }
                        }
                    }
                }
            }

            drawingContext.DrawGeometry(null,
                                        new Pen(BorderBrush, BorderThickness.Left)
                                        {
                                            StartLineCap = PenLineCap.Round,
                                            EndLineCap = PenLineCap.Round
                                        },
                                        geometryLeft);

            drawingContext.DrawGeometry(null,
                                        new Pen(BorderBrush, BorderThickness.Top)
                                        {
                                            StartLineCap = PenLineCap.Round,
                                            EndLineCap = PenLineCap.Round
                                        },
                                        geometryTop);

            drawingContext.DrawGeometry(null,
                                        new Pen(BorderBrush, BorderThickness.Right)
                                        {
                                            StartLineCap = PenLineCap.Round,
                                            EndLineCap = PenLineCap.Round
                                        },
                                        geometryRight);

            drawingContext.DrawGeometry(null,
                                        new Pen(BorderBrush, BorderThickness.Bottom)
                                        {
                                            StartLineCap = PenLineCap.Round,
                                            EndLineCap = PenLineCap.Round
                                        },
                                        geometryBottom);
        }

        // Draw the inner border
        {
            Brush? controlStrongStrokeColorDefaultBrush = ResourceHelper.GetResource<Brush>("ControlStrongStrokeColorDefaultBrush");

            double innerLeftRightMargin = cellWidth / 8.0;
            double innerTopBottomMargin = cellHeight / 8.0;

            PathGeometry geometryLeft = new();
            PathGeometry geometryTop = new();
            PathGeometry geometryRight = new();
            PathGeometry geometryBottom = new();

            for (int i = 0; i < Columns; i++)
            {
                for (int j = 0; j < Rows; j++)
                {
                    foreach (Polygon child in Children)
                    {
                        if (child.Column == i && child.Row == j)
                        {
                            if (IsOnBoundary(child, Direction.Left))
                            {
                                geometryLeft.AddGeometry(new LineGeometry(new Point(i * cellWidth + innerLeftRightMargin, j * cellHeight + innerTopBottomMargin),
                                                                          new Point(i * cellWidth + innerLeftRightMargin, (j + 1) * cellHeight - innerTopBottomMargin)));
                            }

                            if (IsOnBoundary(child, Direction.Top))
                            {
                                geometryTop.AddGeometry(new LineGeometry(new Point(i * cellWidth + innerLeftRightMargin, j * cellHeight + innerTopBottomMargin),
                                                                         new Point((i + 1) * cellWidth - innerLeftRightMargin, j * cellHeight + innerTopBottomMargin)));
                            }

                            if (IsOnBoundary(child, Direction.Right))
                            {
                                geometryRight.AddGeometry(new LineGeometry(new Point((i + 1) * cellWidth - innerLeftRightMargin, j * cellHeight + innerTopBottomMargin),
                                                                           new Point((i + 1) * cellWidth - innerLeftRightMargin, (j + 1) * cellHeight - innerTopBottomMargin)));
                            }

                            if (IsOnBoundary(child, Direction.Bottom))
                            {
                                geometryBottom.AddGeometry(new LineGeometry(new Point(i * cellWidth + innerLeftRightMargin, (j + 1) * cellHeight - innerTopBottomMargin),
                                                                            new Point((i + 1) * cellWidth - innerLeftRightMargin, (j + 1) * cellHeight - innerTopBottomMargin)));
                            }
                        }
                    }
                }
            }

            drawingContext.DrawGeometry(null,
                                        new Pen(controlStrongStrokeColorDefaultBrush, BorderThickness.Left)
                                        {
                                            StartLineCap = PenLineCap.Round,
                                            EndLineCap = PenLineCap.Round
                                        },
                                        geometryLeft);

            drawingContext.DrawGeometry(null,
                                        new Pen(controlStrongStrokeColorDefaultBrush, BorderThickness.Top)
                                        {
                                            StartLineCap = PenLineCap.Round,
                                            EndLineCap = PenLineCap.Round
                                        },
                                        geometryTop);

            drawingContext.DrawGeometry(null,
                                        new Pen(controlStrongStrokeColorDefaultBrush, BorderThickness.Right)
                                        {
                                            StartLineCap = PenLineCap.Round,
                                            EndLineCap = PenLineCap.Round
                                        },
                                        geometryRight);

            drawingContext.DrawGeometry(null,
                                        new Pen(controlStrongStrokeColorDefaultBrush, BorderThickness.Bottom)
                                        {
                                            StartLineCap = PenLineCap.Round,
                                            EndLineCap = PenLineCap.Round
                                        },
                                        geometryBottom);
        }

        drawingContext.Pop();
    }

    private bool Contains(Polygon polygon)
    {
        return polygon.Column >= 0
               && polygon.Column < Columns
               && polygon.Row >= 0
               && polygon.Row < Rows;
    }

    /// <summary>
    /// Is the polygon on the boundary?
    /// </summary>
    /// <param name="polygon"></param>
    /// <param name="direction"></param>
    /// <returns></returns>
    private bool IsOnBoundary(Polygon polygon, Direction direction)
    {
        int column = polygon.Column;
        int row = polygon.Row;

        return direction switch
        {
            Direction.Left => column == 0 || !Children.Any(child => child.Column == column - 1 && child.Row == row),
            Direction.Top => row == 0 || !Children.Any(child => child.Column == column && child.Row == row - 1),
            Direction.Right => column == Columns - 1 || !Children.Any(child => child.Column == column + 1 && child.Row == row),
            Direction.Bottom => row == Rows - 1 || !Children.Any(child => child.Column == column && child.Row == row + 1),
            _ => false,
        };
    }

    protected void SetProperty<T>(ref T field, T value, [CallerMemberName] string? propertyName = null)
    {
        if (EqualityComparer<T>.Default.Equals(field, value))
        {
            return;
        }

        field = value;

        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}
