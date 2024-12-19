/**
 * @author omnisudo
 * @date 2024.03.25
 */

#include "skillquest/graphics/Renderer.hpp"
#include "skillquest/cl.api.hpp"
#include "skillquest/game/base/doohickey/ui/UIDrawable.hpp"
#include "imgui.h"
#include "imgui_impl_glfw.h"
#include "imgui_impl_opengl3.h"
#include "imgui_stdlib.h"
#include "glad/glad.h"

namespace skillquest::graphics {
    Renderer::Renderer () : _prev( std::chrono::system_clock::now() ) {

    }

    auto Renderer::update () -> void {
    }

    auto Renderer::render () -> void {
        auto lock = std::scoped_lock( _render_mutex );

        auto now = std::chrono::system_clock::now();
        auto delta= now - _prev;

        glClearColor( 0, 0, 0, 1.0 );
        glClear( GL_COLOR_BUFFER_BIT | GL_DEPTH_BUFFER_BIT );

        ImGui_ImplGlfw_NewFrame();
        ImGui_ImplOpenGL3_NewFrame();
        ImGui::NewFrame();

        auto uis = sq::shared()->stuff()->scheme( "ui" );
        for ( auto& [ uri, thing ]: uis ) {
            if ( auto lock = thing.lock() ) {
                auto drawable = std::dynamic_pointer_cast< game::base::doohickey::ui::UIDrawable >( lock );
                if ( drawable ) drawable->draw();
            }
        }

        ImGui::Render();
        ImGui_ImplOpenGL3_RenderDrawData( ImGui::GetDrawData() );

        _prev = now;
    }
}