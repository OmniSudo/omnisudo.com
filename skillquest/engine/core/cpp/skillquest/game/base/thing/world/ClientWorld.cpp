/**
 * @author omnisudo
 * @date 2024.08.01
 */

#include "skillquest/game/base/thing/world/ClientWorld.hpp"
#include "skillquest/game/base/thing/character/player/RemotePlayer.hpp"
#include "skillquest/sh.api.hpp"
#include "skillquest/game/base/packet/world/chunk/ChunkDataRequestPacket.hpp"
#include "skillquest/game/base/thing/world/chunk/ClientChunk.hpp"
#include "skillquest/game/base/thing/block/Block.hpp"
#include "skillquest/game/base/doohickey/block/ClientBlockNetworking.hpp"

#include <execution>
#include <mutex>
#include <condition_variable>
#include <thread>

namespace skillquest::game::base::thing::world {
    ClientWorld::ClientWorld ( const ClientWorld::CreateInfo& info ) : World( info ) {
        sq::shared()->network()->packets().add< packet::character::CharacterJoinWorldPacket >();
        channel()->add( this, &ClientWorld::onNet_CharacterJoinWorldPacket );

        sq::shared()->network()->packets().add< packet::character::CharacterLeaveWorldPacket >();
        channel()->add( this, &ClientWorld::onNet_CharacterLeaveWorldPacket );

        sq::shared()->network()->packets().add< packet::world::chunk::ChunkDataPacket >();
        channel()->add( this, &ClientWorld::onNet_ChunkDataPacket );
    }

    ClientWorld::~ClientWorld () {
        channel()->drop( this );
    }

    void ClientWorld::onNet_CharacterLeaveWorldPacket (
            skillquest::network::Connection connection,
            std::shared_ptr< packet::character::CharacterLeaveWorldPacket > data
    ) {
        auto thing = stuff()[ "player://skill.quest/" + data->world() + "/" + data->name() ];

        this->remove_player(
                std::dynamic_pointer_cast< character::player::RemotePlayer >(
                        thing
                )
        );
    }

    void ClientWorld::onNet_CharacterJoinWorldPacket (
            skillquest::network::Connection connection,
            std::shared_ptr< packet::character::CharacterJoinWorldPacket > data
    ) {
        sq::shared()->logger()->trace( "Character {0} joined world", data->name() );

        add_player(
                stuff().create< character::player::RemotePlayer >(
                        {
                                .player = {
                                        .character = {
                                                .thing = {
                                                        .uri = {
                                                                "player://skill.quest/" + this->name() + "/" +
                                                                data->name()
                                                        }
                                                },
                                                .uid = data->uid(),
                                        },
                                        .name = data->name()
                                }
                        }
                )
        );
    }

    void ClientWorld::download ( glm::u16vec3 pos ) {
        if ( localhost() ) {
            channel()->send(
                    localhost()->connection(), new packet::world::chunk::ChunkDataRequestPacket{ pos }
            );
        }
    }

    void ClientWorld::onNet_ChunkDataPacket (
            skillquest::network::Connection connection,
            std::shared_ptr< packet::world::chunk::ChunkDataPacket > data
    ) {
        std::thread(
                [ this, connection, data ] () {
                    auto raw = data->blocks();
                    auto ids = data->ids();
                    auto pos = data->pos();
                    std::map< unsigned long, stuff::IThing* > things;

                    auto networker = std::dynamic_pointer_cast< doohickey::block::ClientBlockNetworking >(
                            stuff()[ doohickey::block::ClientBlockNetworking::CL_URI ]
                    );
                    std::mutex mutex{};
                    std::unique_lock lock{ mutex };
                    std::condition_variable check;
                    int count = ids.size();
                    int complete = 0;
                    std::for_each(
                            std::execution::par,
                            ids.begin(), ids.end(),
                            [ this, networker, &things, &complete, &check ] ( auto&& i ) {
                                if ( stuff().contains( i.second ) ) {
                                    auto thing = std::dynamic_pointer_cast< block::IBlock >( stuff()[ i.second ] );
                                    things[ i.first ] = thing.get();
                                } else {
                                    auto future = networker->request( i.second );
                                    future.wait();
                                    things[ i.first ] = future.get().get();
                                }
                                complete++;
                                check.notify_all();
                            }
                    );
                    check.wait(
                            lock, [ count, complete ] () {
                                return complete >= count;
                            }
                    );

                    auto chunk = stuff().create< chunk::ClientChunk >(
                            {
                                    .world = std::dynamic_pointer_cast< world::World >( stuff::Thing::self() ),
                                    .pos = pos,
                            }
                    );
                    auto& blocks = chunk->blocks();

                    for ( int i = 0; i < blocks.size(); i++ ) {
                        auto t = things.find( raw[ i ] );
                        if ( t == things.end() ) { blocks[ i ] = nullptr; }
                        else { blocks[ i ] = t->second; }
                    }

                    chunks()[ pos ] = chunk;
                    chunk->rasterize();
                }
        ).detach();
    }
}