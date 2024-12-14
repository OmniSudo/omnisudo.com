using SkillQuest.API.Network;

namespace SkillQuest.API.Asset;

public interface IAssetRepository{
    public Task<byte[]> Open(string file, IClientConnection? connection = null);
    
}
