using System.Net;
using SkillQuest.API.ECS;
using SkillQuest.API.Network;

namespace SkillQuest.API.Component;

public interface INetworkedComponent : IComponent {
    public bool Updated { get; set; }
    
    public Dictionary<IPEndPoint, IClientConnection> Subscribers { get; set; }

    public INetworkedComponent Subscribe(IClientConnection client);
    
    public INetworkedComponent Unsubscribe(IClientConnection client);

    /// <summary>
    /// Download a specific component from the remote
    /// If component is null; download the entire entity
    /// </summary>
    /// <param name="remote"> Usually the server </param>
    /// <param name="component"> Specify to reduce download size </param>
    public INetworkedComponent DownloadFrom(IClientConnection remote, IComponent? component = null);
    
    /// <summary>
    /// Upload a specific component to the client
    /// If component is null; upload the entire entity
    /// </summary>
    /// <param name="client"> Target </param>
    /// <param name="component"> Specify to reduce download size </param>
    public INetworkedComponent UploadTo(IClientConnection? client, IComponent? component = null);
}
