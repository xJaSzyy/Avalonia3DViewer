using System.Collections.Generic;
using System.Numerics;

namespace Avalonia3DViewer.Models.Shapes;

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