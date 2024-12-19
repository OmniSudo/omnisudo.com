/**
 * @author omnisudo
 * @date 2024.09.02
 */

#pragma once

#include <memory>
#include "IMesh.hpp"
#include "Material.hpp"

namespace skillquest::graphics {
    struct Surface {
        std::shared_ptr< IMesh > mesh;
        Material material;

        void bind () {
            mesh->bind();
            material.bind();
        }

        void unbind () {
            mesh->unbind();
            material.unbind();
        }

        void draw () {
            mesh->draw();
        }

        bool operator == ( const Surface& other ) const {
            return operator ==( other.mesh.get() ) && operator ==( other.material );
        }

        bool operator == ( const Material& other ) const {
            return this->material.operator ==( other );
        }

        bool operator == ( IMesh* mesh ) const {
            return mesh == this->mesh.get();
        }

        bool operator == ( ITexture* texture ) const {
            return texture == this->material.texture.get();
        }

        bool operator == ( IShader* shader ) const {
            return shader == this->material.shader.get();
        }
    };
}