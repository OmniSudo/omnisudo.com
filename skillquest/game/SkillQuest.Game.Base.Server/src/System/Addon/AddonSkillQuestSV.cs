using System.Collections.Concurrent;
using SkillQuest.API;
using SkillQuest.API.Network;
using SkillQuest.API.Thing;
using SkillQuest.Game.Base.Server.System.Asset;
using SkillQuest.Game.Base.Server.System.Character;
using SkillQuest.Game.Base.Server.System.Users;
using SkillQuest.Game.Base.Shared.Packet.System.Character;
using SkillQuest.Game.Base.Shared.System.Addon;
using SkillQuest.Server.Engine;
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


        application?
            .Mount(new AddonMetallurgySV())
            .Mount(new AddonMiningSV());
    }

    void AuthenticatorOnLoggedIn(IClientConnection connection){
        Console.WriteLine($"{connection.EMail} logged in @ {connection.EndPoint}");
    }

    void OnUnmounted(IAddon addon, IApplication? application){ }

    public Authenticator Authenticator { get; set; }

    void CharacterSelectOnSelected(IClientConnection client, CharacterInfo character){
        Console.WriteLine(client.EMail + ": " + character.Name);

        ItemStack test;

        var inventory = Ledger.Add(new Inventory() {
            Uri = new Uri($"inventory://skill.quest/{character.CharacterId}/main"),
            [new Uri("slot://skill.quest/hand/left")] = new ItemStack(
                Ledger["item://skill.quest/mining/ore/coal"] as IItem,
                1
            ),
            [new Uri("slot://skill.quest/hand/right")] = test = new ItemStack(
                Ledger["item://skill.quest/mining/ore/iron"] as IItem,
                1
            ),
        });

        inventory.CountChanged += (inventory, stack, previous, current) => {
            SH.Assets.Update(stack.Uri, client);
        };
        
        SH.Assets.Update(inventory.Uri, client);

        test.Count++;
    }

    Timer testTimer;

    public CharacterSelect CharacterSelect { get; set; }

    private CharacterCreator CharacterCreator { get; set; }

    void CharacterCreatorOnCreated(IClientConnection client, CharacterInfo character){
        Console.WriteLine(client.EMail + " created character: " + character.Name);
    }
}
