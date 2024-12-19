/**
 * @author omnisudo
 * @date 2024.08.08
 */

#pragma once

#include "skillquest/game/base/packet/item/ItemStackInfoPacket.hpp"
#include "skillquest/game/base/packet/item/ItemStackInfoDeniedPacket.hpp"
#include "skillquest/game/base/thing/character/player/LocalPlayer.hpp"
#include "skillquest/network.hpp"
#include "skillquest/sh.api.hpp"
#include "skillquest/stuff.doohickey.hpp"

namespace skillquest::game::base::doohickey::item {
    class ClientItemStackNetworking : public stuff::Doohickey {
    public:
        inline static const URI CL_URI = { "net://skill.quest/client/itemstack" };

        struct CreateInfo {
            std::shared_ptr< thing::character::player::LocalPlayer > localplayer;
        };

        explicit ClientItemStackNetworking( const CreateInfo& info );

        void onDeactivate() override;

        std::shared_future< std::shared_ptr< thing::item::ItemStack > > request ( const util::UID uid );

    public:
        static URI convertStackUIDToURI( const util::UID& uid );

    private:
        std::shared_ptr<thing::item::ItemStack> createOrUpdate( std::shared_ptr< packet::item::ItemStackInfoPacket >& data );

        struct Response {
            ClientItemStackNetworking* networking;
            skillquest::network::Channel* channel;
            std::promise< std::shared_ptr< thing::item::ItemStack > > promise;

            Response ( ClientItemStackNetworking* networking, const URI& uri );

            ~Response () {
                sq::shared()->network()->channels().destroy( channel );
                channel = nullptr;
            }

            net_receive( ResponseItemStackInfoPacket, packet::item::ItemStackInfoPacket );
            net_receive( ResponseItemStackInfoDeniedPacket, packet::item::ItemStackInfoDeniedPacket );
        };

        net_receive( ItemStackInfoPacket, packet::item::ItemStackInfoPacket );
        net_receive( ItemStackInfoDeniedPacket, packet::item::ItemStackInfoDeniedPacket );

        property( channel, std::shared_ptr< skillquest::network::Channel >, protected_ref, none );
        property( localplayer, std::shared_ptr< thing::character::player::LocalPlayer >, protected, none );
        property( responses, std::map< URI COMMA std::shared_ptr< Response > >, protected_ref, none );
        property( futures, std::map< URI COMMA std::shared_future< std::shared_ptr< thing::item::ItemStack > > >, protected_ref, none );
    };
}// namespace skillquest::game::base::thing::item
