using System.Numerics;

namespace Avalonia3DViewer.Models.Shapes;

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
            new Vector2(0, 1), new Vector2(1, 2), new Vector2(2, 3), new Vector2(3, 0),
            new Vector2(4, 5), new Vector2(5, 6), new Vector2(6, 7), new Vector2(7, 4),
            new Vector2(0, 4), new Vector2(1, 5), new Vector2(2, 6), new Vector2(3, 7)
        ];

        Faces =
        [
            new Vector3(0, 1, 2), new Vector3(0, 2, 3), 
            new Vector3(1, 2, 6), new Vector3(1, 5, 6),
            new Vector3(4, 5, 6), new Vector3(4, 7, 6),
            new Vector3(0, 3, 7), new Vector3(0, 4, 7),
            new Vector3(0, 1, 5), new Vector3(0, 4, 5),
            new Vector3(2, 3, 7), new Vector3(2, 6, 7),
        ];

        Position = position;
    }
}