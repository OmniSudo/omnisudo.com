/**
 * @author omnisudo
 * @date 2024.03.09
 */

#include "skillquest/graphics/GLMesh.hpp"
#include "skillquest/cl.api.hpp"
#include "glad/glad.h"
#include <future>

namespace skillquest::graphics {
	GLMesh::GLMesh( const std::vector< math::Vertex > &vertexes, const std::vector< unsigned int > &indexes ) {
		auto v = std::move( vertexes );
		auto i = std::move( indexes );
		sq::client()->graphics()->thread().enqueue(
				[ this, v, i ]() {
					glGenVertexArrays( 1, &_vao );
					glBindVertexArray( _vao );
					
					glEnable( GL_VERTEX_PROGRAM_POINT_SIZE );
					
					glGenBuffers( 1, &_vbo );
					glBindBuffer( GL_ARRAY_BUFFER, _vbo );
					if (v.size()) {
						glBufferData( GL_ARRAY_BUFFER, sizeof( math::Vertex ) * v.size(), &v[0],
						              GL_DYNAMIC_DRAW );
					}
					
					int stride = sizeof( math::Vertex );
					glEnableVertexAttribArray( 0 );
					glVertexAttribPointer( 0, 3, GL_FLOAT, GL_FALSE, stride,
					                       reinterpret_cast< const void * >(offsetof( math::Vertex,
					                                                                  position )));
					
					glEnableVertexAttribArray( 1 );
					glVertexAttribPointer( 1, 2, GL_FLOAT, GL_FALSE, stride,
					                       reinterpret_cast< const void * >(offsetof( math::Vertex, uv )));
					
					glEnableVertexAttribArray( 2 );
					glVertexAttribPointer( 2, 4, GL_FLOAT, GL_FALSE, stride,
					                       reinterpret_cast< const void * >(offsetof( math::Vertex, color )));
					
					glEnableVertexAttribArray( 3 );
					glVertexAttribPointer( 3, 3, GL_FLOAT, GL_TRUE, stride,
					                       reinterpret_cast< const void * >(offsetof( math::Vertex,
					                                                                  normal )));
					
					glGenBuffers( 1, &_ebo );
					glBindBuffer( GL_ELEMENT_ARRAY_BUFFER, _ebo );
					if (i.size()) {
						glBufferData( GL_ELEMENT_ARRAY_BUFFER, sizeof( unsigned ) * i.size(), i.data(),
						              GL_DYNAMIC_DRAW );
					}
				},
				"Create Mesh"
		);
	}
	
	GLMesh::~GLMesh() {
				auto promise = std::promise< void >();
		auto future = promise.get_future();
		auto del = [ this, &promise ]() {
			glDeleteBuffers( 1, &_ebo );
			glDeleteBuffers( 1, &_vbo );
			glDeleteVertexArrays( 1, &_vao );
			promise.set_value();
		};
		
		if ( sq::client()->graphics()->thread().active() ) {
			del();
		} else {
			// If you encounter an error here, you probably did not properly delete a thing
			sq::client()->graphics()->thread().enqueue(
					del,
					"Delete Mesh " + std::to_string( _vao )
			);
			future.wait();
		}
	}
	
	math::Vertex *GLMesh::vertexes( size_t &size ) {
        auto promise = std::promise< void >();
        auto future = promise.get_future();
        auto del = [ this, &promise ]() {
            glDeleteBuffers( 1, &_ebo );
            glDeleteBuffers( 1, &_vbo );
            glDeleteVertexArrays( 1, &_vao );
            promise.set_value();
        };

        if ( sq::client()->graphics()->thread().active() ) {
            del();
        } else {
            // If you encounter an error here, you probably did not properly delete a thing
            sq::client()->graphics()->thread().enqueue(
                    del,
                    "Delete Mesh " + std::to_string( _vao )
            );
            future.wait();
        }
	}
	
	unsigned int *GLMesh::indexes( size_t &size ) {
		// TODO
		return nullptr;
	}
	
	IMesh &GLMesh::vertexes( const std::vector< math::Vertex > &value ) {
		auto v = std::move( value );
		sq::client()->graphics()->thread().enqueue(
				[ this, v ]() {
					glBindBuffer( GL_ARRAY_BUFFER, _vbo );
					glBufferData( GL_ARRAY_BUFFER, sizeof( math::Vertex ) * v.size(), &v[0],
					              GL_DYNAMIC_DRAW );
				},
				"Upload Vertex Data " + std::to_string( _vao )
		);
		return *this;
	}
	
	IMesh &GLMesh::vertexes( unsigned int offset, const std::vector< math::Vertex > &value ) {
		return vertexes( value.begin() + offset, value.end());
	}
	
	IMesh &GLMesh::vertexes(
			std::vector< math::Vertex >::const_iterator begin,
			std::vector< math::Vertex >::const_iterator end
	) {
		auto v = std::vector< math::Vertex >( begin, end );
		return vertexes( v );
	}
	
	IMesh &GLMesh::indexes( const std::vector< unsigned int > &value ) {
		auto v = std::move( value );
		sq::client()->graphics()->thread().enqueue(
				[ this, v ]() {
					glDeleteBuffers( 1, &_ebo );
					glGenBuffers( 1, &_ebo );
					glBindBuffer( GL_ELEMENT_ARRAY_BUFFER, _ebo );
					glBufferData( GL_ELEMENT_ARRAY_BUFFER, sizeof( unsigned ) * v.size(), v.data(), GL_DYNAMIC_DRAW );
				},
				"Upload Index Data " + std::to_string( _vao )
		);
		return *this;
	}
	
	IMesh &GLMesh::indexes( unsigned int offset, const std::vector< unsigned int > &value ) {
		return indexes( value.begin() + offset, value.end());
	}
	
	IMesh &GLMesh::indexes(
			std::vector< unsigned int >::const_iterator begin,
			std::vector< unsigned int >::const_iterator end
	) {
		auto v = std::vector< unsigned int >{begin, end};
		return indexes( v );
	}
	
	void GLMesh::bind() {
		if (_vao && _vbo) {
			sq::client()->graphics()->thread().enqueue(
					[ this ]() {
						glBindVertexArray( _vao );
						glBindBuffer( GL_ELEMENT_ARRAY_BUFFER, _ebo );
					},
					"Bind Mesh " + std::to_string( _vao )
			);
		}
	}
	
	void GLMesh::unbind() {
		sq::client()->graphics()->thread().enqueue(
				[ this ]() {
					glBindBuffer( GL_ARRAY_BUFFER, 0 );
					glBindBuffer( GL_ELEMENT_ARRAY_BUFFER, 0 );
					glBindVertexArray( 0 );
				},
				"Unbind Mesh " + std::to_string( _vao )
		);
	}
	
	void GLMesh::draw() {
		if (_vao && _vbo) {
			sq::client()->graphics()->thread().enqueue(
                    [ this ]() {
						glBindVertexArray( _vao );
						GLint64 size = 0;
						glGetBufferParameteri64v( GL_ELEMENT_ARRAY_BUFFER, GL_BUFFER_SIZE, &size );
						glDrawElements( GL_TRIANGLES, size, GL_UNSIGNED_INT, 0 );
					},
					"Draw Mesh " + std::to_string( _vao )
			);
		}
	}
}