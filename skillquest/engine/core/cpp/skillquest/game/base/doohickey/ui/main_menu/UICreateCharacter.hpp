/**
 * @author omnisudo
 * @date 2024.08.03
 */

#pragma once

#include "skillquest/game/base/doohickey/ui/UIDrawable.hpp"
#include "skillquest/stuff.doohickey.hpp"
#include "skillquest/network.hpp"
#include "skillquest/character.hpp"
#include "skillquest/game/base/packet/character/create/CharacterCreatedResponsePacket.hpp"

namespace skillquest::game::base::doohickey::ui::main_menu {
    class UICreateCharacter : public stuff::Doohickey, public UIDrawable {
    public:
        struct CreateInfo {
            const stuff::Doohickey::CreateInfo doohickey;
            network::Connection connection;
        };

        explicit UICreateCharacter(const CreateInfo &info);

        ~UICreateCharacter() override;

        auto onDeactivate() -> void override;

        void draw() override;

    private:
        net_receive( CharacterCreatedResponsePacket, packet::character::create::CharacterCreatedResponsePacket );

    property(channel, network::Channel*&, private_ref, none);

    property(connection, network::Connection, public, none);

    property( name, std::string, public_ref, none );

    private:
        void changeToCharacterSelect ();
    };
}