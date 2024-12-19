/**
 * @author omnisudo
 * @date 8/29/24
 */
#pragma once

#include "skillquest/block.hpp"
#include "skillquest/graphics/Model.hpp"

namespace skillquest::game::base::thing::block {
    class ClientBlock : public block::IBlock, public stuff::Thing {
    public:
        struct CreateInfo {
            const URI& uri;

            const std::map<
                    math::Direction,
                    std::vector< math::Vertex >
            > vertexes;

            const std::map<
                    math::Direction,
                    std::vector< unsigned int >
            > indexes;

            const std::map<
                    math::Direction,
                    std::shared_ptr< graphics::ITexture >
            > textures;

        };

    public:
        explicit ClientBlock ( const CreateInfo& info );

        ~ClientBlock () override;

    private:
    property(
            vertexes,
              std::map<
                      math::Direction COMMA
                      std::vector< math::Vertex >
              >,
              public_ref, public
    );

    property(
            indexes,
            std::map<
                    math::Direction COMMA
                    std::vector< unsigned int>
            >,
            public_ref, public
    );
    property(
            textures,
            std::map<
                    math::Direction COMMA
                    std::shared_ptr< graphics::ITexture >
            >,
            public_ref, public
    );
    };
}