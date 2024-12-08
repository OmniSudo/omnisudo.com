using System.Text.Json.Nodes;

namespace SkillQuest.API.Network;

/// <summary>
/// A connection located on the server
/// </summary>
public interface ILocalConnection: IClientConnection{
    public IServerConnection Server { get; }
}
