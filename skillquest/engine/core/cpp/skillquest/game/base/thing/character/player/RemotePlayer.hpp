/**
 * @author omnisudo
 * @date 2024.08.01
 */

#pragma once

#include "skillquest/network.hpp"
#include "skillquest/string.hpp"
#include "skillquest/stuff.thing.hpp"
#include "skillquest/property.hpp"
#include "skillquest/character.hpp"

namespace skillquest::game::base::thing::character::player {
	class RemotePlayer : public PlayerCharacter {
	public:
		struct CreateInfo {
			const PlayerCharacter::CreateInfo& player;
		};
		
		RemotePlayer ( const CreateInfo& info ) :
				PlayerCharacter{ info.player } {
		}
		
		~RemotePlayer () override = default;
	
	private:
	};
}