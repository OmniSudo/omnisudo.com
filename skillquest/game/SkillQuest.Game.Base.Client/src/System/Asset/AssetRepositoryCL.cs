using System.Collections.Concurrent;
using System.Security.Cryptography;
using SkillQuest.API.Asset;
using SkillQuest.API.Network;
using SkillQuest.Game.Base.Shared.Packet.System.Asset;
using static SkillQuest.Shared.Engine.State;

namespace SkillQuest.Game.Base.Client.System.Asset;

public class AssetRepositoryCL : SkillQuest.Shared.Engine.ECS.System, IAssetRepository{
    readonly IChannel _channel;

    public override Uri? Uri { get; set; } = new Uri("cl://asset.skill.quest/");

    public AssetRepositoryCL(){
        _channel = SH.Net.CreateChannel(Uri);

        _channel.Subscribe<AssetRepositoryFileResponsePacket>(OnAssetRepositoryFileResponsePacket);
    }

    private ConcurrentDictionary<string, TaskCompletionSource<byte[]>> _fileRequests = new();

    public IPermissionChecker Permissions { get; set; } = null;

    public async Task<byte[]> Open(string file, IClientConnection connection = null){

        _fileRequests.TryGetValue(file, out var tcs);

        if (tcs is null) {
            tcs = new TaskCompletionSource<byte[]>();
            _fileRequests.TryAdd(file, tcs);
        } else {
            if (tcs.Task.IsCompleted) return tcs.Task.Result;
        }

        FileInfo fi = null;

        try {
            fi = new FileInfo(file);

            if (Uri.TryCreate(file, UriKind.Absolute, out var uri)) {
                if (uri.Scheme != "file") throw new ArgumentException();
            }
        } catch (ArgumentException) {
            fi = null;
        } catch (PathTooLongException) { } catch (NotSupportedException) { }

        if (ReferenceEquals(fi, null)) {
            if (connection is not null) {
                _channel.Send(connection, new AssetRepositoryFileRequestPacket() {
                        File = file,
                        Hash = null
                    }
                );
            } else {
                tcs.SetResult([]);
            }
        } else if (connection is not null) {
            using var md5 = MD5.Create();

            await using var stream = File.OpenRead(AssetPath.Sanitize(file));

            var hash = Convert.ToBase64String(md5.ComputeHash(stream));

            _channel.Send(connection, new AssetRepositoryFileRequestPacket() {
                    File = AssetPath.Sanitize(file),
                    Hash = hash
                }
            );
        } else {
            tcs.SetResult(await File.ReadAllBytesAsync(AssetPath.Sanitize(file)));
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

    public void Update(Uri uri, IClientConnection connection){
        throw new NotImplementedException();
    }

    public void Delete(Uri uri, IClientConnection connection){
        throw new NotImplementedException();
    }
}
