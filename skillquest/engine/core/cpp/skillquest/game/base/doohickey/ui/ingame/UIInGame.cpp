/**
 * @author omnisudo
 * @date 2024.08.03
 */

#include "UIInGame.hpp"
#include "GLFW/glfw3.h"
#include "imgui.h"
#include "skillquest/game/base/doohickey/item/ClientItemNetworking.hpp"
#include "skillquest/game/base/doohickey/item/ClientItemStackNetworking.hpp"
#include "skillquest/sh.api.hpp"
#include "skillquest/game/base/doohickey/block/ClientBlockNetworking.hpp"

namespace skillquest::game::base::doohickey::ui::ingame {
    UIInGame::UIInGame(const UIInGame::CreateInfo &info)
        : stuff::Doohickey{info.doohickey},
          _player{info.local} {
        stuff().create<item::ClientItemNetworking>({
            .localplayer = player(),
        });

        stuff().create<item::ClientItemStackNetworking>({
            .localplayer = player(),
        });

        stuff().create<block::ClientBlockNetworking>({
            .localplayer = player()
        });

        world()->download({0, 0, 0});
    }

    UIInGame::~UIInGame() {
    }

    void UIInGame::draw() {
        auto io = ImGui::GetIO();
        auto size = io.DisplaySize;

        ImGui::SetNextWindowSize(size);
        ImGui::Begin(
            "#INGAME", nullptr,
            ImGuiWindowFlags_NoResize | ImGuiWindowFlags_NoCollapse |
            ImGuiWindowFlags_NoBackground | ImGuiWindowFlags_NoMove |
            ImGuiWindowFlags_NoScrollbar | ImGuiWindowFlags_NoTitleBar);
        ImGui::SetWindowPos({0, 0});

        // TODO: Draw game

        ImGui::End();
    }

    void UIInGame::onDeactivate() {
        stuff::Doohickey::onDeactivate();
        sq::shared()->events()->drop(this);
    }

    std::shared_ptr<thing::world::ClientWorld> UIInGame::world() {
        return std::dynamic_pointer_cast<thing::world::ClientWorld>(player()->world());
    }

    void UIInGame::onKeyPressedEvent(std::shared_ptr<input::device::keyboard::KeyboardKeyPressed> event) {
    }
} // namespace skillquest::game::base::doohickey::ui::ingame
