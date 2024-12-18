namespace SkillQuest.Shared.Engine.Packet.Component.Interaction;

public class InteractionResponsePacket : API.Network.Packet {
    public ItemInteraction Type {
        get;
        set;
    }

    public Uri? Stack {
        get;
        set;
    }

    public Uri? Subject {
        get;
        set;
    }

    public Uri? Target {
        get;
        set;
    }
}
