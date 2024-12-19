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
	class UISignupWindow : public UICredentialsWindow {
	public:
		explicit UISignupWindow ( const CreateInfo& info );
		
		~UISignupWindow () override = default;
	
	property( confirm, std::string, private_ref, public );
		
		void draw () override;
	
	private:
		void changeToLogin ();
		
	};
}