/**
 * @author omnisudo
 * @date 8/29/24
 */
#include "ClientBlock.hpp"

namespace skillquest::game::base::thing::block {
    ClientBlock::ClientBlock ( const CreateInfo& info ) :
            stuff::Thing{
                    {
                            .uri = info.uri,
                    },
            },
            _indexes{ info.indexes },
            _vertexes{ info.vertexes },
            _textures{ info.textures } {

    }

    ClientBlock::~ClientBlock () {
    }
}