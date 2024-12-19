using System.Collections.Concurrent;
using Silk.NET.Maths;
using SkillQuest.Engine.API.Procedural.World;
using SkillQuest.Engine.API.Thing.Character;
using SkillQuest.Engine.API.Thing.Universe;
using static SkillQuest.Engine.Core.State;

namespace SkillQuest.Engine.Core.Entity.Universe;

public class World : IWorld {
    readonly IPlayerCharacter _localhost;

    public World(IPlayerCharacter localhost = null){
        if (localhost is not null) {
            _localhost = localhost;
            Add(localhost);
        }
    }

    public ConcurrentDictionary<Guid, IPlayerCharacter> Players { get; } = new();

    public ConcurrentDictionary<Uri, Prop> Props { get; } = new();
    
    public IPlayerCharacter Add(IPlayerCharacter player){
        Players[player.CharacterId] = player;
        return player;
    }

    public Prop Add(Prop prop){
        Props[prop.Uri] = prop;
        return prop;
    }

    public Task< IRegion? > Generate(Vector3D<long> position){
        return Generate(position, SH.WorldGenerationPipeline);
    }
    
    public Task< IRegion? > Generate(Vector3D<long> position, IWorldGenPipeline pipeline ){
        return pipeline.Generate(this, position);
    }
}
