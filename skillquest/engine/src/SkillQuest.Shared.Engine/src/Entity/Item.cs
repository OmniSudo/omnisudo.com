using SkillQuest.API.Component;
using SkillQuest.API.ECS;
using SkillQuest.API.Network;
using SkillQuest.API.Thing;
using SkillQuest.API.Thing.Character;
using SkillQuest.Shared.Engine.Packet.Component.Interaction;

namespace SkillQuest.Shared.Engine.Entity;

public class Item : Engine.ECS.Entity, IItem{
    string? _name;

    public virtual string Name {
        get => _name ?? Uri?.ToString() ?? "Null";
        protected set => _name = value;
    }

    public virtual string? Description { get; set; }

    #region Primary

    public void OnPrimary(IItemStack stack, ICharacter subject, IEntity target){
        if (stack.Item != this) return;
        Primary?.Invoke(stack, subject, target);
    }

    public event IItem.DoPrimary? Primary;

    #endregion

    #region Secondary

    public void OnSecondary(IItemStack stack, ICharacter subject, IEntity target){
        if (stack.Item != this) return;
        Secondary?.Invoke(stack, subject, target);
    }

    public event IItem.DoSecondary? Secondary;

    #endregion

}
