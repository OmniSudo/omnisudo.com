using System.Runtime.InteropServices;
using Silk.NET.Assimp;
using Silk.NET.OpenGL;
using SkillQuest.Client.Engine.Graphics.API;
using PrimitiveType = Silk.NET.OpenGL.PrimitiveType;

namespace SkillQuest.Client.Engine.Graphics.OpenGL;

public class GlSurface : IHasGl, ISurface, IDisposable{
    uint _vao;

    uint[] _vbos;

    uint _count;

    public GlSurface(GL gl, uint vao, List<uint> vbos, uint indexCount){
        this.gl = gl;
        _vao = vao;
        _vbos = vbos.ToArray();
        _count = indexCount;
    }

    public uint VertexArrayObject => _vao;

    public uint[] VertexBufferObjects => _vbos;

    public uint IndexCount => _count;

    public void Dispose(){
        gl.DeleteVertexArray(VertexArrayObject);

        foreach (var vbo in VertexBufferObjects) {
            gl.DeleteBuffer(vbo);
        }
        _vao = 0;
        _vbos = null;
        _count = 0;
    }

    public void Draw(){
        unsafe {
            gl.BindVertexArray(_vao);
            gl.DrawElements( PrimitiveType.Triangles, _count, DrawElementsType.UnsignedInt, (void*) 0 );
        }
    }

    public GL gl { get; }
}

public class GlSurfaceFactory : IDisposable{
    readonly Assimp _assimp;

    public GlSurfaceFactory(){
        _assimp = Assimp.GetApi();
    }

    /// <summary>
    /// TODO: Verify
    /// </summary>
    /// <param name="gl"></param>
    /// <param name="file"></param>
    /// <returns></returns>
    /// <exception cref="Exception"></exception>
    public ISurface Build(GL gl, string file){
        unsafe {
            Scene* scene = _assimp.ImportFile(file, (uint)(PostProcessSteps.Triangulate | PostProcessSteps.FlipUVs));

            if (scene == null || scene->MFlags == (uint)SceneFlags.Incomplete) {
                throw new Exception($"Failed to load GLTF file: {_assimp.GetErrorStringS()}");
            }

            // Assume the first mesh for simplicity
            Mesh* mesh = scene->MMeshes[0];

            // Extract vertex and index data
            var vertices = new float[mesh->MNumVertices * 8]; // 3 (pos) + 3 (normals) + 2 (tex coords)

            for (int i = 0; i < mesh->MNumVertices; i++) {
                // Position
                vertices[i * 8 + 0] = mesh->MVertices[i].X;
                vertices[i * 8 + 1] = mesh->MVertices[i].Y;
                vertices[i * 8 + 2] = mesh->MVertices[i].Z;

                // Normal
                vertices[i * 8 + 3] = mesh->MNormals[i].X;
                vertices[i * 8 + 4] = mesh->MNormals[i].Y;
                vertices[i * 8 + 5] = mesh->MNormals[i].Z;

                // Texture coordinates (if available)
                if (mesh->MTextureCoords[0] != null) {
                    vertices[i * 8 + 6] = mesh->MTextureCoords[0][i].X;
                    vertices[i * 8 + 7] = mesh->MTextureCoords[0][i].Y;
                } else {
                    vertices[i * 8 + 6] = 0.0f;
                    vertices[i * 8 + 7] = 0.0f;
                }
            }

            var indices = new uint[mesh->MNumFaces * 3]; // Assume triangles

            for (int i = 0; i < mesh->MNumFaces; i++) {
                Face face = mesh->MFaces[i];

                for (int j = 0; j < face.MNumIndices; j++) {
                    indices[i * 3 + j] = face.MIndices[j];
                }
            }

            // Clean up the Assimp scene
            _assimp.ReleaseImport(scene);

            // Create VAO, VBO, and EBO
            uint vao = gl.GenVertexArray();
            uint vbo = gl.GenBuffer();
            uint ebo = gl.GenBuffer();

            gl.BindVertexArray(vao);

            // VBO: Upload vertex data
            gl.BindBuffer(BufferTargetARB.ArrayBuffer, vbo);

            fixed (float* verts = vertices) {
                gl.BufferData(BufferTargetARB.ArrayBuffer, (nuint)(vertices.Length * sizeof(float)),
                    verts,
                    BufferUsageARB.StaticDraw);
            }

            // EBO: Upload index data
            gl.BindBuffer(BufferTargetARB.ElementArrayBuffer, ebo);
            

            fixed (uint* inds = indices) {
                gl.BufferData(BufferTargetARB.ElementArrayBuffer, (nuint)(indices.Length * sizeof(uint)),
                    inds,
                    BufferUsageARB.StaticDraw);
            }

            // Specify vertex attribute pointers
            // Position (location = 0)
            gl.VertexAttribPointer(0, 3, GLEnum.Float, false, 8 * sizeof(float), (void*)0);
            gl.EnableVertexAttribArray(0);

            // Normal (location = 1)
            gl.VertexAttribPointer(1, 3, GLEnum.Float, false, 8 * sizeof(float), (void*)(3 * sizeof(float)));
            gl.EnableVertexAttribArray(1);

            // Texture coordinates (location = 2)
            gl.VertexAttribPointer(2, 2, GLEnum.Float, false, 8 * sizeof(float), (void*)(6 * sizeof(float)));
            gl.EnableVertexAttribArray(2);

            // Unbind the VAO
            gl.BindVertexArray(0);

            var surface = new GlSurface(gl, vao, [vbo, ebo], (uint)indices.Length);
            _surfaces.Add(surface);
            return surface;
        }
    }

