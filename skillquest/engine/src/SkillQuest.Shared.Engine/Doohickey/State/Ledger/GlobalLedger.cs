namespace SkillQuest.Shared.Engine.Doohickey.State.Ledger;

public class GlobalLedger{
    public ItemLedger Items { get; } = new ItemLedger();
    
    public MaterialLedger Materials { get; } = new MaterialLedger();
    
    public ComponentLedger Components { get; } = new ComponentLedger();
}
