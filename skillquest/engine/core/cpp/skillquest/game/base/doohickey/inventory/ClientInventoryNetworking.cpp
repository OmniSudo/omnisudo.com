/**
 * @author  omnisudo
 * @date    2024.11.02
 */

#include "ClientInventoryNetworking.hpp"

#include "skillquest/game/base/doohickey/item/ClientItemStackNetworking.hpp"
#include "skillquest/game/base/packet/inventory/InventoryInitRequestPacket.hpp"
#include "skillquest/game/base/packet/inventory/InventoryItemStackRequestPacket.hpp"

namespace skillquest::game::base::doohickey::inventory {
    ClientInventoryNetworking::ClientInventoryNetworking(const CreateInfo &info)
        : stuff::Doohickey{
              {.uri = CL_URI}
          },
          _channel{
              sq::shared()->network()->channels().create("inventory", true)
          },
          _localplayer{
              info.localplayer
          } {
        sq::shared()->network()->packets().add<packet::inventory::InventoryInitPacket>();
        sq::shared()->network()->packets().add<packet::inventory::InventoryInitDeniedPacket>();
        _channel->add(this, &ClientInventoryNetworking::onNet_InventoryInitPacket);

        sq::shared()->network()->packets().add<packet::inventory::InventorySyncPacket>();
        sq::shared()->network()->packets().add<packet::inventory::InventorySyncDeniedPacket>();
        _channel->add(this, &ClientInventoryNetworking::onNet_InventorySyncPacket);

        sq::shared()->network()->packets().add<packet::inventory::InventoryUpdatePacket>();
        _channel->add(this, &ClientInventoryNetworking::onNet_InventoryUpdatePacket);

        sq::shared()->network()->packets().add<packet::inventory::InventoryItemUsePacket>();
    }

    ClientInventoryNetworking::~ClientInventoryNetworking() {
    }

    std::shared_future<sq::sh::Inventory> ClientInventoryNetworking::request(const URI &uri) {
        if (init_responses().contains(uri)) return futures()[uri];

        auto response = init_responses()[uri] = std::make_shared<InitResponse>(this, uri);
        _channel->send(localplayer()->connection(), new packet::inventory::InventoryInitRequestPacket{uri});

        auto future = response->promise.get_future().share();
        futures()[uri] = future;
        return future;
    }

    std::shared_future<sq::sh::Inventory> ClientInventoryNetworking::request(sq::sh::Inventory inventory) {
        if (sync_responses().contains(inventory->uri())) return futures()[inventory->uri()];

        auto response = sync_responses()[inventory->uri()] = std::make_shared<SyncResponse>(this, inventory->uri());
        _channel->send(localplayer()->connection(),
                       new packet::inventory::InventoryInitRequestPacket{inventory->uri()});

        auto future = response->promise.get_future().share();
        futures()[inventory->uri()] = future;
        return future;
    }

    std::shared_future<sq::sh::Inventory> ClientInventoryNetworking::request(
        sq::sh::Inventory inventory,
        const URI &slot
    ) {
        auto uri = inventory->uri().toString() +
                   (slot.split_query().size() > 0 ? "&" : "?") +
                   "slot=" + slot
                   .path();
        if (stack_responses().contains(uri)) return futures()[uri];

        auto response = stack_responses()[uri] = std::make_shared<StackResponse>(this, inventory, slot);
        _channel->send(localplayer()->connection(),
                       new packet::inventory::InventoryItemStackRequestPacket{inventory, slot});

        auto future = response->promise.get_future().share();
        futures()[inventory->uri()] = future;
        return future;
    }

    sq::sh::Inventory ClientInventoryNetworking::create(std::shared_ptr<packet::inventory::InventoryInitPacket> &data) {
        if (std::dynamic_pointer_cast<sq::sh::Inventory::element_type>(
            stuff().contains(data->target()) ? stuff()[data->target()] : nullptr
        )) {
            stuff().remove(data->target());
        }

        auto inventory = stuff().create<sq::sh::Inventory::element_type>(
            {
                .uri = data->target()
            }
        );

        auto futures = std::vector<std::pair<URI, std::shared_future<sq::sh::ItemStack> > >{};
        for (const auto &[slot, stack]: data->stacks()) {
            futures.push_back({
                    slot,
                    std::dynamic_pointer_cast<item::ClientItemStackNetworking>(
                        sq::shared()->stuff()->operator[](item::ClientItemStackNetworking::CL_URI)
                    )->request(stack.path())
                }
            );
        }

        for (auto &future: futures) {
            future.second.wait();
            inventory->stacks()[future.first] = future.second.get();
        }

        return inventory;
    }

    ClientInventoryNetworking::InitResponse::InitResponse(ClientInventoryNetworking *networking, const URI &inventory) {
        this->networking = networking;
        channel = sq::shared()->network()->channels().create(inventory.toString());
        promise = std::promise<sq::sh::Inventory>{};

        channel->add(this, &ClientInventoryNetworking::InitResponse::onNet_InventoryInitPacket);
        channel->add(this, &ClientInventoryNetworking::InitResponse::onNet_InventoryInitDeniedPacket);
    }

    void ClientInventoryNetworking::InitResponse::onNet_InventoryInitPacket(
        skillquest::network::Connection connection,
        std::shared_ptr<packet::inventory::InventoryInitPacket> data
    ) {
        sq::shared()->logger()->trace("Creating inventory {0}", data->target());
        auto inventory = networking->create(data);
        promise.set_value(inventory);
        networking->_init_responses.erase(data->target());
    }

