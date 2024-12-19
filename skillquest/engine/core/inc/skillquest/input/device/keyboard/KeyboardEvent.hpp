/**
 * @author omnisudo
 * @date 6/29/24
 */
#pragma once

#include "skillquest/event.hpp"
#include "glm/glm.hpp"

namespace skillquest::input::device::keyboard {
	struct KeyboardKeyPressed : event::IEvent {
		KeyboardKeyPressed ( int key, int scancode, int mods ) :
				key( key ), scancode( scancode ), mods( mods ) {
			
		}
		
		int key;
		int scancode;
		int mods;
	};
	
	struct KeyboardKeyReleased : event::IEvent {
		KeyboardKeyReleased ( int key, int scancode, int mods ) :
				key( key ), scancode( scancode ), mods( mods ) {
			
		}
		
		int key;
		int scancode;
		int mods;
	};
	
	struct KeyboardCharTyped : event::IEvent {
		explicit KeyboardCharTyped ( unsigned int value ) : value( value ) {}
		
		unsigned int value;
	};
}