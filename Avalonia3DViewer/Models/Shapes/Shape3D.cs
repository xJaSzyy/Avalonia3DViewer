using System.Collections.Generic;
using System.Numerics;

namespace Avalonia3DViewer.Models.Shapes;

public class Shape3D
{
    public List<Vector3> Vertices { get; protected set; }
    public List<Vector2> Edges { get; protected set; }
    public List<Vector3> Faces { get; protected set; }
    public Vector3 Position { get; protected set; }
    
    protected Shape3D()
    {
        Vertices = [];
        Edges = [];
        Faces = [];
    }
}