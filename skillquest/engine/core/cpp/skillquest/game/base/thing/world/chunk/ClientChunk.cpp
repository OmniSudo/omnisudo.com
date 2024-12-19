/**
 * @author omnisudo
 * @date 2024.08.31
 */

#include "ClientChunk.hpp"
#include "skillquest/game/base/thing/block/ClientBlock.hpp"
#include "skillquest/direction.hpp"
#include "skillquest/sh.api.hpp"
#include <map>

namespace skillquest::game::base::thing::world::chunk {
    ClientChunk::ClientChunk(
        const CreateInfo &info
    ) : Chunk(info) {
    }

    void ClientChunk::rasterize() {
        auto &blocks = this->blocks();

        auto sorted = std::map<
            thing::block::ClientBlock *,
            std::vector<
                glm::u8vec3
            >
        >{};

        auto vertices = std::map<
            thing::block::IBlock *,
            std::map<
                math::Direction,
                std::vector<math::Vertex>
            >
        >{};

        auto indexes = std::map<
            thing::block::IBlock *,
            std::map<
                math::Direction,
                std::vector<unsigned int>
            >
        >{};

        auto textures = std::map<
            thing::block::IBlock *,
            std::map<
                math::Direction,
                std::shared_ptr<graphics::ITexture>
            >
        >{};

        for (int i = 0; i < blocks.size(); i++) {
            auto pos = glm::u8vec3{
                (i >> 0) & 0xF,
                (i >> 4) & 0xF,
                (i >> 8) & 0xF,
            };

            auto block = dynamic_cast<thing::block::ClientBlock *>(blocks[i]);
            if (!sorted.contains(block)) { sorted.emplace(block, std::vector<glm::u8vec3>{}); }

            sorted[block].push_back(pos);
        }

        for (auto &pair: sorted) {
            auto block = pair.first;

            vertices.emplace(
                block,
                std::map<
                    math::Direction,
                    std::vector<math::Vertex>
                >{}
            );
            indexes.emplace(
                block,
                std::map<
                    math::Direction,
                    std::vector<unsigned int>
                >{}
            );
            textures.emplace(
                block,
                std::map<
                    math::Direction,
                    std::shared_ptr<graphics::ITexture>
                >{}
            );
            auto &v = vertices[block];
            auto &i = indexes[block];
            auto &t = textures[block];

            for (auto pos: pair.second) {
                if (!block) continue;
                for (auto &[dir, surface]: block->vertexes()) {
                    auto &vd = v[dir];

                    auto offset = vd.size();
                    for (auto &vert: surface) {
                        vd.push_back({
                            .position = vert.position + glm::vec3{pos.x, pos.y, pos.z},
                            .uv = vert.uv,
                            .color = vert.color,
                            .normal = vert.normal
                        });
                    }

                    for (auto &index: block->indexes().contains(dir)
                                          ? block->indexes()[dir]
                                          : std::vector<unsigned int>{}) {
                        i[dir].push_back(index + offset);
                    }

                    t[dir] = block->textures().contains(dir) ? block->textures()[dir] : nullptr;
                }
            }
        }
    }
}
