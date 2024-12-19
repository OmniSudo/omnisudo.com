using SkillQuest.Engine.API.Network;

namespace SkillQuest.Engine.API.Asset;

public interface IAssetRepository{
    public IPermissionChecker? Permissions { get; set; }
    
    public Task<byte[]> Open(string file, IClientConnection? connection = null);
    
}
