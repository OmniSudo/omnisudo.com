/**
 * @author omnisudo
 * @date 2024.02.22
 */

#pragma once

#include "skillquest/graphics.hpp"
#include "skillquest/math.vertex.hpp"

namespace skillquest::graphics {
	class GLMesh : public IMesh {
		unsigned int _vao = 0;
		
		unsigned int _vbo = 0;
		
		unsigned int _ebo = 0;
	
	public:
		GLMesh( const std::vector< math::Vertex > &vertexes, const std::vector< unsigned > &indexes );
		
		~GLMesh() override;
		
		math::Vertex *vertexes( size_t &size ) override;
		
		unsigned int *indexes( size_t &size ) override;
		
		IMesh &vertexes( const std::vector< math::Vertex > &value ) override;
		
		IMesh &vertexes( unsigned int offset, const std::vector< math::Vertex > &value ) override;
		
		IMesh &vertexes(
				std::vector< math::Vertex >::const_iterator begin,
				std::vector< math::Vertex >::const_iterator end
		) override;
		
		IMesh &indexes( const std::vector< unsigned int > &value ) override;
		
		IMesh &indexes( unsigned int offset, const std::vector< unsigned int > &value ) override;
		
		IMesh &indexes(
				std::vector< unsigned int >::const_iterator begin,
				std::vector< unsigned int >::const_iterator end
		) override;

	public:
		void bind() override;
		
		void unbind() override;
		
		void draw() override;
	
	};
}