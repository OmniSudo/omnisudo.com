/**
 * @author omnisudo
 * @date 2024.02.22
 */

#pragma once

#include "skillquest/graphics.hpp"
#include "glad/glad.h"
#include "GLFW/glfw3.h"

namespace skillquest::graphics {
	class GLWindow : public graphics::IWindow {
	public:
		GLWindow( const CreateInfo &info );
		
		~GLWindow() override;
	
	public:
		auto events() const -> std::shared_ptr< event::EventBus > override;
		
		auto handle() -> void * override;
		
		void update() override;
		
		glm::vec< 2, unsigned int > size() override;
		
		void size( glm::vec< 2, unsigned int > value ) override;
		
		int width() override;
		
		void width( int value ) override;
		
		int height() override;
		
		void height( int value ) override;
		
		bool fullscreen() override;
		
		void fullscreen( bool value ) override;
		
		void grabMouse( bool value ) override;
		
		bool mouseGrabbed() override;
		
		void render() override;
	
	private:
		GLFWwindow* _handle;
	};
}