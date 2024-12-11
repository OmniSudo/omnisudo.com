using SkillQuest.API.Asset;

namespace SkillQuest.Game.Base.Server.System.Asset.Permission;

public class AssetPremissionChecker : IPermissionChecker {
    public event IPermissionChecker.DoPermissionCheck? PermissionCheck;

    public void Check(Uri uri, out bool view, out bool edit){
        var perms = new IPermissionChecker.Permissions() { Uri = uri };
        
        PermissionCheck?.Invoke(perms);
        view = perms.CanView;
        edit = perms.CanEdit;
        
        return;
    }
}
