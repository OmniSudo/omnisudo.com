using System.Xml.Serialization;
using SkillQuest.API.Component;
using SkillQuest.Shared.Engine.ECS;

namespace SkillQuest.Game.Base.Shared.Component.Material.Metallurgy;

[XmlRoot("Component")]
public class Fuel : Component<Fuel>, INetworkedComponent{ }
