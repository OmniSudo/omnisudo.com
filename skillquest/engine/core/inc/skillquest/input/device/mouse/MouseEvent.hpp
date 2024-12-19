/**
 * @author omnisudo
 * @date 6/29/24
 */
#pragma once

#include "glm/glm.hpp"
#include "skillquest/event.hpp"

namespace skillquest::input::device::mouse {
    struct MouseButtonPressed : event::IEvent {
        explicit MouseButtonPressed( int button, int mods ) : button( button ), mods( mods ) {}

        int button;
        int mods;
    };

    struct MouseButtonReleased : event::IEvent {
        explicit MouseButtonReleased( int button, int mods ) : button( button ), mods( mods ) {}

        int button;
        int mods;
    };

    struct MouseCursorMoved : event::IEvent {
        explicit MouseCursorMoved( double x, double y ) : x( x ), y( y ) {}

        double x;
        double y;
    };

    struct MouseScrollWheelMoved : event::IEvent {
        explicit MouseScrollWheelMoved( double x, double y ) : x( x ), y( y ) {}

        double x;
        double y;
    };
}// namespace skillquest::input::device::mouse