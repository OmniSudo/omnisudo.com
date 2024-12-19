/**
 * @author omnisudo
 * @date 8/29/24
 */
#pragma once


#include "skillquest/sh.api.hpp"
#include "skillquest/game/base/packet/block/BlockInfoRequestPacket.hpp"
#include "skillquest/game/base/packet/block/BlockInfoPacket.hpp"
#include "skillquest/game/base/packet/block/BlockInfoDeniedPacket.hpp"
#include "skillquest/game/base/thing/block/Block.hpp"
#include "skillquest/game/base/thing/character/player/LocalPlayer.hpp"
#include "skillquest/stuff.doohickey.hpp"

namespace skillquest::game::base::doohickey::block {
    class ClientBlockNetworking : public stuff::Doohickey {
    public:
        inline static const URI CL_URI = { "net://skill.quest/client/block" };

        struct CreateInfo {
            std::shared_ptr< thing::character::player::LocalPlayer > localplayer;
        };

        explicit ClientBlockNetworking( const CreateInfo& info );

        void onDeactivate() override;

        std::shared_future< std::shared_ptr< thing::block::IBlock > > request( const URI& uri );

    private:
        std::shared_ptr<thing::block::IBlock> createOrUpdateBlock( std::shared_ptr< packet::block::BlockInfoPacket >& data );

        struct Response {
            ClientBlockNetworking* networking;
            skillquest::network::Channel* channel;
            std::promise< std::shared_ptr< thing::block::IBlock > > promise;

            Response ( ClientBlockNetworking* networking, const URI& uri );

            ~Response () {
                sq::shared()->network()->channels().destroy( channel );
                channel = nullptr;
            }

            net_receive( ResponseBlockInfoPacket, packet::block::BlockInfoPacket );
            net_receive( ResponseBlockInfoDeniedPacket, packet::block::BlockInfoDeniedPacket );
        };

        net_receive( BlockInfoPacket, packet::block::BlockInfoPacket );
        net_receive( BlockInfoDeniedPacket, packet::block::BlockInfoDeniedPacket );

        property( channel, std::shared_ptr< skillquest::network::Channel >, protected_ref, none );
        property( localplayer, std::shared_ptr< thing::character::player::LocalPlayer >, protected, none );
        property( responses, std::map< URI COMMA std::shared_ptr< Response > >, protected_ref, none );
        property( futures, std::map< URI COMMA std::shared_future< std::shared_ptr< thing::block::IBlock > > >, protected_ref, none );


    };
}