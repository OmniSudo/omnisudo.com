/**
 * @author omnisudo
 * @date 2024.08.01
 */

#pragma once

#include <utility>

#include "skillquest/game/base/packet/item/ItemInfoPacket.hpp"
#include "skillquest/game/base/packet/item/ItemInfoRequestPacket.hpp"
#include "skillquest/game/base/packet/item/ItemInfoDeniedPacket.hpp"
#include "skillquest/game/base/thing/character/player/LocalPlayer.hpp"
#include "skillquest/network.hpp"
#include "skillquest/sh.api.hpp"
#include "skillquest/stuff.doohickey.hpp"

namespace skillquest::game::base::doohickey::item {
    class ClientItemNetworking : public stuff::Doohickey {
    public:
        inline static const URI CL_URI = { "net://skill.quest/client/item" };

        struct CreateInfo {
            std::shared_ptr< thing::character::player::LocalPlayer > localplayer;
        };

        explicit ClientItemNetworking ( const CreateInfo& info );

        void onDeactivate () override;

        /**
         * Request an item defintion from the server; searches for local before sending net message by default
         * @param uri
         * @return
         */
        std::shared_future< std::shared_ptr< thing::item::IItem > > request( const URI& uri, bool force = false );

    private:
        std::shared_ptr<thing::item::IItem> createOrUpdate( std::shared_ptr< packet::item::ItemInfoPacket >& data );

        struct Response {
            ClientItemNetworking* networking;
            skillquest::network::Channel* channel;
            std::promise< std::shared_ptr< thing::item::IItem > > promise;

            Response ( ClientItemNetworking* networking, const URI& uri );

            ~Response () {
                sq::shared()->network()->channels().destroy( channel );
                channel = nullptr;
            }

            net_receive( ResponseItemInfoPacket, packet::item::ItemInfoPacket );
            net_receive( ResponseItemInfoDeniedPacket, packet::item::ItemInfoDeniedPacket );
        };

        net_receive( ItemInfoPacket, packet::item::ItemInfoPacket );
        net_receive( ItemInfoDeniedPacket, packet::item::ItemInfoDeniedPacket );

        property( channel, std::shared_ptr< skillquest::network::Channel >, protected_ref, none );
        property( localplayer, std::shared_ptr< thing::character::player::LocalPlayer >, protected, none );
        property( responses, std::map< URI COMMA std::shared_ptr< Response > >, protected_ref, none );
        property( futures, std::map< URI COMMA std::shared_future< std::shared_ptr< thing::item::IItem > > >, protected_ref, none );

    };
}// namespace skillquest::game::base::thing::item