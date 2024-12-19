/**
 * @author omnisudo
 * @date 2024.03.08
 */

#include "skillquest/graphics/GLTexture.hpp"
#include "skillquest/cl.api.hpp"

#include "skillquest/graphics.hpp"
#include <future>

namespace skillquest::graphics {
    GLTexture::GLTexture ( const ITexture::CreateInfo& info ) : ITexture( info ), _size( info.size ) {
        auto i = std::move( info );
        sq::client()->graphics()->thread().enqueue(
                [ this, i ] () {
                    glGenTextures( 1, &_id );
                    glBindTexture( GL_TEXTURE_2D, _id );

                    glTexImage2D(
                            GL_TEXTURE_2D,
                            0, GL_RGBA8, i.size.x, i.size.y,
                            0, GL_RGBA, GL_UNSIGNED_BYTE,
                            i.data.data()
                    );

                    glTexParameterf( GL_TEXTURE_2D, GL_TEXTURE_MAG_FILTER, GL_NEAREST );
                    glTexParameterf( GL_TEXTURE_2D, GL_TEXTURE_MIN_FILTER, GL_NEAREST );

                    glBindTexture( GL_TEXTURE_2D, 0 ); // TODO: Load faster without, no wrong textures with
                },
                "Create Texture"
        );
    }

    GLTexture::~GLTexture () {
        auto promise = std::promise< void >();
        auto future = promise.get_future();
        auto del = [ this, &promise ] () {
            glDeleteTextures( 1, &_id );
            promise.set_value();
        };

        if ( sq::client()->graphics()->thread().active() ) {
            del();
        } else {
            // If you encounter an error here, you probably did not properly delete a thing
            sq::client()->graphics()->thread().enqueue(
                    del,
                    "Delete Texture " + std::to_string( _id )
            );
            future.wait();
        }
    }

    void GLTexture::bind () {
        sq::client()->graphics()->thread().enqueue(
                [ this ] () {
                    glBindTexture( GL_TEXTURE_2D, _id );
                },
                "Bind Texture " + std::to_string( _id )
        );
    }

    void GLTexture::unbind () {
        sq::client()->graphics()->thread().enqueue(
                [ this ] () {
                    glBindTexture( GL_TEXTURE_2D, 0 );
                },
                "Unbind Texture " + std::to_string( _id )
        );
    }

    auto GLTexture::pixels ( const std::vector< unsigned char >& value ) -> ITexture& {
        auto v = std::move( value );
        sq::client()->graphics()->thread().enqueue(
                [ this, v ] () {
                    if ( _id ) glDeleteTextures( 1, &_id );

                    glGenTextures( 1, &_id );
                    glBindTexture( GL_TEXTURE_2D, _id );

                    auto count = 4 * _size.x * _size.y;
                    std::vector< unsigned char > data{ v.begin(), v.end() };
                    data.resize( count );

                    glTexImage2D(
                            GL_TEXTURE_2D,
                            0, GL_RGBA8, _size.x, _size.y,
                            0, GL_RGBA, GL_UNSIGNED_BYTE,
                            data.data()
                    );

                    glTexParameterf( GL_TEXTURE_2D, GL_TEXTURE_MAG_FILTER, GL_NEAREST );
                    glTexParameterf( GL_TEXTURE_2D, GL_TEXTURE_MIN_FILTER, GL_NEAREST );

                    glBindTexture( GL_TEXTURE_2D, 0 ); // TODO: Load faster without, no wrong textures with
                },
                "Upload Texture Data " + std::to_string( _id )
        );
        return *this;
    }

    auto GLTexture::size () const -> glm::uvec2 {
        return _size;
    }

    auto GLTexture::size ( glm::uvec2 value ) -> ITexture& {
        sq::client()->graphics()->thread().enqueue(
                [ this, value ] () {
                    _size = value;
                    glTexStorage2D( GL_TEXTURE_2D, 0, GL_RGBA8, value.x, value.y );
                },
                "Resize Texture " + std::to_string( _id )
        );
        return *this;
    }

