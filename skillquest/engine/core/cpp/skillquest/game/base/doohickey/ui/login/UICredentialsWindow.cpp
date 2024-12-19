/**
 * @author omnisudo
 * @date 2024.08.01
 */

#include "UICredentialsWindow.hpp"
#include "skillquest/game/base/thing/world/ClientWorld.hpp"
#include "skillquest/game/base/thing/character/player/LocalPlayer.hpp"
#include "skillquest/sh.api.hpp"
#include "../main_menu/UICharacterSelect.hpp"

namespace skillquest::game::base::doohickey::ui::login {
    void UICredentialsWindow::connected () {
		stuff().remove( stuff::Thing::self() );
        stuff().create< main_menu::UICharacterSelect >(
                {
                        .doohickey = {
                                .uri = { "ui://skill.quest/menu/character_select" }
                        },
                        .connection = connection()
                }
        );
	}
	
	UICredentialsWindow::UICredentialsWindow ( const UICredentialsWindow::CreateInfo& info ) :
			stuff::Doohickey{ info.doohickey },
			UIDrawable(),
			_email( info.email ),
			_password( info.password ),
			_address( info.address ) {
	}
}