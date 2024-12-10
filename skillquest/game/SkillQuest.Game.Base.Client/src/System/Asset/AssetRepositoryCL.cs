using System.Collections.Concurrent;
using System.Security.Cryptography;
using SkillQuest.Addon.Base.Shared.Packet.System.Asset;
using SkillQuest.API.Asset;
using SkillQuest.API.Network;
using static SkillQuest.Shared.Engine.State;

namespace SkillQuest.Addon.Base.Client.System.Asset;

public class AssetRepositoryCL : SkillQuest.Shared.Engine.ECS.System, IAssetRepository{
    readonly IChannel _channel;

    public override Uri? Uri { get; set; } = new Uri("cl://asset.skill.quest/");

    public AssetRepositoryCL(){
        _channel = SH.Net.CreateChannel(Uri);

        _channel.Subscribe<AssetRepositoryFileResponsePacket>(OnAssetRepositoryFileResponsePacket);
    }

    private ConcurrentDictionary<string, TaskCompletionSource<byte[]>> _fileRequests = new();

    public async Task<byte[]> Open(IClientConnection connection, string file){
        
        _fileRequests.TryGetValue(file, out var tcs);
        
        if (tcs is null) {
            tcs = new TaskCompletionSource<byte[]>();
            _fileRequests.TryAdd(file, tcs);
        } else {
            if ( tcs.Task.IsCompleted ) return tcs.Task.Result;
        }

        using (var md5 = MD5.Create()) {
            using (var stream = File.OpenRead(AssetPath.Sanitize(file))) {
                var hash = Convert.ToBase64String(md5.ComputeHash(stream));

                _channel.Send(connection, new AssetRepositoryFileRequestPacket() {
                        File = AssetPath.Sanitize(file),
                        Hash = hash
                    }
                );
            }
        }

        return await tcs.Task;
    }

    void OnAssetRepositoryFileResponsePacket(
        IClientConnection connection,
        AssetRepositoryFileResponsePacket packet
    ){
        _fileRequests.TryRemove(packet.File, out var tcs);

        if (packet.Data != null) {
            packet.Write();
            tcs?.SetResult(Convert.FromBase64String(packet.Data));
        } else {
            tcs?.SetResult(File.ReadAllBytes(AssetPath.Sanitize(packet.File)));
        }
    }
}
