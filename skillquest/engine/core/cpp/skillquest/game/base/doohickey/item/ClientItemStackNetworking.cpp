/**
 * @author omnisudo
 * @date 8/29/24
 */
#include "ClientItemStackNetworking.hpp"

#include <stack>

#include "ClientItemNetworking.hpp"
#include "skillquest/game/base/thing/item/ItemStack.hpp"
#include "skillquest/sh.api.hpp"
#include "skillquest/game/base/packet/item/ItemStackInfoRequestPacket.hpp"

namespace skillquest::game::base::doohickey::item {
    ClientItemStackNetworking::ClientItemStackNetworking ( const ClientItemStackNetworking::CreateInfo& info )
            : stuff::Doohickey{ { .uri = CL_URI } },
              _channel{ sq::shared()->network()->channels().create( "itemstack", true ) },
              _localplayer{ info.localplayer } {
        sq::shared()->network()->packets().add< packet::item::ItemStackInfoPacket >();
        _channel->add( this, &ClientItemStackNetworking::onNet_ItemStackInfoPacket );

        sq::shared()->network()->packets().add< packet::item::ItemStackInfoDeniedPacket >();
        _channel->add( this, &ClientItemStackNetworking::onNet_ItemStackInfoDeniedPacket );
    }

    void ClientItemStackNetworking::onNet_ItemStackInfoPacket (
            skillquest::network::Connection connection,
            std::shared_ptr< packet::item::ItemStackInfoPacket > data
    ) {
        // Create or update the itemstack in the client's itemstack storage
        sq::shared()->logger()->trace( "Creating itemstack {0}", data->stack_uid() );
        auto itemstack = createOrUpdate( data );
    }

    void ClientItemStackNetworking::onNet_ItemStackInfoDeniedPacket (
            skillquest::network::Connection connection, std::shared_ptr< packet::item::ItemStackInfoDeniedPacket > data
    ) {
        sq::shared()->logger()->error( "Server cannot send itemstack {0}", data->uid() );
    }

    void ClientItemStackNetworking::onDeactivate () {
        stuff::Doohickey::onDeactivate();
        _channel->drop( this );
    }

    std::shared_ptr< thing::item::ItemStack >
    ClientItemStackNetworking::createOrUpdate ( std::shared_ptr< packet::item::ItemStackInfoPacket >& data ) {
        auto uri = convertStackUIDToURI( data->stack_uid() );
        auto stack = std::dynamic_pointer_cast< thing::item::ItemStack >(
                stuff().contains( uri ) ? stuff()[ uri ] : nullptr
        );

        if ( !stack ) {
            stuff().remove( uri );
            stack = stuff().create< thing::item::ItemStack >(
                {
                    .id = data->stack_uid(),
                    .owner = localplayer(), // TODO: Allow for non local player to own items
                    .item = std::dynamic_pointer_cast<ClientItemNetworking>( stuff()[ClientItemNetworking::CL_URI])->request( data->item_uri() ).get(),
                    .count = data->count(),
                }
            );
        } else {
            stack->count( data->count() );
            if ( !stack->item() || stack->item()->uri() != data->item_uri() ) {
                auto item = std::dynamic_pointer_cast<ClientItemNetworking>( stuff()[ClientItemNetworking::CL_URI])->request( data->item_uri() ).get();
                stack->item( item );
            }
            // TODO: Set owner
        }

        return stack;
    }

    std::shared_future< std::shared_ptr< thing::item::ItemStack > > ClientItemStackNetworking::request ( const util::UID uid ) {
        auto uri = convertStackUIDToURI( uid );
        if ( responses().contains( uri ) ) return futures()[ uri ];

        auto response = responses()[ uri ] = std::make_shared< Response >( this, uri );
        _channel->send( localplayer()->connection(), new packet::item::ItemStackInfoRequestPacket{ uid } );

        auto future = response->promise.get_future().share();
        futures()[ uri ] = future;
        return future;
    }

    URI ClientItemStackNetworking::convertStackUIDToURI(const util::UID &uid) {
        return { "itemstack://skill.quest/" + uid.toString() };
    }

    ClientItemStackNetworking::Response::Response ( ClientItemStackNetworking* networking, const URI& uri ) {
        this->networking = networking;
        channel = sq::shared()->network()->channels().create( uri.toString() );
        promise = std::promise< std::shared_ptr< thing::item::ItemStack > >{};

        channel->add( this, &ClientItemStackNetworking::Response::onNet_ResponseItemStackInfoDeniedPacket );
        channel->add( this, &ClientItemStackNetworking::Response::onNet_ResponseItemStackInfoPacket );
    }

    void ClientItemStackNetworking::Response::onNet_ResponseItemStackInfoPacket (
            skillquest::network::Connection connection, std::shared_ptr< packet::item::ItemStackInfoPacket > data
    ) {
        sq::shared()->logger()->trace( "Creating itemstack {0}", data->stack_uid() );
        auto itemstack = networking->createOrUpdate( data );

        promise.set_value( itemstack );

        channel = nullptr;
        networking->_responses.erase( convertStackUIDToURI( data->stack_uid() ) );
    }

    void ClientItemStackNetworking::Response::onNet_ResponseItemStackInfoDeniedPacket (
            skillquest::network::Connection connection, std::shared_ptr< packet::item::ItemStackInfoDeniedPacket > data
    ) {
        sq::shared()->logger()->error( "Server denied sending stack {0}", data->uid() );

        promise.set_value( nullptr );

        channel = nullptr;
        networking->_responses.erase( convertStackUIDToURI( data->uid() ) );
    }
}
