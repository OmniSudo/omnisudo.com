/**
 * @author  omnisudo
 * @date    2024.11.02
 */

#pragma once

#include "skillquest/game/base/packet/inventory/InventoryItemStackPacket.hpp"
#include "skillquest/game/base/packet/inventory/InventoryInitPacket.hpp"
#include "skillquest/game/base/packet/inventory/InventoryInitDeniedPacket.hpp"
#include "skillquest/game/base/packet/inventory/InventorySyncPacket.hpp"
#include "skillquest/game/base/packet/inventory/InventorySyncDeniedPacket.hpp"
#include "skillquest/game/base/packet/inventory/InventoryItemUsePacket.hpp"
#include "skillquest/game/base/thing/character/player/LocalPlayer.hpp"
#include "skillquest/stuff.doohickey.hpp"
#include "skillquest/game/base/packet/inventory/InventoryUpdatePacket.hpp"

namespace skillquest::game::base::doohickey::inventory {
    class ClientInventoryNetworking : public stuff::Doohickey {
    public:
        inline static const URI CL_URI = { "net://skill.quest/client/block" };

        struct CreateInfo {
            std::shared_ptr< thing::character::player::LocalPlayer > localplayer;
        };

        explicit ClientInventoryNetworking( const CreateInfo& info );

        ~ClientInventoryNetworking () override;

        std::shared_future< sq::sh::Inventory > request( const URI& uri );
        std::shared_future< sq::sh::Inventory > request( sq::sh::Inventory inventory );
        std::shared_future< sq::sh::Inventory > request( sq::sh::Inventory inventory, const URI& slot );

    private:
        sq::sh::Inventory create( std::shared_ptr< packet::inventory::InventoryInitPacket >& data );

        struct InitResponse {
            ClientInventoryNetworking* networking;
            skillquest::network::Channel* channel;
            std::promise< sq::sh::Inventory > promise;

            InitResponse ( ClientInventoryNetworking* networking, const URI& inventory );

            ~InitResponse () {
                sq::shared()->network()->channels().destroy( channel );
                channel = nullptr;
            }

            net_receive( InventoryInitPacket, packet::inventory::InventoryInitPacket );
            net_receive( InventoryInitDeniedPacket, packet::inventory::InventoryInitDeniedPacket );
        };

        void sync( std::shared_ptr< packet::inventory::InventorySyncPacket >& data );
        net_receive( InventorySyncPacket, packet::inventory::InventorySyncPacket );

        struct SyncResponse {
            ClientInventoryNetworking* networking;
            skillquest::network::Channel* channel;
            std::promise< sq::sh::Inventory > promise;

            SyncResponse ( ClientInventoryNetworking* networking, const URI& inventory );

            ~SyncResponse () {
                sq::shared()->network()->channels().destroy( channel );
                channel = nullptr;
            }

            net_receive( InventorySyncPacket, packet::inventory::InventorySyncPacket );
            net_receive( InventorySyncDeniedPacket, packet::inventory::InventorySyncDeniedPacket );
        };

        struct StackResponse {
            ClientInventoryNetworking* networking;
            skillquest::network::Channel* channel;
            std::promise< sq::sh::Inventory > promise;

            StackResponse ( ClientInventoryNetworking* networking, sq::sh::Inventory inventory, const URI& slot );

            ~StackResponse () {
                sq::shared()->network()->channels().destroy( channel );
                channel = nullptr;
            }

            net_receive( InventorySyncPacket, packet::inventory::InventoryItemStackPacket );
        };

        sq::sh::ItemStack use( std::shared_ptr< packet::inventory::InventoryItemUsePacket >& data );
        struct UseResponse {
            ClientInventoryNetworking* networking;
            skillquest::network::Channel* channel;
            std::promise< sq::sh::Inventory > promise;

            UseResponse ( ClientInventoryNetworking* networking, const URI& inventory, const URI& slot );

            ~UseResponse () {
                sq::shared()->network()->channels().destroy( channel );
                channel = nullptr;
            }

            net_receive( InventoryItemUsePacket, packet::inventory::InventoryItemUsePacket );
        };

        sq::sh::ItemStack update( std::shared_ptr< packet::inventory::InventoryUpdatePacket >& data );

        net_receive( InventoryUpdatePacket, packet::inventory::InventoryUpdatePacket );
        net_receive( InventoryInitPacket, packet::inventory::InventoryInitPacket );

        property( channel, std::shared_ptr< skillquest::network::Channel >, protected_ref, none );
        property( localplayer, std::shared_ptr< thing::character::player::LocalPlayer >, protected, none );
        property( init_responses, std::map< URI COMMA std::shared_ptr< InitResponse > >, protected_ref, none );
        property( sync_responses, std::map< URI COMMA std::shared_ptr< SyncResponse > >, protected_ref, none );
        property( stack_responses, std::map< URI COMMA std::shared_ptr< StackResponse > >, protected_ref, none );
        property( use_responses, std::map< URI COMMA std::shared_ptr< UseResponse > >, protected_ref, none );
        property( futures, std::map< URI COMMA std::shared_future< sq::sh::Inventory > >, protected_ref, none );


    };
}