    auto GLTexture::pixels () const -> std::vector< glm::u8vec4 > {
        std::promise< std::vector< glm::u8vec4 > > promise;
        auto future = promise.get_future();
        auto func = [ this, &promise ] () {
            std::vector< glm::u8vec4 > data;
            data.resize( _size.x * _size.y );

            glBindTexture( GL_TEXTURE_2D, _id );
            glGetTexImage( GL_TEXTURE_2D, 0, GL_RGBA, GL_UNSIGNED_BYTE, &data[ 0 ] );

            promise.set_value( data );
        };
        if ( sq::client()->graphics()->thread().active() ) {
            func();
        } else {
            sq::client()->graphics()->thread().enqueue(
                    func, "Get Pixels"
            );
        }
        future.wait();
        auto data = future.get();
        return data;
    }

    auto GLTexture::id () -> unsigned int {
        return _id;
    }

    ITexture&
    GLTexture::pixels ( glm::uvec2 pos, glm::uvec2 size, const std::vector< unsigned char >& value ) {
        auto v = std::move( value );
        sq::client()->graphics()->thread().enqueue(
                [ this, pos, size, v ] () {
                    glBindTexture( GL_TEXTURE_2D, _id );

                    auto count = 4 * size.x * size.y;
                    std::vector< unsigned char > data{ v.begin(), v.end() };
                    data.resize( count );

                    glTexSubImage2D(
                            GL_TEXTURE_2D,
                            0, pos.x, pos.y, size.x, size.y,
                            GL_RGBA, GL_UNSIGNED_BYTE,
                            data.data()
                    );

                    glBindTexture( GL_TEXTURE_2D, 0 ); // TODO: Load faster without, no wrong textures with
                },
                "Upload Texture Subdata " + std::to_string( _id )
        );
        return *this;
    }

    ITexture& GLTexture::fill ( glm::uvec2 pos, glm::uvec2 size, glm::u8vec4 color ) {
        auto data = std::vector< glm::u8vec4 >{};
        data.resize( size.x * size.y * 4 );
        std::fill( data.begin(), data.end(), color );
        sq::client()->graphics()->thread().enqueue(
                [ this, pos, size, data ] () {
                    glBindTexture( GL_TEXTURE_2D, _id );

                    auto count = 4 * size.x * size.y;

                    glTexSubImage2D(
                            GL_TEXTURE_2D,
                            0, pos.x, pos.y, size.x, size.y,
                            GL_RGBA, GL_UNSIGNED_BYTE,
                            data.data()
                    );

                    glBindTexture( GL_TEXTURE_2D, 0 ); // TODO: Load faster without, no wrong textures with
                },
                "Upload Texture Subdata " + std::to_string( _id )
        );
        return *this;
    }

    ITexture& GLTexture::replace ( glm::u8vec4 source, glm::u8vec4 replacement ) {
        sq::client()->graphics()->thread().enqueue(
                [ this, source, replacement ] () {
                    auto pixels = this->pixels();
                    for ( auto& pixel: pixels ) {
                        if ( pixel == source ) {
                            pixel = replacement;
                        }
                    }
                    this->pixels( pixels );
                },
                "Replace Color"
        );
        return *this;
    }

    ITexture& GLTexture::pixels ( const std::vector< glm::u8vec4 >& value ) {
        return pixels(
                std::vector< unsigned char >{
                        reinterpret_cast<const unsigned char*>(&value[ 0 ]),
                        reinterpret_cast<const unsigned char*>(&value[ 0 ] + value.size())
                }
        );
    }

    ITexture&
    GLTexture::pixels ( glm::uvec2 pos, glm::uvec2 size, const std::vector< glm::u8vec4 >& value ) {
        return pixels(
                pos, size,
                std::vector< unsigned char >{
                        reinterpret_cast<const unsigned char* >(&value[ 0 ]),
                        reinterpret_cast<const unsigned char* >(&value[ 0 ] + value.size())
                }
        );
    }
}