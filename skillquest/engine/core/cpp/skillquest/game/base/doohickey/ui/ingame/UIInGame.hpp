/**
 * @author omnisudo
 * @date 2024.08.03
 */

#pragma once

#include "../UIDrawable.hpp"
#include "skillquest/stuff.doohickey.hpp"
#include "skillquest/network.hpp"
#include "skillquest/game/base/thing/character/player/LocalPlayer.hpp"
#include "skillquest/input.keyboard.hpp"
#include "skillquest/game/base/thing/world/ClientWorld.hpp"
#include "skillquest/game/base/thing/world/chunk/Chunk.hpp"

namespace skillquest::game::base::doohickey::ui::ingame {
    class UIInGame : public stuff::Doohickey, public UIDrawable {
    public:
        struct CreateInfo {
            const stuff::Doohickey::CreateInfo& doohickey;
            std::shared_ptr< thing::character::player::LocalPlayer > local;
        };
    public:
        explicit UIInGame ( const CreateInfo& info );

        ~UIInGame () override;

        std::shared_ptr< thing::world::ClientWorld > world ();

        void draw () override;

        void onDeactivate () override;

    private:
        property( player, std::shared_ptr< thing::character::player::LocalPlayer >, public, none );

        property( keyPressedEvent, std::shared_ptr< event::EventListener >, protected_ref, none );

        void onKeyPressedEvent( std::shared_ptr< input::device::keyboard::KeyboardKeyPressed > event );

    };
}