using System.Collections.Concurrent;
using System.Numerics;
using ImGuiNET;
using SkillQuest.API.ECS;
using SkillQuest.Client.Engine.Graphics.API;

namespace SkillQuest.Game.Base.Client.System.Gui.Editor;

public class GuiElementThingExplorer : global::SkillQuest.Shared.Engine.ECS.System, IDrawable {
    public override Uri? Uri { get; set; } = new Uri("ui://skill.quest/editor/explorer");

    public GuiElementThingExplorer(){
        Tracked += (stuff, thing) => {
            Ledger.ThingAdded += StuffOnThingAdded;
            Ledger.ThingRemoved += StuffOnThingRemoved;
            foreach (var t in Ledger.Entities) {
                StuffOnThingAdded(t.Value);
            }
        };

        Untracked += (stuff, thing) => {
            Tree.Clear();
            Ledger.ThingAdded -= StuffOnThingAdded;
            Ledger.ThingRemoved -= StuffOnThingRemoved;
        };
    }

    private class DirNode{
        public DirNode Parent;
        public string Name;
        public ConcurrentDictionary<string, DirNode> Children;
        public bool Opened = false;
        public IEntity? Thing;
    }
    
    ConcurrentDictionary<string, DirNode> Tree = new ConcurrentDictionary<string, DirNode>();
    
    void StuffOnThingAdded(IEntity iEntity){
        var root = iEntity.Uri.Scheme + "://" + iEntity.Uri.Host;
        if (!Tree.ContainsKey(root)) Tree[root] = new DirNode() { Name = root };
        var tree = Tree[root];

        foreach (var path in iEntity.Uri.Segments) {
            string p = path.Trim('/');
            if (p.Length == 0) continue;
            if (tree.Children is null) tree.Children = new();
            if (!tree.Children.ContainsKey(p)) tree.Children[p] = new DirNode() { Name = p, Parent = tree };
            tree = tree.Children[p];
            
        }
        tree.Thing = iEntity;
    }

    void StuffOnThingRemoved(IEntity iEntity){
        var root = iEntity.Uri.Scheme + "://" + iEntity.Uri.Host;
        if (!Tree.ContainsKey(root)) return;
        var tree = Tree[root];

        foreach (var path in iEntity.Uri.Segments.Reverse().Skip(1).Reverse()) {
            string p = path.Trim('/');
            if (p.Length == 0) continue;
            if (tree.Children is null) return;
            if (!tree.Children.ContainsKey(p)) return;
            tree = tree.Children[p];
        }
        tree.Children.Remove(iEntity.Uri.Segments.Last(), out _);

        while ( (tree?.Children?.Count ??-1)== 0 ) {
            var prev = tree;
            tree = tree.Parent;
            tree?.Children?.Remove( prev.Name, out _ );
        }
    }

    public void Draw(DateTime now, TimeSpan delta){
        if (ImGui.BeginChild("#explorer", new Vector2(250, -1), ImGuiChildFlags.Border)) {
            foreach (var node in Tree.OrderBy(n => n.Value.Name)) {
                DrawNode(node.Value);
            }
            ImGui.EndChild();
        }
    }

    private void DrawNode(DirNode node){
        ImGui.TreePush(node.Name);
        ImGui.PushID(node.Name);
        
        if (ImGui.Button( " " ) ) {
            node.Opened = !node.Opened;
        }
        
        ImGui.SameLine();
        ImGui.Text(node.Name);

        if (node.Opened) {
            if (node.Children is not null) {
                foreach (var child in node.Children.OrderBy(child => child.Value.Name)) {
                    DrawNode(child.Value);
                }
            }
        }
        
        ImGui.PopID();
        ImGui.TreePop();
    }
}
