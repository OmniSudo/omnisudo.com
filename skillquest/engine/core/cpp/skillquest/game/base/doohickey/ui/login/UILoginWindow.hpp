/**
 * @author omnisudo
 * @date 2024.08.01
 */

#pragma once

#include "UICredentialsWindow.hpp"
#include "skillquest/property.hpp"
#include "skillquest/string.hpp"
#include "skillquest/network.hpp"

namespace skillquest::game::base::doohickey::ui::login {
	class UILoginWindow : public UICredentialsWindow {
	public:
		UILoginWindow ( const CreateInfo& info );
		
		~UILoginWindow () override = default;
		
		void draw () override;
	
	private:
		void changeToSignup ();
	};
}