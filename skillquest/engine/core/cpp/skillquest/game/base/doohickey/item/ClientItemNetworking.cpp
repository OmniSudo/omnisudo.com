/**
 * @author omnisudo
 * @date 2024.08.07
 */

#include "ClientItemNetworking.hpp"
#include "skillquest/game/base/thing/item/ClientItem.hpp"
#include "skillquest/sh.api.hpp"

namespace skillquest::game::base::doohickey::item {
    ClientItemNetworking::ClientItemNetworking ( const ClientItemNetworking::CreateInfo& info )
            : stuff::Doohickey{ { .uri = CL_URI } },
              _channel{ sq::shared()->network()->channels().create( "item", true ) },
              _localplayer{ info.localplayer } {
        sq::shared()->network()->packets().add< packet::item::ItemInfoPacket >();
        _channel->add( this, &ClientItemNetworking::onNet_ItemInfoPacket );
    }

    void item::ClientItemNetworking::onNet_ItemInfoPacket (
            skillquest::network::Connection connection,
            std::shared_ptr< packet::item::ItemInfoPacket > data
    ) {
        // Create or update the item in the client's item storage
        sq::shared()->logger()->trace( "Creating item {0}", data->uri() );
        auto item = createOrUpdate( data );
    }

    void ClientItemNetworking::onNet_ItemInfoDeniedPacket (
            skillquest::network::Connection connection, std::shared_ptr< packet::item::ItemInfoDeniedPacket > data
    ) {
        sq::shared()->logger()->error( "Server cannot send item {0}", data->uri() );
    }

    void ClientItemNetworking::onDeactivate () {
        stuff::Doohickey::onDeactivate();
        _channel->drop( this );
    }

    std::shared_ptr< thing::item::IItem >
    ClientItemNetworking::createOrUpdate ( std::shared_ptr< packet::item::ItemInfoPacket >& data ) {
        if ( std::dynamic_pointer_cast< thing::item::ClientItem >(
                stuff().contains( data->uri() ) ? stuff()[ data->uri() ] : nullptr
        ) ) {
            stuff().remove( data->uri() );
        }

        return stuff().create< thing::item::ClientItem >(
                { .uri = data->uri() }
        );
    }

    std::shared_future< std::shared_ptr< thing::item::IItem > > ClientItemNetworking::request ( const URI& uri, bool force ) {
        if ( responses().contains( uri ) ) return futures()[ uri ];

        auto response = responses()[ uri ] = std::make_shared< Response >( this, uri );
        if ( force || !stuff().contains( uri ) ) {
            _channel->send( localplayer()->connection(), new packet::item::ItemInfoRequestPacket{ uri } );
        } else {
            response->promise.set_value( std::dynamic_pointer_cast< thing::item::IItem >( stuff()[ uri ] ) );
        }

        auto future = response->promise.get_future().share();
        futures()[ uri ] = future;
        return future;
    }

    ClientItemNetworking::Response::Response ( ClientItemNetworking* networking, const URI& uri ) {
        this->networking = networking;
        channel = sq::shared()->network()->channels().create( uri.toString() );
        promise = std::promise< std::shared_ptr< thing::item::IItem > >{};

        channel->add( this, &ClientItemNetworking::Response::onNet_ResponseItemInfoDeniedPacket );
        channel->add( this, &ClientItemNetworking::Response::onNet_ResponseItemInfoPacket );
    }

    void ClientItemNetworking::Response::onNet_ResponseItemInfoPacket (
            skillquest::network::Connection connection, std::shared_ptr< packet::item::ItemInfoPacket > data
    ) {
        sq::shared()->logger()->trace( "Creating item {0}", data->uri() );
        auto item = networking->createOrUpdate( data );

        promise.set_value( item );

        channel = nullptr;
        networking->_responses.erase( data->uri() );
    }

    void ClientItemNetworking::Response::onNet_ResponseItemInfoDeniedPacket (
            skillquest::network::Connection connection, std::shared_ptr< packet::item::ItemInfoDeniedPacket > data
    ) {
        sq::shared()->logger()->error( "Server denied sending {0}", data->uri() );

        promise.set_value( nullptr );

        channel = nullptr;
        networking->_responses.erase( data->uri() );
    }
}