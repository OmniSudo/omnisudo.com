/**
 * @author omnisudo
 * @date 2024.08.31
 */

#pragma once

#include "skillquest/game/base/thing/world/chunk/Chunk.hpp"
#include "skillquest/graphics.hpp"
#include "skillquest/game/base/thing/block/Block.hpp"

namespace skillquest::game::base::thing::world::chunk {
    class ClientChunk : public Chunk {
    public:
        explicit ClientChunk( const CreateInfo& info );

        void rasterize();

        property( surfaces, std::map< block::IBlock* COMMA graphics::Model >, public_ref, none );

    };
}