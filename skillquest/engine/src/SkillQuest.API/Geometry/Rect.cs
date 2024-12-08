using System.IO.Hashing;
using System.Text;
using Silk.NET.Maths;

namespace SkillQuest.API.Geometry;

public class Rect : Polygon{
    public Rect(Vector2D<float> min, Vector2D<float> max) : base(
        [
            new Edge(new Vector2D<float>(min.X, min.Y), new Vector2D<float>(min.X, max.Y)),
            new Edge(new Vector2D<float>(min.X, max.Y), new Vector2D<float>(max.X, max.Y)),
            new Edge(new Vector2D<float>(max.X, max.Y), new Vector2D<float>(max.X, min.Y)),
            new Edge(new Vector2D<float>(max.X, min.Y), new Vector2D<float>(min.X, min.Y))
        ]
    ){ }

    public Vector2D<float> size(){
        return Max - Min;
    }

    public float width(){
        return Max.X - Min.X;
    }

    public float height(){
        return Max.Y - Min.Y;
    }

    public bool Colliding(Vector2D<float> point){
        return (Min.X <= point.X && Max.X >= point.X) && (Min.Y <= point.Y && Max.Y >= point.Y);
    }

    public override string ToString(){
        return $"[ {Min}, {Max} ]";
    }

    public long ConstHashCode(){
        return BitConverter.ToInt64(
            Crc32.Hash(
                BitConverter.GetBytes(Min.X)
                    .Concat(BitConverter.GetBytes(Min.Y))
                    .Concat(BitConverter.GetBytes(Max.X))
                    .Concat(BitConverter.GetBytes(Max.Y))
                    .ToArray()
            )
        );
    }
}
