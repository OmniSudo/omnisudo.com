using System.Collections.Concurrent;
using System.Collections.Immutable;

namespace SkillQuest.Shared.Engine.Thing;

public class Inventory(Uri uri) : Engine.ECS.Thing(uri){
    public ImmutableDictionary< Uri, ItemStack > Stacks => _stacks.ToImmutableDictionary();
    
    private readonly ConcurrentDictionary< Uri, ItemStack > _stacks = new();

    public new ItemStack? this[Uri uri] {
        get => _stacks.GetValueOrDefault(uri);
        set {
            if (value is null) {
                _stacks.TryRemove(uri, out var _);
            } else {
                _stacks[uri] = value;
            }
        }
    }
}
