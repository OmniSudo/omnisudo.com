/**
 * @author omnisudo
 * @date 2024.02.22
 */

#pragma once

#include <memory>
#include "ITexture.hpp"
#include "IShader.hpp"

namespace skillquest::graphics {
	struct Material {
		std::shared_ptr< ITexture > texture;
		std::shared_ptr< IShader > shader;
		
		void bind () {
			texture->bind();
			shader->bind();
		}
		
		void unbind () {
			texture->unbind();
			shader->unbind();
		}

        bool operator == ( const Material& material ) const {
            return operator ==( material.texture.get() ) && operator ==( material.shader.get() );
        }

        bool operator == ( ITexture* texture ) const {
            return texture == this->texture.get();
        }

        bool operator == ( IShader* shader ) const {
            return shader == this->shader.get();
        }
	};
}