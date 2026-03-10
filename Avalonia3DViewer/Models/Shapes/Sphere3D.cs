using System;
using System.Numerics;
using System.Collections.Generic;

namespace Avalonia3DViewer.Models.Shapes;

public class Sphere3D : Shape3D
{
    public Sphere3D(Vector3 position, float size = 0.25f, int latSegments = 8, int lonSegments = 8)
    {
        List<Vector3> vertices = [];
        List<Vector3> faces = [];

        for (var lat = 0; lat <= latSegments; lat++)
        {
            var phi = MathF.PI * 0.5f - (MathF.PI * lat / latSegments); 
            var y = MathF.Sin(phi);
            var radius = MathF.Cos(phi);

            for (var lon = 0; lon <= lonSegments; lon++)
            {
                var theta = (2 * MathF.PI * lon) / lonSegments;  
                var x = radius * MathF.Cos(theta);
                var z = radius * MathF.Sin(theta);

                vertices.Add(new Vector3(x, y, z) * size + position);
            }
        }

        for (var lat = 0; lat < latSegments; lat++)
        {
            var first = lat * (lonSegments + 1);
            var next = first + lonSegments + 1;

            for (var lon = 0; lon < lonSegments; lon++)
            {
                var curr1 = first + lon;
                var curr2 = first + lon + 1;
                var next1 = next + lon;
                var next2 = next + lon + 1;

                faces.Add(new Vector3(curr1, curr2, next1));
                faces.Add(new Vector3(curr2, next2, next1));
            }
        }

        Vertices = vertices;
        Faces = faces;
        Edges = ExtractEdges(faces);
    }

    private static List<Vector2> ExtractEdges(List<Vector3> faces)
    {
        var edgeSet = new HashSet<(int, int)>();
        var edges = new List<Vector2>();

        foreach (var face in faces)
        {
            int[] indices = [(int)face.X, (int)face.Y, (int)face.Z];
            for (var i = 0; i < 3; i++)
            {
                var a = indices[i];
                var b = indices[(i + 1) % 3];
                if (a > b) (a, b) = (b, a); 
                if (edgeSet.Add((a, b)))
                    edges.Add(new Vector2(a, b));
            }
        }
        
        return edges;
    }
}