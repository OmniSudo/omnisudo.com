/**
 * @author omnisudo
 * @date 2024.08.01
 */

#pragma once

#include "skillquest/network.hpp"
#include "skillquest/string.hpp"
#include "skillquest/stuff.thing.hpp"
#include "skillquest/property.hpp"
#include "skillquest/character.hpp"
#include "skillquest/game/base/thing/world/World.hpp"

namespace skillquest::game::base::thing::character::player {
    class LocalPlayer : public PlayerCharacter {
    public:
        struct CreateInfo {
            const PlayerCharacter::CreateInfo& player;
            network::Connection connection;
            std::shared_ptr< world::World > world;
        };

        explicit LocalPlayer ( const CreateInfo& info );

        ~LocalPlayer () override = default;

    protected:
        auto onActivate () -> void override;

    property( connection, network::Connection, public, none )

    property( world, std::shared_ptr< world::World >, public, none )
    };
}