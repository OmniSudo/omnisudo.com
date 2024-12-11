using SkillQuest.API.Network;

namespace SkillQuest.API.Asset;

public interface IAssetRepository{
    public IPermissionChecker? Permissions { get; set; }
    
    public Task<byte[]> Open(IClientConnection? connection, string file);
}
