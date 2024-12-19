using SkillQuest.Engine.API.Network;

namespace SkillQuest.Engine.API.Asset;

public interface IPermissionChecker{
    public class Permissions{
        public Uri Uri { get; set; }
        
        public IClientConnection Connection { get; set; }

        public bool CanView { get; set; } = false;

        public bool CanEdit { get; set; } = false;
    }

    public delegate void DoPermissionCheck(Permissions permissions);
    
    public event DoPermissionCheck PermissionCheck;

    public void Check(IClientConnection connection, Uri uri, out bool view, out bool edit);
}
