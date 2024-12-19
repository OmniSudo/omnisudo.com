/**
 * @author omnisudo
 * @date 2024.02.22
 */

#pragma once

#include "ITexture.hpp"
#include "glad/glad.h"

namespace skillquest::graphics {
	class GLTexture : public ITexture {
	public:
		explicit GLTexture ( const ITexture::CreateInfo& info );
		
		~GLTexture() override;
	
	public:
		void bind() override;
		
		void unbind() override;
		
		auto id() -> unsigned int override;
	
	public:
		auto size() const -> glm::uvec2 override;
		
		auto size( glm::uvec2 value ) -> ITexture& override;
		
		auto pixels() const -> std::vector< glm::u8vec4 > override;
		
		auto pixels( const std::vector< unsigned char > &value ) -> ITexture& override;
		
		auto pixels( const std::vector< glm::u8vec4 > &value ) -> ITexture & override;
		
		auto pixels( glm::uvec2 pos, glm::uvec2 size, const std::vector< glm::u8vec4 > &value ) -> ITexture & override;
		
		auto pixels(
				glm::uvec2 pos, glm::uvec2 size,
				const std::vector< unsigned char > &value
				) -> ITexture & override;
		
		auto fill( glm::uvec2 pos, glm::uvec2 size, glm::u8vec4 color ) -> ITexture & override;
		
		auto replace( glm::u8vec4 source, glm::u8vec4 replacement ) -> ITexture & override;
	
	private:
		unsigned int _id;
		
		glm::uvec2 _size;
	};
}