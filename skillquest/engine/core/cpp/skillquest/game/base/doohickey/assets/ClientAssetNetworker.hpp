/**
 * @author omnisudo
 * @date 2024.08.07
 */

#pragma once

#include "skillquest/game/base/packet/asset/AssetFilePacket.hpp"
#include "skillquest/network.hpp"
#include "skillquest/stuff.doohickey.hpp"

namespace skillquest::game::base::assets {
    class ClientAssetNetworker : public stuff::Doohickey {
    public:
        explicit ClientAssetNetworker();

        void onDeactivate () override;

    public:
        void download ( network::Connection connection, std::string filename );

    private:
        net_receive( AssetFilePacket, packet::asset::AssetFilePacket );

        property( channel, network::Channel*&, private, none );
    };
}