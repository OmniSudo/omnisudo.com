/**
 * @author omnisudo
 * @date 2024.08.04
 */

#include "UICreateCharacter.hpp"
#include "imgui.h"
#include "skillquest/game/base/thing/character/player/CharacterSelectPlayer.hpp"
#include "skillquest/game/base/packet/character/create/CreateCharacterRequestPacket.hpp"
#include "skillquest/sh.api.hpp"
#include "UICharacterSelect.hpp"
#include "imgui_stdlib.h"

namespace skillquest::game::base::doohickey::ui::main_menu {
    UICreateCharacter::UICreateCharacter ( const CreateInfo& info ) :
            stuff::Doohickey( info.doohickey ),
            _connection{ info.connection },
            _channel{ sq::shared()->network()->channels().create( "character_create" ) } {
        sq::shared()->network()->packets().add< packet::character::create::CharacterCreatedResponsePacket >();
        channel()->add( this, &UICreateCharacter::onNet_CharacterCreatedResponsePacket );

    }

    UICreateCharacter::~UICreateCharacter () {
    }

    void UICreateCharacter::draw () {
        ImGui::Begin( "Character Creation" );

        ImGui::InputText( "name", &name() );

        if ( ImGui::Button( "Confirm" ) ) {
            channel()->send(
                    connection(),
                    new packet::character::create::CreateCharacterRequestPacket{
                            name()
                    }
            );
        }

        ImGui::SameLine();

        if ( ImGui::Button( "Cancel" ) ) {
            changeToCharacterSelect();
        }

        ImGui::End();
    }

    void UICreateCharacter::onDeactivate () {
        Thing::onDeactivate();
        channel()->drop( this );
    }

    void UICreateCharacter::onNet_CharacterCreatedResponsePacket (
            skillquest::network::Connection connection,
            std::shared_ptr< packet::character::create::CharacterCreatedResponsePacket > data
    ) {
        if ( data->success() ) {
            changeToCharacterSelect();
            return;
        } else {
            // TODO: Show message box saying why creation failed...
        }
    }

    void UICreateCharacter::changeToCharacterSelect () {
        stuff().remove( stuff::Thing::self() );
        stuff().create< UICharacterSelect >(
                {
                        .doohickey = {
                                .uri = { "ui://skill.quest/character_select" }
                        },
                        .connection = connection(),
                }
        );
    }
}