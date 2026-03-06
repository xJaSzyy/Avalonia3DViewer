using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Markup.Xaml;
using Avalonia3DViewer.ViewModels;

namespace Avalonia3DViewer.Views;

public partial class MainWindow : Window
{
    public MainWindow()
    {
        AvaloniaXamlLoader.Load(this);
        
        KeyDown += OnKeyDown;
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
            vm.ToggleVertexNumbers();
            e.Handled = true;
        }
    }
}