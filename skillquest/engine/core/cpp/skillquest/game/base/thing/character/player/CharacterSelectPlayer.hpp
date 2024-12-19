/**
 * @author omnisudo
 * @date 2024.08.03
 */

#pragma once

#include "skillquest/network.hpp"
#include "skillquest/string.hpp"
#include "skillquest/stuff.thing.hpp"
#include "skillquest/property.hpp"
#include "skillquest/character.hpp"

namespace skillquest::game::base::thing::character::player {
    class CharacterSelectPlayer : public PlayerCharacter {
    public:
        struct CreateInfo {
            const PlayerCharacter::CreateInfo& player;
            network::Connection connection;
        };

        CharacterSelectPlayer ( const CreateInfo& info ) :
                PlayerCharacter{ info.player },
                _connection{ info.connection } {
        }

        ~CharacterSelectPlayer () override = default;

    private:
        property( connection, network::Connection, public, private )
    };
}