using SkillQuest.API.Asset;
using SkillQuest.API.Network;

namespace SkillQuest.Game.Base.Server.System.Asset.Permission;

public class AssetPremissionChecker : IPermissionChecker {
    public event IPermissionChecker.DoPermissionCheck? PermissionCheck;

    public void Check(IClientConnection connection, Uri uri, out bool view, out bool edit){
        var perms = new IPermissionChecker.Permissions() { Uri = uri, Connection = connection};
        
        PermissionCheck?.Invoke(perms);
        view = perms.CanView;
        edit = perms.CanEdit;
        
        return;
    }
}
