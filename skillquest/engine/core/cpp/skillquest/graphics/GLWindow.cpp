/**
 * @author omnisudo
 * @date 2024.03.07
 */

#include "skillquest/graphics/GLWindow.hpp"
#include "skillquest/sh.api.hpp"
#include "skillquest/cl.api.hpp"
#include "imgui.h"
#include "imgui_impl_glfw.h"
#include "imgui_impl_opengl3.h"

namespace skillquest::graphics {
    void error_callback ( int code, const char* description ) {
        sq::shared()->logger()->error( "GLFW ERROR {0}:\n{1}", code, description );
    }

    GLWindow::GLWindow ( const skillquest::graphics::IWindow::CreateInfo& info ) : IWindow{ info } {
        glfwSetErrorCallback( &error_callback );

        glfwInit();

        glfwWindowHint( GLFW_CONTEXT_VERSION_MAJOR, 3 );
        glfwWindowHint( GLFW_CONTEXT_VERSION_MINOR, 3 );

        _handle = glfwCreateWindow( info.size.x, info.size.y, info.title.c_str(), NULL, NULL );
        if ( !_handle ) {
            sq::shared()->logger()->fatal( "Failed to create GLFW window!" );
            throw std::runtime_error{ "Unknown Error" }; // TODO
        }

        glfwMakeContextCurrent( _handle );

#ifndef PLATFORM_WEB
        gladLoadGL();
#endif

        glfwSetWindowUserPointer( _handle, this );
    }

    GLWindow::~GLWindow () {
        glfwDestroyWindow( _handle );
        _handle = nullptr;
    }

    auto GLWindow::events () const -> std::shared_ptr< event::EventBus > {
        return sq::shared()->events();
    }

    glm::uvec2 GLWindow::size () {
        glm::ivec2 s;
        glfwGetWindowSize( _handle, &s.x, &s.y );
        return s;
    }

    void GLWindow::size ( glm::uvec2 value ) {
        glfwSetWindowSize( _handle, value.x, value.y );
    }

    int GLWindow::width () {
        return size().x;
    }

    void GLWindow::width ( int value ) {
        size( { value, height() } );
    }

    int GLWindow::height () {
        return size().y;
    }

    void GLWindow::height ( int value ) {
        size( { width(), value } );
    }

    bool GLWindow::fullscreen () {
        // TODO
        return false;
    }

    void GLWindow::fullscreen ( bool value ) {

    }

    void GLWindow::grabMouse ( bool value ) {
        glfwSetInputMode( _handle, GLFW_CURSOR, value ? GLFW_CURSOR_HIDDEN : GLFW_CURSOR_NORMAL );
    }

    bool GLWindow::mouseGrabbed () {
        return glfwGetInputMode( _handle, GLFW_CURSOR ) == GLFW_CURSOR_HIDDEN;
    }

    void GLWindow::render () {
        auto cl = sq::client();
        if ( cl && cl->graphics() ) {
            // TODO: Does swap buffers need to be called in the same thread as GL
            cl->graphics()->thread().enqueue(
                    [ this ] () {
                        glfwSwapBuffers( _handle );
                    },
                    "Swap Buffers"
            );
        }
    }

    void* GLWindow::handle () {
        return _handle;
    }

    void GLWindow::update () {
        glfwPollEvents();
    }
}