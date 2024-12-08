using SkillQuest.API;

namespace SkillQuest.Shared.Engine.Assets;

public class Location{
    public Location(IAddon addon, string path){
        string side = "Shared";

        switch (addon.Uri.Scheme) {
            case "cl":
            case "client":
                side = "Client";
                break;
            case "sv":
            case "server":
                side = "Server";
                break;
        }
        this.Path = System.IO.Path.Combine($"Addons/{addon.Name}/{side}/assets/", path );
    }

    public string Path { get; set; }

    public override string ToString(){
        return Path;
    }

    public static implicit operator string(Location location){
        return location.ToString();
    }
}
