using System.Collections.Concurrent;
using System.Reflection;
using System.Xml.Linq;
using System.Xml.Serialization;
using SkillQuest.API.ECS;

namespace SkillQuest.Shared.Engine.Doohickey.State.Ledger;

public class ComponentLedger : IDisposable{
    public Type this[Uri uri] {
        get {
            _components.TryGetValue(uri, out var component);
            return component;
        }
        set {
            if (value is null) {
                _components.TryRemove(uri, out _);
            } else {
                if (!value.IsAssignableTo(typeof(IComponent))) return;

                _components.TryGetValue(uri, out var old);

                if (old != value) {
                    var item = _components[uri] = value;
                }
            }
        }
    }

    private ConcurrentDictionary<Uri, Type> _components = new();

    public void AttachTo(IThing thing, Uri uri, XElement xml){
        var type = this[uri];
        if (type is null) return;

        try {
            var serializer = new XmlSerializer(type);
            var c = serializer.Deserialize(xml.CreateReader()) as IComponent;

            thing.Connect(c, type);
        } catch (Exception e) {
            Console.WriteLine(e);
        }
    }

    public void LoadFromXmlFile(string file){
        LoadFromXml(XElement.Load(file));
    }

    public void LoadFromXml(XElement skillquest_root){
        if (!skillquest_root.HasElements) return;
        if (skillquest_root.Name != "SkillQuest") return;

        var components = skillquest_root.Elements("Component");

        foreach (var component in components) {
            var asm = GetAssembly(component.Attribute("asm")?.Value ?? "") ?? Assembly.GetExecutingAssembly();
            var type = GetType(asm, component.Attribute("type")?.Value ?? "");
            if (type is null) continue;
            if (!typeof(IComponent).IsAssignableFrom(type)) continue;

            foreach (var alias in component.Elements("Alias")) {
                var aliasName = alias.Value;
                var aliasUri = new Uri(aliasName);
                this[aliasUri] = type;

                Console.WriteLine($"Added component {type.Name} alias {aliasName}");
            }
        }
    }

    private Assembly? GetAssembly(string name){
        return AppDomain.CurrentDomain.GetAssemblies().FirstOrDefault(assembly => assembly.GetName().Name == name);
    }

    private Type? GetType(Assembly? asm, string typeName){
        return asm?.GetType(typeName);
    }

    public void Dispose(){
        _components.Clear();
    }
}
