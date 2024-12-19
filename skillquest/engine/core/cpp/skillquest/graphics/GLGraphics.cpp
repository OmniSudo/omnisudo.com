/**
 * @author omnisudo
 * @date 2024.02.22
 */

#include <cstring>
#include <png.h>
#include <fstream>
#include "skillquest/graphics/GLGraphics.hpp"
#include "skillquest/graphics/GLWindow.hpp"

#include "skillquest/graphics/GLTexture.hpp"
#include "skillquest/graphics/GLMesh.hpp"
#include "skillquest/graphics/GLShader.hpp"

#include "skillquest/sh.api.hpp"
#include "skillquest/hash.hpp"
#include "imgui.h"
#include "imgui_impl_glfw.h"
#include "imgui_impl_opengl3.h"

namespace skillquest::graphics {
    GLGraphics::GLGraphics ( const IGraphics::CreateInfo& info ) {
        _threads = std::make_shared< threadpool::ThreadPool >();
        _threads->start( 1 );

        _textures = std::make_shared< GLTextureFactory >();
        _meshes = std::make_shared< GLMeshFactory >();
        _shaders = std::make_shared< GLShaderFactory >();

        auto i = std::move( info );
        _threads->enqueue(
                [ this, i ] () {
                    _window = std::make_shared< graphics::GLWindow >(
                            graphics::GLWindow::CreateInfo{
                                    .title = i.title,
                                    .size = i.size,
                            }
                    );
                },
                "Create OpenGL Window"
        );

        _renderer = std::make_shared< Renderer >();

        _threads->wait();
    }

    GLGraphics::~GLGraphics () {
        _meshes = nullptr;
        _shaders = nullptr;
        _textures = nullptr;

        _threads->enqueue(
                [ this ] () {
                    ImGui_ImplOpenGL3_Shutdown();
                    ImGui_ImplGlfw_Shutdown();
                    ImGui::DestroyContext();

                    _window = nullptr;
                },
                "Delete OpenGL Window"
        );
        _threads->wait();
    }

    IWindow& GLGraphics::window () const {
        return *_window;
    }

    auto GLGraphics::thread () -> threadpool::ThreadPool& {
        return *_threads;
    }

    IGraphics::AssetFactory< ITexture >& GLGraphics::textures () {
        return *_textures;
    }

    IGraphics::AssetFactory< IMesh >& GLGraphics::meshes () {
        return *_meshes;
    }

    IGraphics::AssetFactory< IShader >& GLGraphics::shaders () {
        return *_shaders;
    }

    void GLGraphics::update () {
        thread().wait();

        thread().enqueue(
                [ this ] () {
                    window().update();

                    _renderer->update();
                },
                "Update Renderer"
        );
    }

    void GLGraphics::render () {
        thread().enqueue(
                [ this ] () {
                    _renderer->render();

                    window().render();
                },
                "Renderer Rendering"
        );

    }

