using System.Text.Json.Serialization;
using SkillQuest.API.ECS;
using SkillQuest.API.Network;
using SkillQuest.Shared.Engine.ECS;

namespace SkillQuest.Shared.Engine.Component;

public class PermissionProviderComponent : Component< PermissionProviderComponent >{
    public class Permissions{
        public bool CanView {
            get;
            set;
        }
        
        public bool CanEdit { get; set; }
    }
    
    public delegate void DoPermissonCheck( IClientConnection subject, IEntity target, Permissions check );
    
    public event DoPermissonCheck Check;

    public Permissions HasPermission(IClientConnection subject){
        var perms = new Permissions() {
            CanView = false,
            CanEdit = false,
        };
        
        if ( Entity is not null ) Check?.Invoke( subject, Entity, perms );

        return perms;
    }
}
