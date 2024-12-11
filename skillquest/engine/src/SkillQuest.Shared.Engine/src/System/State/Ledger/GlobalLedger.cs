namespace SkillQuest.Shared.Engine.System.State.Ledger;

public class GlobalLedger{
    public ItemLedger Items { get; } = new ItemLedger();
    
    public MaterialLedger Materials { get; } = new MaterialLedger();
    
    public ComponentLedger Components { get; } = new ComponentLedger();
}
