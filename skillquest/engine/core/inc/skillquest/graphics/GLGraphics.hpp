/**
 * @author omnisudo
 * @date 2024.02.22
 */

#pragma once

#include "IGraphics.hpp"
#include "skillquest/threadpool.hpp"
#include "Renderer.hpp"

namespace skillquest::graphics {
	class GLGraphics : public IGraphics {
	public:
		explicit GLGraphics( const IGraphics::CreateInfo& info );
		
		~GLGraphics() override;
		
		auto window() const -> IWindow & override;
		
		auto thread () -> threadpool::ThreadPool& override;
		
		auto update() -> void override;
		
		auto render() -> void override;
	
	private:
		std::shared_ptr< IWindow > _window = nullptr;
		
		std::shared_ptr< threadpool::ThreadPool > _threads = nullptr;
		
		std::shared_ptr< graphics::Renderer > _renderer = nullptr;
	
	public:
		class GLTextureFactory : public AssetFactory< ITexture > {
		public:
			~GLTextureFactory() override = default;
			
			std::shared_ptr< ITexture > get( const std::filesystem::path& path ) override;
			
			std::shared_ptr< ITexture > empty() override;
		};
		
		auto textures() -> AssetFactory <ITexture>& override;
		
		class GLMeshFactory : public AssetFactory< IMesh > {
		public:
			~GLMeshFactory() override = default;
			
			std::shared_ptr< IMesh > get( const std::filesystem::path& path ) override;
			
			std::shared_ptr< IMesh > empty() override;
		};
		
		auto meshes() -> AssetFactory <IMesh>& override;
		
		class GLShaderFactory : public AssetFactory< IShader > {
		public:
			~GLShaderFactory() override = default;
			
			std::shared_ptr< IShader > get( const std::filesystem::path& path ) override;
			
			std::shared_ptr< IShader > empty() override;
		};
		
		auto shaders() -> AssetFactory <IShader>& override;
		
	private:
		std::shared_ptr< GLTextureFactory > _textures = nullptr;
		
		std::shared_ptr< GLMeshFactory > _meshes = nullptr;
		
		std::shared_ptr< GLShaderFactory > _shaders = nullptr;
	};
}