    List<ISurface> _surfaces = new();

    /// <summary>
    /// TODO Verify
    /// </summary>
    /// <param name="gl"></param>
    /// <param name="vertices"></param>
    /// <param name="indices"></param>
    /// <returns></returns>
    public ISurface Build(GL gl, Vertex[] vertices, uint[] indices){
        unsafe {
            uint vao = gl.GenVertexArray();
            uint vbo = gl.GenBuffer();
            uint ebo = gl.GenBuffer();

            gl.BindVertexArray(vao);

            // VBO: Upload vertex data
            gl.BindBuffer(BufferTargetARB.ArrayBuffer, vbo);

            gl.BufferData(BufferTargetARB.ArrayBuffer, (nuint)(vertices.Length * sizeof(float)), &vertices,
                BufferUsageARB.StaticDraw);

            // EBO: Upload index data
            gl.BindBuffer(BufferTargetARB.ElementArrayBuffer, ebo);

            gl.BufferData(BufferTargetARB.ElementArrayBuffer, (nuint)(indices.Length * sizeof(uint)), &indices,
                BufferUsageARB.StaticDraw);

            // Specify vertex attribute pointers
            // Position (location = 0)
            gl.VertexAttribPointer(0, 3, GLEnum.Float, false, 8 * sizeof(float), (void*)0);
            gl.EnableVertexAttribArray(0);

            // Normal (location = 1)
            gl.VertexAttribPointer(1, 3, GLEnum.Float, false, 8 * sizeof(float), (void*)(3 * sizeof(float)));
            gl.EnableVertexAttribArray(1);

            // Texture coordinates (location = 2)
            gl.VertexAttribPointer(2, 2, GLEnum.Float, false, 8 * sizeof(float), (void*)(6 * sizeof(float)));
            gl.EnableVertexAttribArray(2);

            // Unbind the VAO
            gl.BindVertexArray(0);

            var surface = new GlSurface(gl, vao, [vbo, ebo], (uint)vertices.Length);
            _surfaces.Add(surface);
            return surface;
        }
    }

    public void Dispose(){
        foreach (var surface in _surfaces) {
            surface.Dispose();
        }
        _surfaces.Clear();
    }
}
