/**
 * @author omnisudo
 * @date 2024.08.07
 */

#include "ClientAssetNetworker.hpp"
#include "skillquest/game/base/packet/asset/AssetFileRequestPacket.hpp"
#include "skillquest/sh.api.hpp"
#include <fstream>
#include "skillquest/base64.hpp"

namespace skillquest::game::base::assets {

    ClientAssetNetworker::ClientAssetNetworker()
        : stuff::Doohickey{ { .uri = { "net://skill.quest/client/assets" } } },
          _channel{ sq::shared()->network()->channels().create( "image", true ) } {
        sq::shared()->network()->packets().add< packet::asset::AssetFilePacket >();
        _channel->add( this, &ClientAssetNetworker::onNet_AssetFilePacket );
    }

    void ClientAssetNetworker::onDeactivate() {
        Thing::onDeactivate();
        _channel->drop( this );
    }

    void ClientAssetNetworker::download( network::Connection connection, std::string filename ) {
        _channel->send( connection, new packet::asset::AssetFileRequestPacket{ filename } );
    }

    void ClientAssetNetworker::onNet_AssetFilePacket( skillquest::network::Connection connection, std::shared_ptr< packet::asset::AssetFilePacket > data ) {
        (std::ofstream( data->filename().c_str(), std::ios::binary ) << convert::base64::decode( data->data_b64() ) ).close();
    }

}// namespace skillquest::game::base::assets