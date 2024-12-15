namespace SkillQuest.API.Network;

/// <summary>
/// A connection located on the client
/// </summary>
public interface IRemoteConnection: IClientConnection {
    
    public void Connect();
}
