/**
 * @author omnisudo
 * @date 2024.08.03
 */

#include "UICharacterSelect.hpp"
#include "UICreateCharacter.hpp"
#include "imgui.h"
#include "skillquest/game/base/packet/character/select/CharacterSelectInfoRequestPacket.hpp"
#include "skillquest/game/base/packet/character/select/SelectCharacterPacket.hpp"
#include "skillquest/game/base/thing/character/player/CharacterSelectPlayer.hpp"
#include "skillquest/game/base/thing/character/player/LocalPlayer.hpp"
#include "skillquest/game/base/doohickey/ui/ingame/UIInGame.hpp"
#include "skillquest/game/base/thing/world/ClientWorld.hpp"
#include "skillquest/sh.api.hpp"

namespace skillquest::game::base::doohickey::ui::main_menu {
    UICharacterSelect::UICharacterSelect ( const UICharacterSelect::CreateInfo& info ) :
            stuff::Doohickey{
                    info.doohickey
            },
            _channel{
                    sq::shared()->network()
                                ->channels()
                                .create(
                                        "character_select",
                                        true
                                )
            },
            _connection{
                    info.connection
            } {
        // Character data
        sq::shared()->network()->packets().add< packet::character::select::CharacterSelectInfoPacket >();
        channel()->add( this, &UICharacterSelect::onNet_CharacterSelectInfoPacket );

        // A character joins the world on the characterselect channel, only localhost will be joining via that channela
        sq::shared()->network()->packets().add< packet::character::CharacterJoinWorldPacket >();
        channel()->add( this, &UICharacterSelect::onNet_CharacterJoinedWorldPacket );

        // Request all characters
        channel()->send( info.connection, new packet::character::select::CharacterSelectInfoRequestPacket{} );
    }

    UICharacterSelect::~UICharacterSelect () {
    }

    // TODO: Better character selection UI
    void UICharacterSelect::draw () {
        ImGui::Begin( "Character Select" );

        for ( auto character: characters() ) {
            auto selectedPlayer = std::dynamic_pointer_cast< thing::character::player::CharacterSelectPlayer >( character );
            if ( ImGui::Button( character->name().c_str() ) ) {
                channel()->send(
                        selectedPlayer->connection(),
                        new packet::character::select::SelectCharacterPacket{
                                selectedPlayer->name()
                        }
                );
            }
        }

        if ( ImGui::Button( "New Character" ) ) {
            stuff().remove( stuff::Thing::self() );
            characters().clear();
            stuff().create< UICreateCharacter >(
                    {
                            .doohickey = {
                                    .uri = { "ui://skill.quest/create_character" }
                            },
                            .connection = connection(),
                    }
            );
        }

        ImGui::End();
    }

    void UICharacterSelect::onNet_CharacterSelectInfoPacket (
            skillquest::network::Connection connection,
            std::shared_ptr< packet::character::select::CharacterSelectInfoPacket > data
    ) {
        auto chars = std::find_if(
                characters().begin(), characters().end(),
                [ data ] ( const auto& character ) -> bool {
                    return character->name() == data->name();
                }
        );
        if ( chars == characters().end() ) {// No Such Character
            characters().push_back(
                    stuff().create< thing::character::player::CharacterSelectPlayer >(
                            {
                                    .player = {
                                            .character = {
                                                    .thing = {
                                                            .uri = {
                                                                    "character://skill.quest/player/select/" +
                                                                    data->name()
                                                            }
                                                    },
                                                    .uid = data->uid()
                                            },
                                            .name = data->name()
                                    },
                                    .connection = connection
                            }
                    )
            );
        }
    }

    void UICharacterSelect::onNet_CharacterJoinedWorldPacket (
            skillquest::network::Connection connection,
            std::shared_ptr< packet::character::CharacterJoinWorldPacket > data
    ) {
        std::shared_ptr< thing::character::player::LocalPlayer > local = nullptr;

        try {
            auto world = stuff().create< thing::world::ClientWorld >(
                    {
                            .channel = sq::shared()->network()->channels().create( "world." + data->world() ),
                            .name = data->world()
                    }
            );

            local = stuff().create< thing::character::player::LocalPlayer >(
                    {
                            .player = {
                                    .character = {
                                            .thing = {
                                                    .uri = {
                                                            "player://skill.quest/" + data->world() + "/" + data->name()
                                                    }
                                            },
                                            .uid = data->uid(),
                                    },
                                    .name = data->name(),
                            },
                            .connection = connection,
                            .world = world,
                    }
            );
        } catch ( const std::exception& e ) {
            sq::shared()->logger()->error(
                    "Failed to create LocalPlayer {0} in World {1}",
                    data->name(), data->world()
            );

            return;
        }

        if ( !local ) return;

        for ( auto character: characters() ) {
            stuff().remove( character );
        }

        characters().clear();

        stuff().remove( stuff::Thing::self() );

        stuff().create< ingame::UIInGame >(
                {
                        .doohickey = {
                                .uri = { "ui://skill.quest/ingame" }
                        },
                        .local = local,
                }
        );
    }

    void UICharacterSelect::onDeactivate () {
        Thing::onDeactivate();
        channel()->drop( this );
    }
}// namespace skillquest::game::base::doohickey::ui::main_menu