/**
 * @author omnisudo
 * @date 2024.02.22
 */

#pragma once

#include "skillquest/graphics.hpp"
#include <filesystem>

namespace skillquest::graphics {
	namespace glsl {
		/**
		 * OpenGL shader base
		 * @author  omnisudo
		 * @date    2021.09.01
		 */
		class IModule {
			/**
			 * Shader's ID
			 */
			int _id = 0;
		
		protected:
			std::string _source;
		
		public:
			/**
			 * CTOR
			 * @param source Shader code
			 */
			explicit IModule( std::string source );
		
		public:
			/**
			 * Getter
			 * @return Shader's ID
			 */
			inline int id() const {
				return _id;
			}
			
			/**
			 * Getter
			 * @return Successfully compiled
			 */
			inline bool valid() const {
				return _id != 0;
			}
		
		protected:
			/**
			 * Setter
			 * @param value Shader's ID
			 */
			inline void id( int value ) {
				_id = value;
			}
		
		public:
			/**
			 * Link the shader to the shader program
			 * @param program Shader program ID
			 */
			void link( int program );
		};
		
		/**
		 * OpenGL Fragment shader
		 * @author  omnisudo
		 * @date    9/1/21
		 */
		class FragmentShader : public IModule {
		public:
			explicit FragmentShader( const std::string &source );
			
		};
		
		/**
		 * OpenGL Vertex shader
		 * @author  omnisudo
		 * @date    9/1/21
		 */
		class VertexShader : public IModule {
		public:
			explicit VertexShader( const std::string &source );
			
		};
	}
	
	class GLShader : public IShader {
		std::filesystem::path _path;
		
		/**
		 * Program ID
		 */
		int _id = 0;
		
		/**
		 * List of loaded shaders
		 */
		std::vector< glsl::IModule * > _shaders;
	
	public:
		/**
		 * Create a shader program from in-source shaders.
		 * @param shaders Pointers are owned by GLShader
		 */
		explicit GLShader( const std::map< std::string, std::string > &shaders );
		
		/**
		 * DTOR
		 */
		~GLShader() override;
	
	public:
		inline std::filesystem::path path() override {
			return _path;
		}
		
		/**
		 * getter
		 * @return is the program loaded correctly
		 */
		inline bool valid() override {
			return _id > 0;
		}
	
	public:
		/**
		 * use this shader for rendering
		 */
		void bind() override;
		
		/**
		 * stop using this shader for rendering
		 */
		void unbind() override;
		
		void set( std::string name, glm::vec2 value ) override;
		
		void set( std::string name, glm::vec3 value ) override;
		
		void set( std::string name, glm::vec4 value ) override;
		
		void set( std::string name, glm::mat2 value ) override;
		
		void set( std::string name, glm::mat3 value ) override;
		
		void set( std::string name, glm::mat4 value ) override;
	
	private:
		/**
		 * add a shader
		 */
		void add( glsl::IModule *shader );
		
		unsigned int position( const std::string &name );
		
		int uniformLocation( const std::string &uniform );
	};
}