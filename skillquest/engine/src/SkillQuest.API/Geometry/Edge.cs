using System.Drawing;
using System.IO.Hashing;
using Silk.NET.Maths;

namespace SkillQuest.API.Geometry;

public class Edge{
    public Vector2D<float> PointA { get; init; }
    public Vector2D<float> PointB { get; init; }

    public Edge(Vector2D<float> pointA, Vector2D<float> pointB){
        PointA = pointA;
        PointB = pointB;
    }
    
    public override string ToString(){
        return $"[ {PointA}, {PointB} ]";
    }

    public long ConstHashCode (){
        return BitConverter.ToInt64(
            Crc32.Hash(
                BitConverter.GetBytes(PointA.X)
                    .Concat(BitConverter.GetBytes(PointA.Y))
                    .Concat(BitConverter.GetBytes(PointB.X))
                    .Concat(BitConverter.GetBytes(PointB.Y))
                    .ToArray()
            )
        );
    }

    public override bool Equals(object? obj) {
        return obj is Edge edge && edge.PointA == PointA && edge.PointB == PointB;
    }

    public bool Intersects(Edge other) {
        var x1 = this.PointA.X;
        var x2 = this.PointB.X;
        var x3 = other.PointA.X;
        var x4 = other.PointB.X;

        var y1 = this.PointA.Y;
        var y2 = this.PointB.Y;
        var y3 = other.PointA.Y;
        var y4 = other.PointB.Y;

        // calculate the direction of the lines
        var uA = ((x4-x3)*(y1-y3) - (y4-y3)*(x1-x3)) / ((y4-y3)*(x2-x1) - (x4-x3)*(y2-y1));
        var uB = ((x2-x1)*(y1-y3) - (y2-y1)*(x1-x3)) / ((y4-y3)*(x2-x1) - (x4-x3)*(y2-y1));

        // if uA and uB are between 0-1, lines are colliding
        return uA >= 0 && uA <= 1 && uB >= 0 && uB <= 1;
    }
}
