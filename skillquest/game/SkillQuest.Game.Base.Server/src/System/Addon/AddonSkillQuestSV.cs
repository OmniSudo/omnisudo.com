using SkillQuest.API;
using SkillQuest.API.Component;
using SkillQuest.API.ECS;
using SkillQuest.API.Network;
using SkillQuest.API.Thing;
using SkillQuest.API.Thing.Character;
using SkillQuest.Game.Base.Server.Database.Inventory;
using SkillQuest.Game.Base.Server.Database.ItemStack;
using SkillQuest.Game.Base.Server.System.Asset;
using SkillQuest.Game.Base.Server.System.Character;
using SkillQuest.Game.Base.Server.System.Users;
using SkillQuest.Game.Base.Shared.Packet.System.Character;
using SkillQuest.Game.Base.Shared.System.Addon;
using SkillQuest.Server.Engine;
using SkillQuest.Server.Engine.Component;
using SkillQuest.Shared.Engine.Database;
using SkillQuest.Shared.Engine.Entity;

namespace SkillQuest.Game.Base.Server.System.Addon;

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
        SV.Database = new SqliteDatabase("game/base/SkillQuest.Game.Base.Server/assets/database/skillquest.db");
        SV.Connection = SH.Net.Host(3698);

        SH.Assets = SH.Ledger.Add(new AssetRepositorySV());

        Authenticator = SH.Ledger.Add(new Authenticator(SV.Connection));
        Authenticator.LoggedIn += AuthenticatorOnLoggedIn;

        CharacterSelect = SH.Ledger.Add(new CharacterSelect());
        CharacterSelect.CharacterSelected += CharacterSelectOnSelected;

        CharacterCreator = SH.Ledger.Add(new CharacterCreator());
        CharacterCreator.CharacterCreated += CharacterCreatorOnCreated;

        SH.Ledger.EntityAdded += LedgerOnEntityAdded;

        ItemStackDatabase.Instance.CreateTables();
        
        application?
            .Mount(new AddonMetallurgySV())
            .Mount(new AddonMiningSV());
    }

    void LedgerOnEntityAdded(IEntity ientity){
        if (ientity is ICharacter ) {
            ientity[ typeof( EntityIsNetworkedComponent ) ] = new EntityIsNetworkedComponent();
        }
    }

    void AuthenticatorOnLoggedIn(IClientConnection connection){
        Console.WriteLine($"{connection.EMail} logged in @ {connection.EndPoint}");
    }

    void OnUnmounted(IAddon addon, IApplication? application){ }

    public Authenticator Authenticator { get; set; }

    void CharacterSelectOnSelected(IClientConnection client, IPlayerCharacter character){
    }

    public CharacterSelect CharacterSelect { get; set; }

    private CharacterCreator CharacterCreator { get; set; }

    void CharacterCreatorOnCreated(IClientConnection client, CharacterInfo character){
        Console.WriteLine(client.EMail + " created character: " + character.Name);
    }
}
