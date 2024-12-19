namespace SkillQuest.Engine.API.Network;

/// <summary>
/// A connection located on the server
/// </summary>
public interface ILocalConnection: IClientConnection{
    public IServerConnection Server { get; }
}
