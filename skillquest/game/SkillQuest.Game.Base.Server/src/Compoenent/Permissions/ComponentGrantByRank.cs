using System.Collections.Concurrent;
using SkillQuest.Game.Base.Server.Database.Users;

namespace SkillQuest.Game.Base.Server.Compoenent.Permissions;

public class ComponentGrantByRank{
    public bool Default = false;

    public void Grant(Rank rank){
        Rank[rank] = true;
    }

    public void Deny(Rank rank){
        Rank[rank] = false;
    }

    ConcurrentDictionary<Rank, bool?> Rank { get; set; }

    public bool? Get(Rank rank){
        return Rank.TryGetValue(rank, out var value) ? value : Default;
    }

    public bool? this[Rank rank] {
        get => Get(rank);
        set {
            switch (value) {
                case true:
                    Grant(rank);
                    break;
                case false:
                    Deny(rank);
                    break;
            }
        }
    }
}
