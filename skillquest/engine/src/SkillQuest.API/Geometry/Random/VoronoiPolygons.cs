using System.Collections.Concurrent;
using System.Collections.Immutable;
using System.IO.Hashing;
using System.Runtime.CompilerServices;
using SharpVoronoiLib;
using Silk.NET.Maths;

namespace SkillQuest.API.Geometry.Random;

public class VoronoiPolygons{
    BVH tree = new BVH(new());

    public Polygon at(Vector2D<float> pos){
        if (tree != null) {
            return tree.Intersect(pos);
        }
        return null;
    }

    public HashSet<Polygon> At(Polygon bounds){
        if (tree != null) {
            return tree.Intersect(bounds);
        }
        return null;
    }

    public VoronoiPolygons(long seed, float spacing){
        Seed = seed;
        Spacing = spacing;
    }

    public long Seed { get; init; }

    public float Spacing { get; init; }

    private Vector2D<float> RandomPointWithin(Vector2D<float> min, Vector2D<float> max){
        var seed = Crc32.Hash(
            BitConverter.GetBytes(Seed)
                .Concat(BitConverter.GetBytes(min.X))
                .Concat(BitConverter.GetBytes(min.Y))
                .Concat(BitConverter.GetBytes(max.X))
                .Concat(BitConverter.GetBytes(max.Y))
                .ToArray()
        );
        var rand = new System.Random(BitConverter.ToInt32(seed));

        return new Vector2D<float>(
            min.X + rand.NextSingle() * (max.X - min.X),
            min.Y + rand.NextSingle() * (max.Y - min.Y)
        );
    }

    private HashSet<Polygon> CreatePolygons(Rect bounds){
        var ret = new List<VoronoiSite>();

        float minx = bounds.Min.X / Spacing * Spacing - Spacing;
        float miny = bounds.Min.Y / Spacing * Spacing - Spacing;
        float maxx = bounds.Max.X / Spacing * Spacing + Spacing * 2f;
        float maxy = bounds.Max.Y / Spacing * Spacing + Spacing * 2f;

        for (float x = minx; x <= maxx; x += Spacing) {
            for (float y = miny; y <= maxy; y += Spacing) {
                var pos = RandomPointWithin(new Vector2D<float>(x, y), new Vector2D<float>(x + Spacing, y + Spacing));
                ret.Add(new VoronoiSite(pos.X, pos.Y));
            }
        }

        var edges = VoronoiPlane.TessellateOnce(
            ret,
            minx, miny,
            maxx, maxy,
            BorderEdgeGeneration.DoNotMakeBorderEdges
        );
        return PolygonsFromEdges(bounds, edges);
    }

    public async Task Add(Rect r){
        await Task.Run(() => tree = new BVH(CreatePolygons(r), tree));
    }

    private HashSet<Polygon> PolygonsFromEdges(Rect bounds, List<VoronoiEdge> edges){
        var sitesRight = edges
            .GroupBy(edge => edge.Right.Centroid)
            .ToImmutableDictionary(
                group => new Vector2D<float>((float)group.Key.X, (float)group.Key.Y),
                group => group.First().Right?.Cell
            );

        var sitesLeft = edges
            .GroupBy(edge => edge.Left.Centroid)
            .ToImmutableDictionary(
                group => new Vector2D<float>((float)group.Key.X, (float)group.Key.Y),
                group => group.First().Left?.Cell
            );

        ConcurrentDictionary<Vector2D<float>, List<Edge>> sites = new();

        foreach (var pair in sitesRight) {
            var p = pair.Key;
            var e = pair.Value;

            if (!sites.ContainsKey(p)) {
                sites.TryAdd(p, new List<Edge>());
                sites[p].AddRange(e.Select(edge =>
                    new Edge(new Vector2D<float>((float)edge.Start!.X, (float)edge.Start!.Y),
                        new Vector2D<float>((float)edge.End!.X, (float)edge.End!.Y))));
            }
        }

        foreach (var pair in sitesLeft) {
            var p = pair.Key;
            var e = pair.Value;

            if (!sites.ContainsKey(p)) {
                sites.TryAdd(p, new List<Edge>());
                sites[p].AddRange(e.Select(edge =>
                    new Edge(new Vector2D<float>((float)edge.Start!.X, (float)edge.Start!.Y),
                        new Vector2D<float>((float)edge.End!.X, (float)edge.End!.Y))));
            }
        }

        HashSet<Polygon> polygons = new();

        foreach (var pair in sites) {
            var p = pair.Key;
            var e = pair.Value;

            var polygon = new Polygon(e);
            if (polygon.Open) continue;
            if (polygon.Colliding(bounds)) polygons.Add(polygon);
        }

        return polygons;
    }

    public void Remove(Rect r){
        HashSet<Polygon> polygons = CreatePolygons(r);
        HashSet<Polygon> newPolygons = tree.Polygons;
        newPolygons.RemoveWhere(polygon => polygons.Contains(polygon));
        tree = new BVH(newPolygons);
    }

    public HashSet<Polygon> Polygons => tree.Polygons;
}
