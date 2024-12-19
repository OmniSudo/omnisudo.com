/**
 * @author omnisudo
 * @date 2024.02.22
 */

#pragma once

#include "skillquest/string.hpp"
#include "glm/vec2.hpp"
#include "skillquest/event.hpp"

namespace skillquest::graphics {
	class IWindow {
	public:
		struct CreateInfo {
			std::string title = "SkillQuest";
			
			glm::uvec2 size = { 1280, 720 };
		};
	
	public:
		IWindow ( const CreateInfo& info ) {}
		
		virtual ~IWindow() = default;
	
	public:
		virtual auto events () const -> std::shared_ptr< event::EventBus > = 0;
		
		virtual glm::vec< 2, unsigned int > size () = 0;
		
		virtual void size ( glm::vec< 2, unsigned int > value ) = 0;
		
		virtual int width () = 0;
		
		virtual void width ( int value ) = 0;
		
		virtual int height () = 0;
		
		virtual void height ( int value ) = 0;
		
		virtual bool fullscreen () = 0;
		
		virtual void fullscreen ( bool value ) = 0;
		
		virtual void grabMouse ( bool value ) = 0;
		
		virtual bool mouseGrabbed () = 0;
		
		virtual auto handle () -> void* = 0;
		
	public:
		virtual void render () = 0;
		
		virtual void update () = 0;
	};
}