/**
* @author omnisudo
* @date 2024.07.29
*/

#pragma once

#include "skillquest/property.hpp"
#include "skillquest/graphics.hpp"
#include "skillquest/sh.api.hpp"
#include <memory>

namespace skillquest {
    namespace application {
        class ClientEngine;
    }

    class ClientAPI {
    private:
        friend class skillquest::application::ClientEngine;

        inline static ClientAPI* _instance = nullptr;

        ClientAPI() {}

    public:
        static auto instance() -> ClientAPI*& {
            return _instance == nullptr ? ( _instance = new ClientAPI() ) : _instance;
        }

        ~ClientAPI() = default;

    private:
    property( graphics, std::shared_ptr< skillquest::graphics::IGraphics >, public, private )

    };
}// namespace skillquest

namespace sq {
    skillquest::ClientAPI*& client();
}