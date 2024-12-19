/**
 * @author omnisudo
 * @date 2024.08.01
 */

#pragma once

#include "skillquest/stuff.doohickey.hpp"
#include "skillquest/property.hpp"
#include "skillquest/string.hpp"
#include "skillquest/network.hpp"
#include "skillquest/game/base/doohickey/ui/UIDrawable.hpp"
#include "skillquest/game/base/thing/world/World.hpp"
#include <thread>

namespace skillquest::game::base::doohickey::ui::login {
	class UICredentialsWindow : public stuff::Doohickey, public UIDrawable {
	public:
		struct CreateInfo {
			const stuff::Doohickey::CreateInfo& doohickey;
			std::string email;
			std::string password;
			std::string address = "skillquest.omnisudo.com"; // TODO: Replace public IP of omnisudo.com with omnisudo.com
		};
	
	public:
		explicit UICredentialsWindow ( const CreateInfo& info );
		
		~UICredentialsWindow () override = default;
	
	property( email, std::string, public_ref, public );
	property( password, std::string, protected_ref, public );
	property( address, std::string, public_ref, public );
	property( connection, network::Connection, protected, protected );

	protected:
		void connected ();

	};
}