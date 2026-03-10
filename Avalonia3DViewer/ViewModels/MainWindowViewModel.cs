using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Numerics;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Shapes;
using Avalonia.Media;
using Avalonia.Threading;
using Avalonia3DViewer.Models.Shapes;

namespace Avalonia3DViewer.ViewModels;

public partial class MainWindowViewModel : ViewModelBase
{
    private const float ShapeSize = .1f;
    
    public ObservableCollection<Control> Shapes { get; } = []; 
    
    private double _canvasWidth;
    private double _canvasHeight;
    
    private readonly List<Shape3D> _currentShapes = [];
    private readonly DispatcherTimer _timer;
    private float _dz = 1;
    private float _angle;
    private float _pitch;  
    
    private bool _isPaused;
    private bool _showNumbers;

    public MainWindowViewModel()
    {
        LoadShapes();
        
        _timer = new DispatcherTimer { Interval = TimeSpan.FromMilliseconds(1000.0 / 60.0) };
        _timer.Tick += (s, e) => Animate();
        _timer.Start();
    }

    private void LoadShapes()
    {
        _currentShapes.Add(new Cube3D(new Vector3(-ShapeSize - ShapeSize / 2, 0, 0), ShapeSize));
        _currentShapes.Add(new Pyramid3D(new Vector3(ShapeSize + ShapeSize / 2, 0, 0), ShapeSize));
    }

    private void UpdateAll()
    {
        Shapes.Clear();

        foreach (var shape in _currentShapes)
        {
            var screenPoints = new List<Vector2>();
            
            foreach (var vertex in shape.Vertices)
            {
                var worldPoint = vertex + shape.Position;
                var rotated = Rotate(worldPoint, _angle);
                var projected = ToProject(rotated);
                var screenPos = ToScreenPosition(projected);
                screenPoints.Add(screenPos);

                var grid = new Grid
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
                    
                    grid.Children.Add(text);
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
                    
                    grid.Children.Add(ellipse);
                }
                
                Shapes.Add(grid);
            }
        
            foreach (var edge in shape.Edges)
            {
                var start = screenPoints[(int)edge.X];
                var end = screenPoints[(int)edge.Y];
            
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
                var point1 = screenPoints[(int)face.X];
                var point2 = screenPoints[(int)face.Y];
                var point3 = screenPoints[(int)face.Z];
                
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
        var minDimension = Math.Min(_canvasWidth, _canvasHeight);
    
        return new Vector2(
            (float)((point.X + 1) / 2 * minDimension + (_canvasWidth - minDimension) / 2),
            (float)((1 - (point.Y + 1) / 2) * minDimension + (_canvasHeight - minDimension) / 2)
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
    
        var pitchCos = MathF.Cos(_pitch);
        var pitchSin = MathF.Sin(_pitch);
        var pitched = new Vector3(
            point.X,
            point.Y * pitchCos - point.Z * pitchSin,
            point.Y * pitchSin + point.Z * pitchCos
        );
    
        return new Vector3(
            pitched.X * cos - pitched.Z * sin,
            pitched.Y,
            pitched.X * sin + pitched.Z * cos
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

    public void Zoom(double y)
    {
        const float step = .1f;
        const float minZoom = 0f;
        const float maxZoom = 1.5f;
        
        _dz += y > 0 ? step : -step;
        _dz = Math.Clamp(_dz, minZoom, maxZoom);
        
        if (_isPaused)
        {
            UpdateAll();
        }
    }
    
    public void MoveVertical(double delta)
    {
        const float sensitivity = 0.02f;  
        _pitch += (float)(delta * sensitivity);
        _pitch = Math.Clamp(_pitch, -MathF.PI / 2 + 0.1f, MathF.PI / 2 - 0.1f);  

        if (_isPaused)
        {
            UpdateAll();
        }
    }
}