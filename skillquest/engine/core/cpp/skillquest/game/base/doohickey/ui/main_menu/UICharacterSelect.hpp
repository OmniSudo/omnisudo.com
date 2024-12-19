/**
 * @author omnisudo
 * @date 2024.08.03
 */

#pragma once

#include "skillquest/game/base/doohickey/ui/UIDrawable.hpp"
#include "skillquest/stuff.doohickey.hpp"
#include "skillquest/network.hpp"
#include "skillquest/character.hpp"
#include "skillquest/game/base/packet/character/select/CharacterSelectInfoPacket.hpp"
#include "skillquest/game/base/packet/character/CharacterJoinWorldPacket.hpp"


namespace skillquest::game::base::doohickey::ui::main_menu {
    class UICharacterSelect : public stuff::Doohickey, public UIDrawable {
    public:
        struct CreateInfo {
            const stuff::Doohickey::CreateInfo& doohickey;
            network::Connection connection;
        };
    public:
        explicit UICharacterSelect ( const CreateInfo& info );

        ~UICharacterSelect () override;

        auto onDeactivate() -> void override;

        void draw () override;

    private:
        property( characters, std::vector< std::shared_ptr< thing::character::PlayerCharacter > >, protected_ref, none );

        property( channel, network::Channel*&, private_ref, none );

        property( connection, network::Connection, private, none );

        net_receive( CharacterSelectInfoPacket, packet::character::select::CharacterSelectInfoPacket );

        net_receive( CharacterJoinedWorldPacket, packet::character::CharacterJoinWorldPacket );

    };
}