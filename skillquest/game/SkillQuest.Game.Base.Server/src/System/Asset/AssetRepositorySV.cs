using System.Security.Cryptography;
using System.Text;
using System.Xml;
using System.Xml.Serialization;
using SkillQuest.API.Asset;
using SkillQuest.API.ECS;
using SkillQuest.API.Network;
using SkillQuest.Game.Base.Server.Database.Users;
using SkillQuest.Game.Base.Server.System.Addon;
using SkillQuest.Game.Base.Server.System.Asset.Permission;
using SkillQuest.Game.Base.Shared.Packet.System.Asset;
using static SkillQuest.Shared.Engine.State;

namespace SkillQuest.Game.Base.Server.System.Asset;

public class AssetRepositorySV : SkillQuest.Shared.Engine.ECS.System, IAssetRepository{
    readonly IChannel _channel;

    public override Uri? Uri { get; set; } = new Uri("sv://asset.skill.quest/");

    public AssetRepositorySV(){
        _channel = SH.Net.CreateChannel(Uri);

        _channel.Subscribe<AssetRepositoryFileRequestPacket>(OnAssetRepositoryFileRequestPacket);

        Permissions.PermissionCheck += permissions => {
            // TODO: No longer allow all inventories to be viewed
            if (permissions.Uri.Scheme == "inventory" || permissions.Uri.Scheme == "stack") {
                permissions.CanView = true;
                return;
            }

            var rank = (Ledger?[new Uri("sv://addon.skill.quest/skillquest")] as AddonSkillQuestSV)
                ?.Authenticator
                .Rank(permissions.Connection);

            permissions.CanView = true;

            if (rank == Rank.Admin)
                permissions.CanEdit = true;
        };
    }

    public IPermissionChecker? Permissions { get; set; } = new AssetPremissionChecker();

    public async Task<byte[]> Open(string file, IClientConnection? connection = null){
        if (connection is null) {
            return File.ReadAllBytes(AssetPath.Sanitize(file));
        }
        return Array.Empty<byte>(); // TODO: Upload client file to server
    }

    void OnAssetRepositoryFileRequestPacket(IClientConnection connection, AssetRepositoryFileRequestPacket packet){
        try {
            using (var md5 = MD5.Create()) {
                using (var stream = File.OpenRead(AssetPath.Sanitize(packet.File))) {
                    var hash = Convert.ToBase64String(md5.ComputeHash(stream));

                    if (hash != packet.Hash) {
                        // Send file because the hash is wrong
                        _channel.Send(connection, new AssetRepositoryFileResponsePacket() {
                            File = packet.File,
                            Data = Convert.ToBase64String(File.ReadAllBytes(AssetPath.Sanitize(packet.File)))
                        });
                    } else {
                        // Send null because the hash is correct
                        _channel.Send(connection, new AssetRepositoryFileResponsePacket() {
                            File = packet.File,
                            Data = null
                        });
                    }
                }
            }
        } catch (Exception e) {
            _channel.Send(connection, new AssetRepositoryFileResponsePacket() {
                File = packet.File,
                Data = null
            });
        }
    }
}