    void ClientInventoryNetworking::InitResponse::onNet_InventoryInitDeniedPacket(
        skillquest::network::Connection connection,
        std::shared_ptr<packet::inventory::InventoryInitDeniedPacket> data
    ) {
        sq::shared()->logger()->error("Server denied sending {0}", data->target());
        promise.set_value(nullptr);
        networking->_init_responses.erase(data->target());
    }

    void ClientInventoryNetworking::sync(
        std::shared_ptr<packet::inventory::InventorySyncPacket> &data
    ) {
        sq::sh::Inventory inventory = std::dynamic_pointer_cast<sq::sh::Inventory::element_type>(
            stuff().contains(data->target()) ? stuff()[data->target()] : nullptr
        );
        if (!inventory) {
            return;
        }

        auto futures = std::vector<std::pair<URI, std::shared_future<sq::sh::ItemStack> > >{};
        for (const auto &[slot, stack]: data->stacks()) {
            futures.push_back({
                    slot,
                    std::dynamic_pointer_cast<item::ClientItemStackNetworking>(
                        sq::shared()->stuff()->operator[](item::ClientItemStackNetworking::CL_URI)
                    )->request(stack)
                }
            );
        }

        for (auto &future: futures) {
            future.second.wait();
            inventory->stacks()[future.first] = future.second.get();
        }
    }

    void ClientInventoryNetworking::onNet_InventorySyncPacket(
        skillquest::network::Connection connection,
        std::shared_ptr<packet::inventory::InventorySyncPacket> data
    ) {
        this->sync(data);
    }

    ClientInventoryNetworking::SyncResponse::SyncResponse(ClientInventoryNetworking *networking, const URI &inventory) {
        this->networking = networking;
        channel = sq::shared()->network()->channels().create(inventory.toString());
        promise = std::promise<sq::sh::Inventory>{};

        channel->add(this, &ClientInventoryNetworking::SyncResponse::onNet_InventorySyncPacket);
        channel->add(this, &ClientInventoryNetworking::SyncResponse::onNet_InventorySyncDeniedPacket);
    }

    void ClientInventoryNetworking::SyncResponse::onNet_InventorySyncPacket(
        skillquest::network::Connection connection,
        std::shared_ptr<packet::inventory::InventorySyncPacket> data
    ) {
        sq::shared()->logger()->trace("Syncing inventory {0}", data->target());
        networking->sync(data);
        auto inventory = networking->stuff()
                .at<sq::sh::Inventory::element_type>(data->target())
                .begin()
                .operator*()
                .second.lock();
        promise.set_value(inventory);
        networking->_init_responses.erase(data->target());
    }

    void ClientInventoryNetworking::SyncResponse::onNet_InventorySyncDeniedPacket(
        skillquest::network::Connection connection,
        std::shared_ptr<packet::inventory::InventorySyncDeniedPacket> data
    ) {
        sq::shared()->logger()->error("Server denied syncing {0}", data->target());
        promise.set_value(nullptr);
        networking->_init_responses.erase(data->target());
    }

    sq::sh::ItemStack ClientInventoryNetworking::use(std::shared_ptr<packet::inventory::InventoryItemUsePacket> &data) {
        // TODO: Use the item using the ClientItemSystem?
        auto inventory = stuff()                .at<sq::sh::Inventory::element_type>(data->target())
                .begin()
                .operator*()
                .second.lock();

        auto stack = inventory->stacks().contains( data->slot() ) ? inventory->stacks()[ data->slot() ] : nullptr;

        return stack;
    }

    ClientInventoryNetworking::UseResponse::UseResponse(
        ClientInventoryNetworking *networking, const URI &inventory,
        const URI &slot
    ) {
        this->networking = networking;
        channel = sq::shared()->network()->channels().create(
            inventory.toString() +
            (inventory.split_query().size() > 0 ? "&" : "?") +
            "slot=" + slot.path()
        );
        promise = std::promise<sq::sh::Inventory>{};

        channel->add(this, &ClientInventoryNetworking::UseResponse::onNet_InventoryItemUsePacket);
    }

    void ClientInventoryNetworking::UseResponse::onNet_InventoryItemUsePacket(
        skillquest::network::Connection connection,
        std::shared_ptr<packet::inventory::InventoryItemUsePacket> data
    ) {
        sq::shared()->logger()->trace("Using inventory {0} item {1}", data->target(), data->slot());
        networking->use( data );
        auto inventory = networking->stuff()
                .at<sq::sh::Inventory::element_type>(data->target())
                .begin()
                .operator*()
                .second.lock();
        promise.set_value(inventory);
        networking->_use_responses.erase(data->target());
    }

    sq::sh::ItemStack ClientInventoryNetworking::update(
        std::shared_ptr<packet::inventory::InventoryUpdatePacket> &data
    ) {
        sq::sh::Inventory inventory = std::dynamic_pointer_cast<sq::sh::Inventory::element_type>(
            stuff().contains(data->target()) ? stuff()[data->target()] : nullptr
        );
        if (!inventory) {
            return nullptr;
        }

        auto future = request(inventory, data->slot());
        future.wait();
        return future.get()->stacks()[data->slot()];
    }

    void ClientInventoryNetworking::onNet_InventoryUpdatePacket(
        skillquest::network::Connection connection,
        std::shared_ptr<packet::inventory::InventoryUpdatePacket> data
    ) {
    }

    void ClientInventoryNetworking::onNet_InventoryInitPacket(
        skillquest::network::Connection connection,
        std::shared_ptr<packet::inventory::InventoryInitPacket> data
    ) {
    }
}
