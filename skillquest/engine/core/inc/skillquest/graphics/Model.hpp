/**
 * @author omnisudo
 * @date 2024.02.22
 */

#pragma once

#include <memory>
#include "Surface.hpp"
#include "skillquest/direction.hpp"

namespace skillquest::graphics {
	struct Model {
        std::map< math::Direction, Surface > surfaces;

        void draw ( math::Direction index = math::Direction::NONE ) {
            if ( index == math::Direction::NONE ) {
                for ( auto surface: surfaces ) {
                    surface.second.bind();
                    surface.second.draw();
                }
            } else {
                for ( math::Direction x : math::DIRECTIONS ) {
                    if ( static_cast<int>( index ) & static_cast< int >( x ) ) {
                        auto surface = surfaces[ x ];
                        surface.bind();
                        surface.draw();
                        surface.unbind();
                    }
                }
            }
        }
	};
}