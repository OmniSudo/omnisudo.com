using SkillQuest.API;
using SkillQuest.API.Network;
using SkillQuest.Server.Game.Addons.SkillQuest.Server.Doohickey.Character;
using SkillQuest.Server.Game.Addons.SkillQuest.Server.Doohickey.Users;
using SkillQuest.Shared.Engine.Assets;
using SkillQuest.Shared.Engine.Database;
using SkillQuest.Shared.Game.Addons.SkillQuest.Shared.Doohickey.Addon;
using SkillQuest.Shared.Game.Addons.SkillQuest.Shared.Packet.Character;

namespace SkillQuest.Server.Game.Addons.SkillQuest.Server.Doohickey.Addon;

using static Shared.Engine.State;
using static State;

public class AddonSkillQuestSV : AddonSkillQuestSH{
    public override Uri Uri { get; set; } = new Uri("sv://addon.skill.quest/skillquest");

    public override string Description { get; } = "Base Game Server";

    public AddonSkillQuestSV(){
        Mounted += OnMounted;
        Unmounted += OnUnmounted;
    }

    async void OnMounted(IAddon addon, IApplication? application){
        SV.Database = new SqliteDatabase(new Location(addon, "skillquest.db"));
        SV.Connection = SH.Net.Host(3698);

        Authenticator = SH.Stuff.Add(new Authenticator(SV.Connection));
        Authenticator.LoggedIn += AuthenticatorOnLoggedIn;

        CharacterSelect = SH.Stuff.Add(new CharacterSelect());
        CharacterSelect.CharacterSelected += CharacterSelectOnSelected;

        CharacterCreator = SH.Stuff.Add(new CharacterCreator());
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
