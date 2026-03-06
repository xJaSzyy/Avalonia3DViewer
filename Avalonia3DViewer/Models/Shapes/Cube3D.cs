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