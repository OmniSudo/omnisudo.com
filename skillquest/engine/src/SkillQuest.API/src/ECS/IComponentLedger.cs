using System.Reflection;
using System.Xml.Linq;

namespace SkillQuest.API.ECS;

public interface IComponentLedger{
    Type this[Uri uri] { get; set; }
    
    public Uri? this[Type type] { get; }

    public void AttachTo(IEntity iEntity, Uri uri, XElement xml);

    public void LoadFromXmlFile(string file);

    public void LoadFromXml(XElement skillquest_root);
}
