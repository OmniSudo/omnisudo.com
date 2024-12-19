/**
 * @author omnisudo
 * @date 6/28/24
 */

#pragma once

#include "skillquest/application/Engine.hpp"

#include <future>
#include "skillquest/network.hpp"
#include "skillquest/graphics/IWindow.hpp"

namespace skillquest::application {
	class ClientEngine : public Engine {
    public:
        ClientEngine ();

        ~ClientEngine() override;

	public:
		auto onStart () -> void override;
		
		auto onStop () -> void override;
		
		auto onUpdate () -> void override;

	};
}