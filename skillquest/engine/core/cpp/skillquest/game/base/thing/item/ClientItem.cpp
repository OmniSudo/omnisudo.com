/**
 * @author omnisudo
 * @date 2024.08.08
 */

#include "ClientItem.hpp"

namespace skillquest::game::base::thing::item {
    ClientItem::ClientItem ( const CreateInfo& info ) : stuff::Thing{
            {
                            .uri = info.uri,
            }
    } {
    }

    ClientItem::~ClientItem () {
    }
}// namespace skillquest::game::base::thing::item