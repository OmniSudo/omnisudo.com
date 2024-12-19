/**
 * @author omnisudo
 * @date 2024.03.25
 */

#pragma once

#include "skillquest/property.hpp"
#include <chrono>
#include <mutex>

namespace skillquest::graphics {
class Renderer {
	public:
		explicit Renderer();
		
		auto update () -> void;
		
		auto render () -> void;

        property( prev, std::chrono::system_clock::time_point, none, none )

        property( render_mutex, std::mutex, none, none )
	};
}