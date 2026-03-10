using System.Numerics;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Markup.Xaml;
using Avalonia3DViewer.ViewModels;

namespace Avalonia3DViewer.Views;

public partial class MainWindow : Window
{
    private Point _lastMousePos;
    private bool _isMouseDragging;
    
    public MainWindow()
    {
        AvaloniaXamlLoader.Load(this);
        
        KeyDown += OnKeyDown;
        PointerWheelChanged += OnWheelChanged;
        PointerPressed += OnPointerPressed; 
        PointerMoved += OnPointerMoved; 
        PointerReleased += OnPointerReleased;
    }
    
    private void OnCanvasSizeChanged(object sender, SizeChangedEventArgs e)
    {
        if (sender is Canvas canvas && DataContext is MainWindowViewModel vm)
        {
            vm.UpdateCanvasSize(canvas.Bounds.Width, canvas.Bounds.Height);
        }
    }
    
    private void OnKeyDown(object? sender, KeyEventArgs e)
    {
        if (DataContext is not MainWindowViewModel vm)
        {
            return;
        }

        if (e.Key == Key.Space)
        {
            vm.TogglePause();
            e.Handled = true;
        }

        if (e.Key == Key.Tab)
        {
            vm.SwitchMode();
            e.Handled = true;
        }
    }
    
    private void OnWheelChanged(object? sender, PointerWheelEventArgs e)
    {
        if (DataContext is MainWindowViewModel vm)
        {
            vm.Zoom(e.Delta.Y);
            e.Handled = true;
        }
    }

    private void OnPointerPressed(object? sender, PointerPressedEventArgs e)
    {
        if (e.GetCurrentPoint(this).Properties.IsLeftButtonPressed)
        {
            _isMouseDragging = true;
            _lastMousePos = e.GetPosition(this);
            e.Handled = true;
        }
    }

    private void OnPointerMoved(object? sender, PointerEventArgs e)
    {
        if (DataContext is not MainWindowViewModel vm)
        {
            return;
        }
        
        if (_isMouseDragging)
        {
            var pos = e.GetPosition(this);
            var delta = pos - _lastMousePos;
            vm.MoveVertical(-delta.Y);  
            _lastMousePos = pos;
            e.Handled = true;
        }
    }

    private void OnPointerReleased(object? sender, PointerReleasedEventArgs e)
    {
        _isMouseDragging = false;
        e.Handled = true;
    }
}