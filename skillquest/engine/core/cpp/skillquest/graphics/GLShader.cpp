/**
 * @author omnisudo
 * @date 2024.03.09
 */

#include "skillquest/graphics/GLShader.hpp"
#include "glad/glad.h"
#include "skillquest/cl.api.hpp"
#include "skillquest/sh.api.hpp"
#include <future>

namespace skillquest::graphics {
	namespace glsl {
		
		IModule::IModule( std::string source ) : _source{ source } {
		
		}
		
		void IModule::link( int program ) {
			glAttachShader( program, id());
		}
		
		FragmentShader::FragmentShader( const std::string &source ) : IModule( source ) {
			id( glCreateShader( GL_FRAGMENT_SHADER ));
			auto cSource = _source.c_str();
			glShaderSource( id(), 1, &cSource, NULL );
			glCompileShader( id());
			int ok;
			glGetShaderiv( id(), GL_COMPILE_STATUS, &ok );
			if (ok != GL_TRUE ) {
				int maxLength;
				int length;
				glGetShaderiv( id(), GL_INFO_LOG_LENGTH, &maxLength );
				char *log = new char[maxLength];
				glGetShaderInfoLog( id(), maxLength, &length, log );
				glDeleteShader( id());
				auto what = std::string( log, length );
				
				delete[] log;
				glDeleteShader( id());
				id( 0 );
				
				throw std::runtime_error( what );
			}
		}
		
		VertexShader::VertexShader( const std::string &source ) : IModule( source ) {
			id( glCreateShader( GL_VERTEX_SHADER ));
			auto cSource = _source.c_str();
			glShaderSource( id(), 1, &cSource, NULL );
			glCompileShader( id());
			int ok;
			glGetShaderiv( id(), GL_COMPILE_STATUS, &ok );
			if (ok != GL_TRUE) {
				int maxLength;
				int length;
				glGetShaderiv( id(), GL_INFO_LOG_LENGTH, &maxLength );
				char *log = new char[maxLength];
				glGetShaderInfoLog( id(), maxLength, &length, log );
				glDeleteShader( id());
				auto what = std::string( log, length );
				
				delete[] log;
				glDeleteShader( id());
				id( 0 );
				
				throw std::runtime_error( what );
			}
		}
	}
	
	GLShader::GLShader( const std::map< std::string, std::string > &shaders ) {
		auto s = std::move( shaders );
		sq::client()->graphics()->thread().enqueue(
				[ this, s ] () {
					_id = glCreateProgram();
					for (auto pair: s) {
						glsl::IModule *shader;
						if (pair.first == "vertex.glsl") {
							shader = new glsl::VertexShader( pair.second );
						} else if (pair.first == "fragment.glsl") {
							shader = new glsl::FragmentShader( pair.second );
						}
						_shaders.push_back( shader );
					}
					
					for (auto shader: _shaders) {
						shader->link( _id );
					}
					
					glLinkProgram( _id );
					
					int ok = GL_TRUE;
					glGetProgramiv( _id, GL_LINK_STATUS, &ok );
					if ( ok != GL_TRUE ) {
						int maxLength;
						int length = 0;
						glGetProgramiv( _id, GL_INFO_LOG_LENGTH, &maxLength );
						char *log = new char[maxLength];
						glGetProgramInfoLog( _id, maxLength, &length, log );
						auto error = std::string( log, length );
						delete[] log;
						glDeleteProgram( _id );
						throw std::runtime_error( "Failed to link shader:\n" + error );
					}
				},
				"Create Shader Program"
		);
	}
	
	GLShader::~GLShader() {
		auto promise = std::promise< void >();
		auto future = promise.get_future();
		auto del = [ this, &promise ]() {
			for (auto &shader: _shaders) {
				delete shader;
			}
			_shaders.clear();
			glDeleteProgram( _id );
			promise.set_value();
		};
		if ( sq::client()->graphics()->thread().active() ) {
			del();
		} else {
			// If you encounter an error here, you probably did not properly delete an stuff::Thing
			sq::client()->graphics()->thread().enqueue(
					del,
					"Destroy Shader " + std::to_string( _id )
			);
			future.wait();
		}
	}
	
	void GLShader::bind() {
		sq::client()->graphics()->thread().enqueue(
				[ this ]() {
					glUseProgram( _id );
				},
				"Bind Shader " + std::to_string( _id )
		);
	}
	
	void GLShader::unbind() {
		sq::client()->graphics()->thread().enqueue(
				[ this ]() {
					glUseProgram( 0 );
				},
				"Unbind Shader " + std::to_string( _id ));
	}
	
	void GLShader::set( std::string name, glm::vec2 value ) {
		sq::client()->graphics()->thread().enqueue(
				[ this, name, value ]() {
					auto i = uniformLocation( name );
					if (i == -1) return;
					glUniform2fv( i, 1, &value[0] );
				},
				"Set Shader VEC2 Uniform " + std::to_string( _id ));
	}
	
	void GLShader::set( std::string name, glm::vec3 value ) {
		sq::client()->graphics()->thread().enqueue(
				[ this, name, value ]() {
					auto i = uniformLocation( name );
					if (i == -1) return;
					glUniform3fv( i, 1, &value[0] );
				},
				"Set Shader VEC3 Uniform " + std::to_string( _id ));
	}
	
	void GLShader::set( std::string name, glm::vec4 value ) {
		sq::client()->graphics()->thread().enqueue(
				[ this, name, value ]() {
					auto i = uniformLocation( name );
					if (i == -1) return;
					glUniform4fv( i, 1, &value[0] );
				},
				"Set Shader VEC4 Uniform " + std::to_string( _id ));
	}
	
	void GLShader::set( std::string name, glm::mat2 value ) {
		sq::client()->graphics()->thread().enqueue(
				[ this, name, value ]() {
					auto i = uniformLocation( name );
					if (i == -1) return;
					glUniformMatrix2fv( i, 1, GL_FALSE, &value[0][0] );
				},
				"Set Shader MAT2 Uniform " + std::to_string( _id ));
	}
	
	void GLShader::set( std::string name, glm::mat3 value ) {
		sq::client()->graphics()->thread().enqueue(
				[ this, name, value ]() {
					auto i = uniformLocation( name );
					if (i == -1) return;
					glUniformMatrix3fv( i, 1, GL_FALSE, &value[0][0] );
				},
				"Set Shader MAT3 Uniform " + std::to_string( _id ));
	}
	
	void GLShader::set( std::string name, glm::mat4 value ) {
		sq::client()->graphics()->thread().enqueue(
				[ this, name, value ]() {
					auto i = uniformLocation( name );
					if (i == -1) return;
					glUniformMatrix4fv( i, 1, GL_FALSE, &value[0][0] );
				},
				"Set Shader MAT4 Uniform " + std::to_string( _id ));
	}
	
	void GLShader::add( glsl::IModule *shader ) {
		_shaders.push_back( shader );
	}
	
	unsigned int GLShader::position( const std::string &name ) {
		return glGetUniformLocation( _id, name.c_str());
	}
	
	int GLShader::uniformLocation( const std::string &uniform ) {
		return glGetUniformLocation( _id, uniform.c_str());
	}
}