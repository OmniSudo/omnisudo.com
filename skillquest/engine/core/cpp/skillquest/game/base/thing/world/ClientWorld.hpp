/**
 * @author omnisudo
 * @date 2024.08.01
 */

#pragma once

#include "skillquest/glm.hash.hpp"
#include <glm/glm.hpp>
#include "skillquest/game/base/thing/world/World.hpp"
#include "skillquest/network.hpp"
#include "skillquest/game/base/thing/character/player/LocalPlayer.hpp"
#include "skillquest/game/base/packet/character/CharacterJoinWorldPacket.hpp"
#include "skillquest/game/base/packet/character/CharacterLeaveWorldPacket.hpp"
#include "skillquest/game/base/packet/world/chunk/ChunkDataPacket.hpp"

namespace skillquest::game::base::thing::world {
    class ClientWorld : public World {
    public:
        explicit ClientWorld ( const CreateInfo& info );

        ~ClientWorld () override;

        void download ( glm::u16vec3 pos );

        net_receive( CharacterJoinWorldPacket, packet::character::CharacterJoinWorldPacket );

        net_receive( CharacterLeaveWorldPacket, packet::character::CharacterLeaveWorldPacket );

        net_receive( ChunkDataPacket, packet::world::chunk::ChunkDataPacket );

    property( localhost, std::shared_ptr< character::player::LocalPlayer >, public, private );

    private:
        friend class character::player::LocalPlayer;

    };
}