/**
 * @author omnisudo
 * @date 2024.08.03
 */

#include "LocalPlayer.hpp"
#include "skillquest/game/base/thing/world/ClientWorld.hpp"

namespace skillquest::game::base::thing::character::player {
    LocalPlayer::LocalPlayer ( const LocalPlayer::CreateInfo& info ) :
            PlayerCharacter{ info.player },
            _connection{ info.connection },
            _world{ info.world } {
    }

    auto LocalPlayer::onActivate () -> void {
        if ( _world ) {
            auto local = std::dynamic_pointer_cast< LocalPlayer >(
                    stuff::Thing::self()
            );

            _world->add_player( local );

            auto clworld = std::dynamic_pointer_cast< world::ClientWorld >( _world );
            if ( clworld )  {
                clworld->localhost( local );
            }
        }
    }
}