using SkillQuest.API.Component;
using SkillQuest.API.ECS;
using SkillQuest.API.Network;
using SkillQuest.API.Thing;
using SkillQuest.API.Thing.Character;
using SkillQuest.Shared.Engine.Packet.Component.Interaction;

namespace SkillQuest.Shared.Engine.Entity;

public class Item : Engine.ECS.Entity, IItem{
    string? _name;
    
    public virtual string Name {
        get => _name ?? Uri?.ToString() ?? "Null";
        protected set => _name = value;
    }
    
    public virtual string? Description { get; set; }
    
    private IChannel _channel;

    public Item(){
        _channel = State.SH.Net.CreateChannel(Uri);
        
        _channel.Subscribe<InteractionRequestPacket>( OnInteractionRequestPacket);
        _channel.Subscribe<InteractionResponsePacket>( OnInteractionResponsePacket);
    }

    /// <summary>
    /// Client side request primary function of an item
    /// </summary>
    /// <param name="stack"></param>
    /// <param name="subject"></param>
    /// <param name="target"></param>
    public void Primary( IItemStack stack, ICharacter subject, IEntity target){
        var net = Components.First( comp => comp is INetworkedComponent ).Value as INetworkedComponent;
        foreach (var conn in net.Subscribers) {
            _channel.Send( conn.Value, new InteractionRequestPacket() {
                Type = ItemInteraction.Primary,
                Stack = stack.Uri,
                Subject = subject.Uri,
                Target = target.Uri,
            });
        }
    }

    public delegate void DoPrimary( IItemStack stack, ICharacter subject, IEntity target );
    
    public event DoPrimary OnPrimary;

    void OnInteractionRequestPacket(IClientConnection connection, InteractionRequestPacket packet){
        
    }

    void OnInteractionResponsePacket(IClientConnection connection, InteractionResponsePacket packet){
        
    }
}