/**
 * @author omnisudo
 * @date 6/28/24
 */

#include "skillquest/application/ClientEngine.hpp"
#include "skillquest/platform.hpp"

#include "glad/glad.h"
#include "GLFW/glfw3.h"

#include "imgui.h"
#include "imgui_impl_glfw.h"
#include "imgui_impl_opengl3.h"

#include "skillquest/cl.api.hpp"
#include "skillquest/sh.api.hpp"

#include "skillquest/input/device/keyboard/KeyboardEvent.hpp"
#include "skillquest/input/device/mouse/MouseEvent.hpp"

#include "skillquest/crypto/hash/SHA256.hpp"
#include "skillquest/game/base/doohickey/ui/login/UILoginWindow.hpp"
#include "skillquest/graphics/GLWindow.hpp"
#include "skillquest/graphics/GLGraphics.hpp"
#include "skillquest/cl.api.hpp"

namespace skillquest::application {
    ClientEngine::ClientEngine () {

    }

    ClientEngine::~ClientEngine () {

    }

    void ClientEngine::onStart () {
        auto sh = sq::shared();
        Engine::onStart();

        if ( !glfwInit() ) {
            sh->logger()->error( "Failed to init graphics backend." );
            throw std::runtime_error( "Graphics Init Failed" );
        } else {
            sh->logger()->trace( "Graphics backend successfully initialized" );
        }

        sh->logger()->trace( "Creating graphics capabilities" );
        sq::client()->graphics(
                std::make_shared< graphics::GLGraphics >(
                        graphics::GLGraphics::CreateInfo{
                                .title = "SkillQuest",
                                .size = { 1280, 720 }
                        }
                )
        );

        auto w = reinterpret_cast< GLFWwindow* >( sq::client()->graphics()->window().handle() );

        glfwSetWindowUserPointer( w, this );

        glfwSetCharCallback(
                w, [] ( GLFWwindow* window, unsigned int c ) {
                    sq::shared()->events()->post( new input::device::keyboard::KeyboardCharTyped{ c } );
                }
        );

        glfwSetKeyCallback(
                w, [] ( GLFWwindow* window, int key, int scancode, int action, int mods ) {
                    if ( action == GLFW_PRESS ) {
                        sq::shared()->events()->post(
                                new input::device::keyboard::KeyboardKeyPressed{
                                        key,
                                        scancode,
                                        mods
                                }
                        );
                    } else if ( action == GLFW_RELEASE ) {
                        sq::shared()->events()->post(
                                new input::device::keyboard::KeyboardKeyReleased{
                                        key,
                                        scancode,
                                        mods
                                }
                        );
                    }
                }
        );

        glfwSetCursorPosCallback(
                w, [] ( GLFWwindow* window, double x, double y ) {
                    sq::shared()->events()->post( new input::device::mouse::MouseCursorMoved{ x, 0 } );
                }
        );

        glfwSetMouseButtonCallback(
                w, [] ( GLFWwindow* window, int button, int action, int mods ) {
                    if ( action == GLFW_PRESS ) {
                        sq::shared()->events()->post( new input::device::mouse::MouseButtonPressed{ button, mods } );
                    } else if ( action == GLFW_RELEASE ) {
                        sq::shared()->events()->post( new input::device::mouse::MouseButtonReleased{ button, mods } );
                    }
                }
        );

        glfwSetScrollCallback(
                w, [] ( GLFWwindow* window, double x, double y ) {
                    sq::shared()->events()->post( new input::device::mouse::MouseScrollWheelMoved{ x, y } );
                }
        );

        glfwSetWindowCloseCallback(
                w, [] ( GLFWwindow* window ) {
                    auto engine = static_cast< ClientEngine* >( glfwGetWindowUserPointer( window ) );
                    engine->quit();
                }
        );

        sq::client()->graphics()->thread().enqueue(
                [ this ] () {
                    sq::shared()->logger()->trace( "Creating ImGui context" );
                    ImGui::CreateContext();

                    ImGui_ImplGlfw_InitForOpenGL( reinterpret_cast< GLFWwindow* >( sq::client()->graphics()->window().handle() ), true );
                    ImGui_ImplOpenGL3_Init();
                },
                "Init ImGui"
        );

#ifdef PLATFORM_WEB
        ImGui_ImplGlfw_InstallEmscriptenCallbacks( w, "#canvas" );
#endif
        sq::shared()->stuff()->create< game::base::doohickey::ui::login::UILoginWindow >(
                {
                        .doohickey = {
                                .uri = { "ui://skill.quest/window/login" }
                        },
                        .address = "skillquest.omnisudo.com"
                }
        );
    }

    auto ClientEngine::onUpdate () -> void {
        Engine::onUpdate();

        sq::client()->graphics()->update();
        sq::client()->graphics()->render();
    }

    void ClientEngine::onStop () {
        auto cl = sq::client();
        cl->_graphics = nullptr;

        Engine::onStop();
    }
}// namespace skillquest::application