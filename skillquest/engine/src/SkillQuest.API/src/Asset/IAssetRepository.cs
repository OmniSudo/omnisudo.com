using SkillQuest.API.Network;

namespace SkillQuest.API.Asset;

public interface IAssetRepository{
    public IPermissionChecker? Permissions { get; set; }
    
    public Task<byte[]> Open(string file, IClientConnection? connection = null);
}