    std::shared_ptr< ITexture > GLGraphics::GLTextureFactory::get ( const std::filesystem::path& path ) {
        if ( !exists( path ) ) throw std::runtime_error{ "No such image file " + path.string() };

        FILE* f;
        int is_png, bit_depth, color_type, row_bytes, i;
        png_infop info_ptr, end_info;
        png_uint_32 t_width, t_height;
        png_byte header[8], * image_data;
        png_bytepp row_pointers;
        png_structp png_ptr;
        GLuint texture;
        int alpha;

        if ( !( f = fopen( path.string().c_str(), "r" ) ) ) {
            return nullptr;
        }
        fread( header, 1, 8, f );
        is_png = !png_sig_cmp( header, 0, 8 );
        if ( !is_png ) {
            fclose( f );
            return nullptr;
        }
        png_ptr = png_create_read_struct(
                PNG_LIBPNG_VER_STRING, nullptr,
                nullptr, nullptr
        );
        if ( !png_ptr ) {
            fclose( f );
            return nullptr;
        }
        info_ptr = png_create_info_struct( png_ptr );
        if ( !info_ptr ) {
            png_destroy_read_struct(
                    &png_ptr, ( png_infopp ) nullptr,
                    ( png_infopp ) nullptr
            );
            fclose( f );
            return nullptr;
        }
        end_info = png_create_info_struct( png_ptr );
        if ( !end_info ) {
            png_destroy_read_struct(
                    &png_ptr, ( png_infopp ) nullptr,
                    ( png_infopp ) nullptr
            );
            fclose( f );
            return nullptr;
        }
        if ( setjmp( png_jmpbuf( png_ptr ) ) ) {
            png_destroy_read_struct( &png_ptr, &info_ptr, &end_info );
            fclose( f );
            return nullptr;
        }
        png_init_io( png_ptr, f );
        png_set_sig_bytes( png_ptr, 8 );
        png_read_info( png_ptr, info_ptr );
        png_get_IHDR(
                png_ptr, info_ptr, &t_width, &t_height, &bit_depth,
                &color_type, nullptr, nullptr, nullptr
        );

        png_read_update_info( png_ptr, info_ptr );
        row_bytes = png_get_rowbytes( png_ptr, info_ptr );
        image_data = ( png_bytep ) malloc( row_bytes * t_height * sizeof( png_byte ) );
        if ( !image_data ) {
            png_destroy_read_struct( &png_ptr, &info_ptr, &end_info );
            fclose( f );
            return nullptr;
        }
        row_pointers = ( png_bytepp ) malloc( t_height * sizeof( png_bytep ) );
        if ( !row_pointers ) {
            png_destroy_read_struct( &png_ptr, &info_ptr, &end_info );
            free( image_data );
            fclose( f );
            return NULL;
        }
        for ( i = 0; i < t_height; ++i ) {
            row_pointers[ t_height - 1 - i ] = image_data + i * row_bytes;
        }
        png_read_image( png_ptr, row_pointers );
        switch ( png_get_color_type( png_ptr, info_ptr ) ) {
            case PNG_COLOR_TYPE_RGBA:
                alpha = GL_RGBA;
                break;
            case PNG_COLOR_TYPE_RGB:
                alpha = GL_RGB;
                break;
            default:
                printf(
                        "Color type %d not supported!\n",
                        png_get_color_type( png_ptr, info_ptr )
                );
                png_destroy_read_struct( &png_ptr, &info_ptr, &end_info );
                return NULL;
        }

        std::vector< unsigned char > pixels{ image_data, image_data + ( t_width * t_height * ( alpha ? 4 : 3 ) ) };

        if ( !alpha ) {
            for ( auto i = 0; i < pixels.size(); i += 4 ) {
                pixels.insert( pixels.begin() + i + 3, 1, 255 );
            }
        }

        std::shared_ptr< graphics::ITexture > ptr;
        try {
            ptr = std::shared_ptr< graphics::ITexture >{
                    new graphics::GLTexture(
                            {
                                    .size = { t_width, t_height },
                                    .data = pixels,
                            }
                    )
            };
        } catch ( const std::exception& e ) {
            sq::shared()->logger()->error( "Failed to load Image {0}", path.string() );
        }

        return ptr;
    }

    std::shared_ptr< ITexture > GLGraphics::GLTextureFactory::empty () {
        return std::shared_ptr< ITexture >{
                new GLTexture{
                        {
                                .size = {},
                                .data = {}
                        }
                }
        };
    }

