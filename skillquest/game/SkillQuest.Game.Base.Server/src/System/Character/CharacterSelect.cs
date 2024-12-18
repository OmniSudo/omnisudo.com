using SkillQuest.API.Network;
using SkillQuest.API.Thing;
using SkillQuest.API.Thing.Character;
using SkillQuest.Game.Base.Server.Database.Character;
using SkillQuest.Game.Base.Server.Database.Inventory;
using SkillQuest.Game.Base.Shared.Packet.System.Character;
using SkillQuest.Game.Base.Shared.Packet.System.Character.Select;
using SkillQuest.Server.Engine.Component;
using SkillQuest.Shared.Engine.Entity;
using SkillQuest.Shared.Engine.Entity.Character;
using SkillQuest.Shared.Engine.Entity.Universe;

namespace SkillQuest.Game.Base.Server.System.Character;

using static global::SkillQuest.Shared.Engine.State;

public class CharacterSelect : SkillQuest.Shared.Engine.ECS.System{
    public override Uri? Uri { get; set; } = new Uri("sv://control.skill.quest/character/select");

    IChannel _channel { get; }

    CharacterDatabase _database { get; }

    public CharacterSelect(){
        _channel = SH.Net.CreateChannel(Uri);

        _database = CharacterDatabase.Instance;

        _channel.Subscribe<CharacterSelectInfoRequestPacket>(OnCharacterSelectInfoRequestPacket);
        _channel.Subscribe<SelectCharacterRequestPacket>(OnSelectCharacterRequestPacket);
    }

    void OnCharacterSelectInfoRequestPacket(IClientConnection connection, CharacterSelectInfoRequestPacket packet){
        var characters = _database.Characters(connection.Id);

        _channel.Send(connection, new CharacterSelectInfoPacket() { Characters = characters });
    }

    void OnSelectCharacterRequestPacket(IClientConnection connection, SelectCharacterRequestPacket packet){
        var characters = _database.Characters(connection.Id);
        var character = characters?.FirstOrDefault(c => c.CharacterId == packet.Id);

        if (character is not null) {
            Select(connection, character);
        }
    }

    public void Select(IClientConnection connection, CharacterInfo character){
        if (character.CharacterId.Equals(Guid.Empty)) {
            _channel.Send(connection, new SelectCharacterResponsePacket() { Selected = null });
            return;
        }

        Console.WriteLine(
            "User {0} [{1}] selected character {2} [{3}]",
            connection.EMail,
            connection.Id,
            character.Name,
            character.CharacterId
        );

        var worldCharacter = new WorldPlayer() {
            CharacterId = character.CharacterId.GetValueOrDefault(Guid.Empty),
            Inventory = InventoryDatabase.Instance
                .Load(new Uri($"inventory://{character.CharacterId}/main"))
                .Connect(new EntityIsNetworkedComponent()) as IInventory,
            Name = character.Name,
            Uri = character.Uri,
        };

        if (
            !worldCharacter.Inventory!.Stacks?.ContainsKey(
                new Uri($"slot://{character.Name}/main/hand/right")
            ) ?? true
        ) {
            worldCharacter.Inventory![new Uri($"slot://{character.Name}/main/hand/right")] = new ItemStack(
                SH.Ledger["item://skill.quest/mining/tool/pickaxe/iron"] as IItem,
                1,
                Guid.NewGuid(),
                worldCharacter
            );
            InventoryDatabase.Instance.Save(worldCharacter.Inventory!);
        }

        if (worldCharacter.CharacterId == Guid.Empty) {
            _channel.Send(connection, new SelectCharacterResponsePacket() { Selected = null });
            return;
        }

        Ledger.Add(worldCharacter);
        var world = Ledger[character.World] as World;

        world?.Add(worldCharacter);

        CharacterSelected?.Invoke(connection, worldCharacter);
        _channel.Send(connection, new SelectCharacterResponsePacket() { Selected = character });
    }

    public delegate void DoCharacterSelected(IClientConnection client, IPlayerCharacter character);

    public event DoCharacterSelected CharacterSelected;

}
