using System.Collections.ObjectModel;
using Silk.NET.Maths;

namespace SkillQuest.API.Geometry;

public interface TwoDimensional{
    public enum Axis{
        X,
        Y
    }

    Vector2D<float>? Center { get; init; }

    List<Edge> Edges { get; init; }

    List<Vector2D<float>>? Points { get; init; }

    bool Open { get; init; }

    bool Colliding(Vector2D<float> pos);
    bool Colliding(TwoDimensional? shape);
    bool Intersects(Edge edge);

}
