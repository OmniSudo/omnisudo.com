namespace SkillQuest.Shared.Engine.Entity;

public class Prop : ECS.Entity {
    public string Name { get; protected set; }

    public Guid Id { get; protected set; } = Guid.NewGuid();
}
