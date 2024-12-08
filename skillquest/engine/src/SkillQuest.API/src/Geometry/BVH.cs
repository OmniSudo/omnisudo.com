using Silk.NET.Maths;

namespace SkillQuest.API.Geometry;

public class BVH{
     public Polygon? Intersect(Vector2D<float> pos) {
        return Intersect(_root, pos);
    }

    public HashSet<Polygon> Intersect(Polygon bounds) {
        return Intersect(_root, bounds);
    }

    private Polygon? Intersect(Node branch, Vector2D<float> pos) {
        if (!branch.bounds.Colliding(pos)) {
            return null;
        }

        Polygon ret = null;

        if (branch.children.Count != 0) {
            Node child = branch.children[ 0 ];
            ret = Intersect(child, pos);
            if (ret != null) {
                return ret;
            }
            if (branch.children.Count > 1) {
                child = branch.children[ 1 ];
                ret = Intersect(child, pos);
                return ret;
            }
        }

        foreach (var polygon in branch.polygons) {
            if (polygon.Colliding(pos))
                return polygon;
        }
        return null;
    }

    private HashSet<Polygon> Intersect(Node branch, Polygon bounds) {
        HashSet<Polygon> ret = new();

        if (!branch.bounds.Colliding(bounds)) {
            return ret;
        }

        if (branch.children.Count == 0) {
            foreach (var child in branch.children) {
                foreach (var intersector in Intersect(child, bounds)) {
                    ret.Add(intersector);
                }
            }
            return ret;
        }

        foreach (var polygon in branch.polygons) {
            if (polygon.Colliding(bounds))
                ret.Add(polygon);
        }
        return ret;
    }

    private class Node {
        public Rect bounds;
        public HashSet<Polygon> polygons;
        public List<Node> children = new();

        public Node(HashSet<Polygon> polygons) {
            this.polygons = new(polygons);

            if (polygons.Count > 0) {
                this.bounds = new Rect(
                    new Vector2D<float>(
                        this.polygons.AsParallel().Min(polygon => polygon.Bounds.Min.X),
                        this.polygons.AsParallel().Min(polygon => polygon.Bounds.Min.Y)
                    ),
                    new Vector2D<float>(
                        this.polygons.AsParallel().Max(polygon => polygon.Bounds.Min.X),
                        this.polygons.AsParallel().Max(polygon => polygon.Bounds.Min.Y)
                    )
                );
            } else {
                this.bounds = new Rect(new Vector2D<float>(0, 0), new Vector2D<float>(0, 0));
            }
        }
    }

    private Node _root;

    public BVH(HashSet<Polygon> polygons) {
        _root = BuildBVH(new Node(polygons));
    }

    public BVH(HashSet<Polygon> polygons, BVH other) {
        HashSet<Polygon> p = new(polygons);
        if (other != null) {
            foreach (var poly in other._root.polygons) {
                p.Add(poly);
            }
        }
        _root = BuildBVH(new Node(p));
    }

    public HashSet<Polygon> Polygons => _root.polygons;

    private enum AXIS {
        X, Y, Z
    }

    private Node BuildBVH(Node node) {
        Rect bounds = node.bounds;
        Vector2D<float> delta = bounds.Max - bounds.Min;

        AXIS axis = AXIS.X;
        if (delta.Y > delta.X) {
            axis = AXIS.Y;
        }

        float val = 0;
        foreach (var polygon in node.polygons) {
            if (polygon.Open) continue;
            
            Vector2D<float> center = polygon.Bounds.Center!.Value;
            val += axis == AXIS.Y ? center.Y : center.X;
        }
        val /= node.polygons.Count;

        HashSet<Polygon> left = new();
        HashSet<Polygon> right = new();

        foreach (var polygon in node.polygons) {
            double pVal = axis == AXIS.X ? polygon.Center!.Value.X : polygon.Center!.Value.Y;
            if (pVal <= val) {
                left.Add(polygon);
            } else {
                right.Add(polygon);
            }
        }

        branch(node, left);
        branch(node, right);
        return node;
    }

    private void branch(Node root, HashSet<Polygon> branch) {
        if (branch.Count == 0) {
            return;
        }
        if (branch.Count == 1) {
            root.children.Add(new Node(branch));
            return;
        }
        root.children.Add(BuildBVH(new Node(branch)));
    }
}
