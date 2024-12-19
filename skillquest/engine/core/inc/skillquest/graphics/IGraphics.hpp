/**
 * @author omnisudo
 * @date 2024.02.22
 */

#pragma once

#include "skillquest/string.hpp"
#include "skillquest/threadpool.hpp"
#include "IWindow.hpp"
#include "ITexture.hpp"
#include "IShader.hpp"
#include "Material.hpp"
#include "IMesh.hpp"
#include <filesystem>
#include "glm/vec2.hpp"

namespace skillquest::graphics {
	class IGraphics {
	public:
		struct CreateInfo {
			std::string title = "SkillQuest";
			
			glm::uvec2 size = { 1280, 720 };
		};
		
		virtual ~IGraphics() = default;
	
		virtual auto window () const -> IWindow& = 0;
		
		virtual auto thread () -> threadpool::ThreadPool& = 0;
		
		virtual auto update () -> void = 0;
		
		virtual auto render () -> void = 0;
		
	public:
		template< typename TAsset >
		class AssetFactory {
		public:
			virtual ~AssetFactory() = default;
			
			virtual std::shared_ptr< TAsset > get ( const std::filesystem::path& path ) = 0;
			
			virtual std::shared_ptr< TAsset > empty () = 0;
		};
		
	public:
		virtual auto textures () -> AssetFactory< ITexture >& = 0;
		
		virtual auto meshes() -> AssetFactory< IMesh >& = 0;
		
		virtual auto shaders () -> AssetFactory< IShader >& = 0;
	};
}