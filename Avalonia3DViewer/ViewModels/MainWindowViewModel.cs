using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Numerics;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Shapes;
using Avalonia.Media;
using Avalonia.Threading;

namespace Avalonia3DViewer.ViewModels;

public partial class MainWindowViewModel : ViewModelBase
{
    public ObservableCollection<Control> Shapes { get; } = new(); 
    
    private double _canvasWidth;
    private double _canvasHeight;
    
    private List<Shape3D> _currentShapes = [];
    private readonly DispatcherTimer _timer;
    private float _dz = 1;
    private float _angle;
    private const float Size = .1f;

    private bool _isPaused = false;
    private bool _showNumbers = false;

    public MainWindowViewModel()
    {
        LoadShapes();
        
        _timer = new DispatcherTimer { Interval = TimeSpan.FromMilliseconds(1000.0 / 60.0) };
        _timer.Tick += (s, e) => Animate();
        _timer.Start();
    }

    private void LoadShapes()
    {
        _currentShapes.Add(new Cube3D(new Vector3(-Size - Size/2, 0, 0), Size));
        _currentShapes.Add(new Pyramid3D(new Vector3(Size + Size/2, 0, 0), Size));
    }

    private void UpdateAll()
    {
        Shapes.Clear();

        for (var i = 0; i < _currentShapes.Count; i++)
        {
            var screenPoints = new List<Vector2>();

            var shape = _currentShapes[i];
            
            foreach (var vertex in shape.Vertices)
            {
                var worldPoint = vertex + shape.Position;
                var rotated = Rotate(worldPoint, _angle);
                var projected = ToProject(rotated);
                var screenPos = ToScreenPosition(projected);
                screenPoints.Add(screenPos);

                var vertexContainer = new Grid
                {
                    Width = 32,
                    Height = 32,
                    RenderTransform = new TranslateTransform(screenPos.X - 16, screenPos.Y - 16)
                };

                if (_showNumbers)
                {
                    var text = new TextBlock
                    {
                        Text = shape.Vertices.IndexOf(vertex).ToString(),
                        HorizontalAlignment = Avalonia.Layout.HorizontalAlignment.Center,
                        VerticalAlignment = Avalonia.Layout.VerticalAlignment.Center,
                        Foreground = Brushes.White,
                        FontSize = 16,
                        FontWeight = FontWeight.Bold
                    };
                    
                    vertexContainer.Children.Add(text);
                }
                else
                {
                    var ellipse = new Ellipse
                    {
                        Width = 8,
                        Height = 8,
                        Fill = Brushes.Red,
                        HorizontalAlignment = Avalonia.Layout.HorizontalAlignment.Center,
                        VerticalAlignment = Avalonia.Layout.VerticalAlignment.Center
                    };
                    
                    vertexContainer.Children.Add(ellipse);
                }
                
                Shapes.Add(vertexContainer);
            }
        
            foreach (var edge in shape.Edges)
            {
                var start = screenPoints[edge.Item1];
                var end = screenPoints[edge.Item2];
            
                var edgeShape = new Line
                {
                    StartPoint = new Point(start.X, start.Y),
                    EndPoint = new Point(end.X, end.Y),
                    Stroke = Brushes.Green,
                    StrokeThickness = 1
                };
            
                Shapes.Add(edgeShape);
            }

            foreach (var face in shape.Faces)
            {
                var point1 = screenPoints[face.Item1];
                var point2 = screenPoints[face.Item2];
                var point3 = screenPoints[face.Item3];
                
                var faceShape = new Polygon
                {
                    Points = new List<Point>
                    {
                        new(point1.X, point1.Y),
                        new(point2.X, point2.Y),
                        new(point3.X, point3.Y),
                    },
                    Fill = Brushes.White,
                    Opacity = .25f
                };
                
                Shapes.Add(faceShape);
            }
        }
    }

    private void Animate()
    {
        if (_isPaused)
        {
            return;
        }
        
        const float dt = 1f / 60f;
        _angle += MathF.PI * 2 * dt * 0.5f;
        
        UpdateAll();
    }

    private Vector2 ToScreenPosition(Vector2 point)
    {
        return new Vector2(
            (float)((point.X + 1) / 2 * _canvasWidth),
            (float)((1 - (point.Y + 1) / 2) * _canvasHeight)
        );
    }

    private Vector2 ToProject(Vector3 point)
    {
        var scale = 1.0f / (0.5f + point.Z * _dz); 
        return new Vector2(point.X * scale, point.Y * scale);
    }

    private Vector3 Rotate(Vector3 point, float angle)
    {
        var cos = MathF.Cos(angle);
        var sin = MathF.Sin(angle);
        
        return new Vector3(
            point.X * cos - point.Z * sin,
            point.Y,
            point.X * sin + point.Z * cos
        );
    }

    public void UpdateCanvasSize(double width, double height)
    {
        _canvasWidth = width;
        _canvasHeight = height;
        
        UpdateAll();
    }
    
    public void TogglePause() 
    {
        _isPaused = !_isPaused;
    }

    public void ToggleVertexNumbers()
    {
        _showNumbers = !_showNumbers;

        if (_isPaused)
        {
            UpdateAll();
        }
    }
}

public class Shape3D
{
    public List<Vector3> Vertices { get; protected set; }
    public List<(int, int)> Edges { get; protected set; }
    public List<(int, int, int)> Faces { get; protected set; }
    public Vector3 Position { get; protected set; }
    
    protected Shape3D()
    {
        Vertices = [];
        Edges = [];
        Faces = [];
    }
}

public class Cube3D : Shape3D
{
    public Cube3D(Vector3 position, float size = 0.25f)
    {
        Vertices =
        [
            new Vector3(-size, -size, size),
            new Vector3(size, -size, size),
            new Vector3(size, size, size),
            new Vector3(-size, size, size),
            new Vector3(-size, -size, -size),
            new Vector3(size, -size, -size),
            new Vector3(size, size, -size),
            new Vector3(-size, size, -size)
        ];

        Edges =
        [
            (0, 1), (1, 2), (2, 3), (3, 0),
            (4, 5), (5, 6), (6, 7), (7, 4),
            (0, 4), (1, 5), (2, 6), (3, 7)
        ];

        Faces =
        [
            (0, 1, 2), (0, 2, 3), 
            (1, 2, 6), (1, 5, 6),
            (4, 5, 6), (4, 7, 6),
            (0, 3, 7), (0, 4, 7),
            (0, 1, 5), (0, 4, 5),
            (2, 3, 7), (2, 6, 7),
        ];

        Position = position;
    }
}

public class Pyramid3D : Shape3D
{
    public Pyramid3D(Vector3 position, float size = 0.25f)
    {
        Vertices =
        [
            new Vector3(-size, -size, size),
            new Vector3(size, -size, size),
            new Vector3(-size, -size, -size),
            new Vector3(size, -size, -size),
            new Vector3(0, size * 2, 0)
        ];

        Edges =
        [
            (0, 4), (1, 4), (2, 4), (3, 4),
            (0, 1), (1, 3), (2, 3), (2, 0)
        ];

        Faces =
        [
            (0, 1, 4), (2, 3, 4), (0, 2, 4), (1, 3, 4),
            (0, 1, 2), (1, 2, 3)
        ];

        Position = position;
    }
}