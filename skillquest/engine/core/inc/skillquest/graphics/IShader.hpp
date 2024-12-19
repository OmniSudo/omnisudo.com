/**
 * @author omnisudo
 * @date 2024.02.22
 */

#pragma once

#include <filesystem>
#include <glm/glm.hpp>

namespace skillquest::graphics {
	class IShader {
	public:
		/**
	 	 * DTOR
		 */
		virtual ~IShader() = default;
	
	public:
		virtual std::filesystem::path path() = 0;
		
		/**
		 * Getter
		 * @return Is the program loaded correctly
		 */
		virtual bool valid() = 0;
	
	public:
		/**
		 * Use this shader for rendering
		 */
		virtual void bind() = 0;
		
		/**
		 * stop using this shader for rendering
		 */
		virtual void unbind() = 0;
		
		virtual void set( std::string name, glm::vec2 value ) = 0;
		
		virtual void set( std::string name, glm::vec3 value ) = 0;
		
		virtual void set( std::string name, glm::vec4 value ) = 0;
		
		virtual void set( std::string name, glm::mat2 value ) = 0;
		
		virtual void set( std::string name, glm::mat3 value ) = 0;
		
		virtual void set( std::string name, glm::mat4 value ) = 0;
	
	};
}