/**
 * @author omnisudo
 * @date 8/29/24
 */
#include "ClientBlockNetworking.hpp"
#include "skillquest/game/base/thing/block/ClientBlock.hpp"
#include "skillquest/sh.api.hpp"

namespace skillquest::game::base::doohickey::block {
    ClientBlockNetworking::ClientBlockNetworking ( const ClientBlockNetworking::CreateInfo& info )
            : stuff::Doohickey{ { .uri = CL_URI } },
              _channel{ sq::shared()->network()->channels().create( "block", true ) },
              _localplayer{ info.localplayer } {
        sq::shared()->network()->packets().add< packet::block::BlockInfoPacket >();
        _channel->add( this, &ClientBlockNetworking::onNet_BlockInfoPacket );

        sq::shared()->network()->packets().add< packet::block::BlockInfoDeniedPacket >();
        _channel->add( this, &ClientBlockNetworking::onNet_BlockInfoDeniedPacket );
    }

    void block::ClientBlockNetworking::onNet_BlockInfoPacket (
            skillquest::network::Connection connection,
            std::shared_ptr< packet::block::BlockInfoPacket > data
    ) {
        // Create or update the block in the client's block storage
        sq::shared()->logger()->trace( "Creating block {0}", data->uri() );
        auto block = createOrUpdateBlock( data );
    }

    void ClientBlockNetworking::onNet_BlockInfoDeniedPacket (
            skillquest::network::Connection connection, std::shared_ptr< packet::block::BlockInfoDeniedPacket > data
    ) {
        sq::shared()->logger()->error( "Server cannot send block {0}", data->uri() );
    }

    void ClientBlockNetworking::onDeactivate () {
        stuff::Doohickey::onDeactivate();
        _channel->drop( this );
    }

    std::shared_ptr< thing::block::IBlock >
    ClientBlockNetworking::createOrUpdateBlock ( std::shared_ptr< packet::block::BlockInfoPacket >& data ) {
        if ( std::dynamic_pointer_cast< thing::block::ClientBlock >(
                stuff().contains( data->uri() ) ? stuff()[ data->uri() ] : nullptr
        ) ) {
            stuff().remove( data->uri() );
        }

        return stuff().create< thing::block::ClientBlock >(
                { .uri = data->uri() }
        );
    }

    std::shared_future< std::shared_ptr< thing::block::IBlock > > ClientBlockNetworking::request ( const URI& uri ) {
        if ( responses().contains( uri ) ) return futures()[ uri ];

        auto response = responses()[ uri ] = std::make_shared< Response >( this, uri );
        _channel->send( localplayer()->connection(), new packet::block::BlockInfoRequestPacket{ uri } );

        auto future = response->promise.get_future().share();
        futures()[ uri ] = future;
        return future;
    }

    ClientBlockNetworking::Response::Response ( ClientBlockNetworking* networking, const URI& uri ) {
        this->networking = networking;
        channel = sq::shared()->network()->channels().create( uri.toString() );
        promise = std::promise< std::shared_ptr< thing::block::IBlock > >{};

        channel->add( this, &ClientBlockNetworking::Response::onNet_ResponseBlockInfoDeniedPacket );
        channel->add( this, &ClientBlockNetworking::Response::onNet_ResponseBlockInfoPacket );
    }

    void ClientBlockNetworking::Response::onNet_ResponseBlockInfoPacket (
            skillquest::network::Connection connection, std::shared_ptr< packet::block::BlockInfoPacket > data
    ) {
        sq::shared()->logger()->trace( "Creating block {0}", data->uri() );
        auto block = networking->createOrUpdateBlock( data );

        promise.set_value( block );

        channel = nullptr;
        networking->_responses.erase( data->uri() );
    }

    void ClientBlockNetworking::Response::onNet_ResponseBlockInfoDeniedPacket (
            skillquest::network::Connection connection, std::shared_ptr< packet::block::BlockInfoDeniedPacket > data
    ) {
        sq::shared()->logger()->error( "Server denied sending {0}", data->uri() );

        promise.set_value( nullptr );

        channel = nullptr;
        networking->_responses.erase( data->uri() );
    }
}