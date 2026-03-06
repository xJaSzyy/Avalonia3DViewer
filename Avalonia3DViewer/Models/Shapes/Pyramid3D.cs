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