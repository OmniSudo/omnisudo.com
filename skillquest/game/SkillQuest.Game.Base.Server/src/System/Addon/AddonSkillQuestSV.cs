using SkillQuest.Addon.Base.Server.System.Asset;
using SkillQuest.Addon.Base.Server.System.Character;
using SkillQuest.Addon.Base.Server.System.Users;
using SkillQuest.Addon.Base.Shared.Packet.System.Character;
using SkillQuest.Addon.Base.Shared.System.Addon;
using SkillQuest.API;
using SkillQuest.API.Network;
using SkillQuest.Server.Engine;
using SkillQuest.Shared.Engine.Database;

namespace SkillQuest.Addon.Base.Server.System.Addon;

using static global::SkillQuest.Shared.Engine.State;
using static State;

public class AddonSkillQuestSV : AddonSkillQuestSH{
    public override Uri Uri { get; set; } = new Uri("sv://addon.skill.quest/skillquest");

    public override string Description { get; } = "Base Game Server";

    public AddonSkillQuestSV(){
        Mounted += OnMounted;
        Unmounted += OnUnmounted;
    }

    async void OnMounted(IAddon addon, IApplication? application){
        SV.Database = new SqliteDatabase("addon/base/SkillQuest.Game.Base.Server/assets/database/skillquest.db");
        SV.Connection = SH.Net.Host(3698);

        SH.Assets = SH.Entities.Add(new AssetRepositorySV());
        
        Authenticator = SH.Entities.Add(new Authenticator(SV.Connection));
        Authenticator.LoggedIn += AuthenticatorOnLoggedIn;

        CharacterSelect = SH.Entities.Add(new CharacterSelect());
        CharacterSelect.CharacterSelected += CharacterSelectOnSelected;

        CharacterCreator = SH.Entities.Add(new CharacterCreator());
        CharacterCreator.CharacterCreated += CharacterCreatorOnCreated;
    }

    void AuthenticatorOnLoggedIn(IClientConnection connection){
        Console.WriteLine($"{connection.EMail} logged in @ {connection.EndPoint}");
    }

    void OnUnmounted(IAddon addon, IApplication? application){ }

    public Authenticator Authenticator { get; set; }

    void CharacterSelectOnSelected(IClientConnection client, CharacterInfo character){
        Console.WriteLine(client.EMail + ": " + character.Name);
    }

    public CharacterSelect CharacterSelect { get; set; }

    private CharacterCreator CharacterCreator { get; set; }

    void CharacterCreatorOnCreated(IClientConnection client, CharacterInfo character){
        Console.WriteLine(client.EMail + " created character: " + character.Name);
    }
}
