namespace SkillQuest.API.Asset;

public interface IPermissionChecker{
    public class Permissions{
        public Uri Uri { get; set; }

        public bool CanView { get; set; } = false;

        public bool CanEdit { get; set; } = false;
    }

    public delegate void DoPermissionCheck(Permissions permissions);
    
    public event DoPermissionCheck PermissionCheck;

    public void Check(Uri uri, out bool view, out bool edit);
}
