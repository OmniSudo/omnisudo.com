using Silk.NET.Maths;
using Silk.NET.OpenGL;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Processing;
using SkillQuest.Client.Engine.Graphics.API;

namespace SkillQuest.Client.Engine.Graphics.OpenGL;

public class GlTexture : IHasGl, ITexture, IDisposable {
    private uint _id;

    private Vector2D<uint> _size;

    public GlTexture ( GL gl, uint id, Vector2D<uint> size ){
        this.gl = gl;
        this._id = id;
        this._size = size;
    }

    public uint ID => this._id;

    public Vector2D<uint> Size => this._size;

    public void Dispose () {
        gl.DeleteTexture( _id );
    }

    public GL gl { get; }

    public void Bind(){
        gl.BindTexture(TextureTarget.Texture2D, this._id);
    }

    public void Unbind(){
        gl.BindTexture(TextureTarget.Texture2D, 0);
    }
}

public class GlTextureFactory : IDisposable {
	private List< ITexture > _textures = new ();

	public GlTextureFactory () {
	}

	public ITexture Build ( GL gl, string file ) {
        try {
            // Load the image using ImageSharp
            using Image<Rgba32> image = Image.Load<Rgba32>(file);

            // Flip the image vertically (OpenGL expects origin at the bottom-left)
            image.Mutate(x => x.Flip(FlipMode.Vertical));

            // Get image data
            var pixels = new byte[image.Width * image.Height * 4]; // 4 channels for Rgba32
            image.CopyPixelDataTo(pixels);

            // Generate a texture ID
            uint texture = gl.GenTexture();
            gl.BindTexture(TextureTarget.Texture2D, texture);

            // Set texture parameters (optional, customize as needed)
            gl.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)GLEnum.Linear);
            gl.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)GLEnum.Linear);
            gl.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapS, (int)GLEnum.Repeat);
            gl.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapT, (int)GLEnum.Repeat);

            // Upload the image data to OpenGL
            unsafe {
                fixed (void* pixelPtr = pixels) {
                    gl.TexImage2D(
                        TextureTarget.Texture2D,
                        0, // Mipmap level
                        (int)GLEnum.Rgba, // Internal format
                        (uint)image.Width, // Width
                        (uint)image.Height, // Height
                        0, // Border (must be 0)
                        GLEnum.Rgba, // Format of the pixel data
                        GLEnum.UnsignedByte, // Data type of pixel data
                        pixelPtr // Pointer to pixel data
                    );
                }
            }

            // Generate mipmaps (optional, if using mipmaps)
            //gl.GenerateMipmap(TextureTarget.Texture2D);

            // Unbind the texture
            gl.BindTexture(TextureTarget.Texture2D, 0);

            _textures.Add(new GlTexture(gl, texture, new Vector2D<uint>((uint)image.Width, (uint)image.Height)));
            return _textures.Last();
        } catch (Exception e) {
            throw new Exception( "Unable to create texture: " + file, e );
        }
    }

	public GlTextureFactory Clear () {
		foreach ( var texture in _textures ) {
			texture.Dispose();
		}
        _textures.Clear();
        
		return this;
	}

    public void Dispose(){
        Clear();
    }
}
