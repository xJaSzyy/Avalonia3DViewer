using System.Numerics;

namespace Avalonia3DViewer.Models.Shapes;

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
            new Vector3(0, size, 0)
        ];

        Edges =
        [
            new Vector2(0, 4), new Vector2(1, 4), new Vector2(2, 4), new Vector2(3, 4),
            new Vector2(0, 1), new Vector2(1, 3), new Vector2(2, 3), new Vector2(2, 0)
        ];

        Faces =
        [
            new Vector3(0, 1, 4), new Vector3(2, 3, 4), 
            new Vector3(0, 2, 4), new Vector3(1, 3, 4),
            new Vector3(0, 1, 2), new Vector3(1, 2, 3)
        ];

        Position = position;
    }
}