/**
 * @author omnisudo
 * @date 2024.02.22
 */

#pragma once

#include "glm/vec2.hpp"
#include "glm/vec4.hpp"
#include <vector>

namespace skillquest::graphics {
	class ITexture {
	public:
		struct CreateInfo {
			glm::uvec2 size;
			
			std::vector< unsigned char > data;
		};
		
		ITexture  ( const CreateInfo& info ) {}
		
		virtual ~ITexture () = default;
		
	public:
		virtual void bind () = 0;
		
		virtual void unbind () = 0;
		
		virtual auto id () -> unsigned int = 0;
		
	public:
		virtual auto size () const -> glm::uvec2 = 0;
		
		virtual auto size ( glm::uvec2 value ) -> ITexture& = 0;
		
		virtual auto pixels () const -> std::vector< glm::u8vec4 > = 0;
		
		virtual auto pixels ( const std::vector< unsigned char >& value ) -> ITexture& = 0;
		
		virtual auto pixels ( const std::vector< glm::u8vec4 >& value ) -> ITexture& = 0;
		
		virtual auto pixels(
				glm::uvec2 pos, glm::uvec2 size,
				const std::vector< unsigned char > &value
		) -> ITexture & = 0;
		
		virtual auto pixels(
				glm::uvec2 pos, glm::uvec2 size,
				const std::vector< glm::u8vec4 > &value
		) -> ITexture & = 0;
		
		virtual auto fill(
				glm::uvec2 pos, glm::uvec2 size, glm::u8vec4 color
		) -> ITexture & = 0;
		
		virtual auto replace( glm::u8vec4 source, glm::u8vec4 replacement ) -> ITexture& = 0;
	};
}