    std::shared_ptr< IMesh > GLGraphics::GLMeshFactory::get ( const std::filesystem::path& path ) {
        if ( !exists( path ) ) throw std::runtime_error{ "No such obj file " + path.string() };

        auto stream = std::ifstream( path.c_str() );

        std::vector< glm::vec3 > positions;
        std::vector< glm::vec2 > uvs;
        std::vector< glm::vec3 > normals;

        auto hash = util::hash< glm::vec3, glm::vec2, glm::vec4, glm::vec3 >();

        std::vector< math::Vertex > vertexes;
        std::vector< unsigned > indexes;

        std::string line;
        long lineNo = 0;
        while ( std::getline( stream, line ) ) {
            lineNo++;
            if ( line.empty() ) {
                continue;
            }

            if ( line.starts_with( '#' ) || line.starts_with( '\r' ) ) {
                continue;
            }

            std::string type = "";
            std::vector< std::string > split;
            std::size_t pos = 0;
            std::string token;

            pos = line.find( " " );
            if ( pos == std::string::npos ) {
                throw std::runtime_error( "Improper spacing: " + path.string() + " " + std::to_string( lineNo ) );
            }

            type = line.substr( 0, pos );
            line.erase( 0, pos + 1 );

            while ( ( pos = line.find( " " ) ) != std::string::npos ) {
                token = line.substr( 0, pos );
                line.erase( 0, pos + 1 );
                split.push_back( token );
            }
            split.push_back( line );

            if ( type == "v" ) {
                if ( split.size() < 3 ) {
                    throw std::runtime_error( "Improper spacing: " + path.string() + " " + std::to_string( lineNo ) );
                }
                positions.emplace_back(
                        std::stof( split[ 0 ] ),
                        std::stof( split[ 1 ] ),
                        std::stof( split[ 2 ] )

                );
            } else if ( type == "vt" ) {
                if ( split.size() < 2 ) {
                    throw std::runtime_error( "Improper spacing: " + path.string() + " " + std::to_string( lineNo ) );
                }
                uvs.emplace_back(
                        std::stof( split[ 0 ] ),
                        std::stof( split[ 1 ] )

                );
            } else if ( type == "vn" ) {
                if ( split.size() < 3 ) {
                    throw std::runtime_error( "Improper spacing: " + path.string() + " " + std::to_string( lineNo ) );
                }
                normals.emplace_back(
                        glm::normalize(
                                glm::vec3{
                                        std::stof( split[ 0 ] ),
                                        std::stof( split[ 1 ] ),
                                        std::stof( split[ 2 ] )
                                }
                        )
                );
            } else if ( type == "f" ) {
                if ( split.size() < 3 ) {
                    throw std::runtime_error( "Improper spacing: " + path.string() + " " + std::to_string( lineNo ) );
                }
                for ( auto i = 0; i < 3; i++ ) {
                    std::vector< std::string > face;
                    while ( ( pos = split[ i ].find( "/" ) ) != std::string::npos ) {
                        token = split[ i ].substr( 0, pos );
                        split[ i ].erase( 0, pos + 1 );
                        face.push_back( token );
                    }
                    face.push_back( split[ i ] );
                    if ( face.size() < 1 ) {
                        throw std::runtime_error(
                                "Improper spacing: " + path.string() + " " + std::to_string( lineNo )
                        );
                    }
                    math::Vertex vertex{ .position = positions[ std::stoi( face[ 0 ] ) - 1 ] };
                    if ( face.size() >= 2 && !face[ 1 ].empty() ) {
                        vertex.uv = uvs[ std::stoi( face[ 1 ] ) - 1 ];
                    }
                    if ( face.size() >= 3 && !face[ 2 ].empty() ) {
                        vertex.normal = normals[ std::stoi( face[ 2 ] ) - 1 ];
                    }
                    auto vhash = hash( vertex.position, vertex.uv, vertex.color, vertex.normal );
                    auto it = std::find_if(
                            vertexes.begin(), vertexes.end(),
                            [ vhash, &hash ] ( const auto& v ) -> bool {
                                return vhash == hash( v.position, v.uv, v.color, v.normal );
                            }
                    );
                    if ( it == vertexes.end() ) {
                        vertexes.emplace_back( vertex );
                        indexes.emplace_back( vertexes.size() - 1 );
                    } else {
                        indexes.emplace_back( it - vertexes.begin() );
                    }
                }
            }
        }

        return std::shared_ptr< IMesh >{ new GLMesh( vertexes, indexes ) };
    }

    std::shared_ptr< IMesh > GLGraphics::GLMeshFactory::empty () {
        return std::shared_ptr< IMesh >{ new GLMesh( {}, {} ) };
    }

    std::shared_ptr< IShader > GLGraphics::GLShaderFactory::get ( const std::filesystem::path& path ) {
        if ( !exists( path ) ) throw std::runtime_error{ "No such shader directory " + path.string() };

        std::map< std::string, std::string > shaders;
        try {
            std::vector< std::string > files = { "vertex.glsl", "fragment.glsl" };
            for ( auto file: files ) {
                auto filepath = path / file;
                if ( !std::filesystem::exists( filepath ) ) continue;
                auto istream = std::fstream( filepath );
                std::string source(
                        ( std::istreambuf_iterator< char >( istream ) ), std::istreambuf_iterator< char >()
                );
                shaders.emplace( file, source );
            }

            return std::shared_ptr< IShader >( new graphics::GLShader( shaders ) );
        } catch ( std::exception& e ) {
            sq::shared()->logger()->error( "Failed to load shader {0}: {1}", path.string(), e.what() );
            return nullptr;
        }
    }

    std::shared_ptr< IShader > GLGraphics::GLShaderFactory::empty () {
        return std::shared_ptr< IShader >( nullptr );
    }
}