/**
 * @author omnisudo
 * @date 2024.02.22
 */

#pragma once

#include "skillquest/math.vertex.hpp"

namespace skillquest::graphics {
	class IMesh {
	public:
		virtual ~IMesh() = default;
		
		virtual math::Vertex *vertexes( std::size_t &size ) = 0;
		
		virtual unsigned *indexes( std::size_t &size ) = 0;
		
		virtual IMesh &vertexes( const std::vector< math::Vertex > &value ) = 0;
		
		virtual IMesh &vertexes(
				unsigned int offset,
				const std::vector< math::Vertex > &value
		) = 0;
		
		virtual IMesh &vertexes(
				std::vector< math::Vertex >::const_iterator begin,
				std::vector< math::Vertex >::const_iterator end
		) = 0;
		
		virtual IMesh &indexes(
				const std::vector< unsigned > &value
		) = 0;
		
		virtual IMesh &indexes(
				unsigned int offset,
				const std::vector< unsigned > &value
		) = 0;
		
		virtual IMesh &indexes(
				std::vector< unsigned >::const_iterator begin,
				std::vector< unsigned >::const_iterator end
		) = 0;
	
	public:
		virtual void bind() = 0;
		
		virtual void unbind() = 0;
		
		virtual void draw() = 0;
	};
}