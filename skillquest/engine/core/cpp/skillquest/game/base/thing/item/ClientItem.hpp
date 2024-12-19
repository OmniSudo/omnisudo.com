/**
 * @author omnisudo
 * @date 2024.08.08
 */

#pragma once

#include "skillquest/item.hpp"

namespace skillquest::game::base::thing::item {
class ClientItem : public IItem, public stuff::Thing {
    public:
        struct CreateInfo {
            const URI& uri;
        };

    public:
        explicit ClientItem( const CreateInfo& info );

        ~ClientItem() override;

    private:
    };
}// namespace skillquest::game::base::thing::item