using Silk.NET.Maths;
using SkillQuest.API.Procedural.World;
using SkillQuest.API.Thing.Character;
using SkillQuest.API.Thing.Universe;
using SkillQuest.Shared.Engine.Entity.Character;
using static SkillQuest.Shared.Engine.State;

namespace SkillQuest.Shared.Engine.Entity.Universe;

public class World : IWorld {
    readonly IPlayerCharacter _localhost;

    public World(IPlayerCharacter localhost = null){
        if (localhost is not null) {
            _localhost = localhost;
            Add(localhost);
        }
    }

    public Dictionary<Guid, IPlayerCharacter> Players { get; } = new();
    
    public void Add(IPlayerCharacter player){
        Players[player.CharacterId] = player;
    }

    public Task< IRegion? > Generate(Vector3D<long> position){
        return Generate(position, SH.WorldGenerationPipeline);
    }
    
    public Task< IRegion? > Generate(Vector3D<long> position, IWorldGenPipeline pipeline ){
        return pipeline.Generate(this, position);
    }
}
