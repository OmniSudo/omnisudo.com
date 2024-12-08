using System.Collections;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using Silk.NET.Maths;
using Silk.NET.OpenGL;
using SkillQuest.Client.Engine.Graphics.API;

namespace SkillQuest.Client.Engine.Graphics.OpenGL;

public class GlModule : IHasGl, IModule, IDisposable{
    private uint _id;

    private uint[] _shaderIDs;

    public GlModule(GL gl, uint program, uint[] ids){
        this.gl = gl;
        this._id = program;
        this._shaderIDs = ids;
    }

    public void Dispose(){
        foreach (var shader in _shaderIDs) {
            gl.DetachShader(_id, shader);
            gl.DeleteShader(shader);
        }

        gl.DeleteProgram(_id);

        _id = 0;
        _shaderIDs = null;
    }


    public uint ID => this._id;


    public GlModule Enable(String attributeName, uint attributeID){
        unsafe {
            var name = (byte*)Marshal.StringToHGlobalAnsi(attributeName);
            gl.BindAttribLocation(_id, attributeID, name);
            Marshal.FreeHGlobal((IntPtr)name);
            return this;
        }
    }

    public void Bind(){
        gl.UseProgram(_id);
    }

    public void Unbind(){
        gl.UseProgram(0);
    }

    public void Uniform(String name, int value){
        Uniform(location(name), value);
    }


    public void Uniform(String name, float value){
        Uniform(location(name), value);
    }


    public void Uniform(String name, bool value){
        Uniform(location(name), value);
    }


    public void Uniform(String name, Vector2D<int> value){
        Uniform(location(name), value);
    }


    public void Uniform(String name, Vector2D<float> value){
        Uniform(location(name), value);
    }


    public void Uniform(String name, Vector3D<int> value){
        Uniform(location(name), value);
    }


    public void Uniform(String name, Vector3D<float> value){
        Uniform(location(name), value);
    }


    public void Uniform(String name, Vector4D<int> value){
        Uniform(location(name), value);
    }


    public void Uniform(String name, Vector4D<float> value){
        Uniform(location(name), value);
    }


    public void Uniform(String name, Matrix3X3<float> value){
        Uniform(location(name), value);
    }


    public void Uniform(String name, Matrix4X4<float> value){
        Uniform(location(name), value);
    }


    public void Uniform(int id, int value){
        gl.Uniform1(id, value);
    }


    public void Uniform(int id, float value){
        gl.Uniform1(id, value);
    }


    public void Uniform(int id, double value){
        gl.Uniform1(id, value);
    }


    public void Uniform(int id, bool value){
        gl.Uniform1(id, value ? 1 : 0);
    }


    public void Uniform(int id, Vector2D<int> value){
        gl.Uniform2(id, value.X, value.Y);
    }


    public void Uniform(int id, Vector2D<float> value){
        gl.Uniform2(id, value.X, value.Y);
    }


    public void Uniform(int id, Vector2D<double> value){
        gl.Uniform2(id, value.X, value.Y);
    }


    public void Uniform(int id, Vector2D<uint> value){
        gl.Uniform2(id, value.X, value.Y);
    }

    public void Uniform(int id, Vector3D<int> value){
        gl.Uniform3(id, value.X, value.Y, value.Z);
    }


    public void Uniform(int id, Vector3D<float> value){
        gl.Uniform3(id, value.X, value.Y, value.Z);
    }


    public void Uniform(int id, Vector3D<double> value){
        gl.Uniform3(id, value.X, value.Y, value.Z);
    }


    public void Uniform(int id, Vector3D<uint> value){
        gl.Uniform3(id, value.X, value.Y, value.Z);
    }


    public void Uniform(int id, Vector4D<int> value){
        gl.Uniform4(id, value.X, value.Y, value.Z, value.W);
    }


    public void Uniform(int id, Vector4D<float> value){
        gl.Uniform4(id, value.X, value.Y, value.Z, value.W);
    }


    public void Uniform(int id, Vector4D<double> value){
        gl.Uniform4(id, value.X, value.Y, value.Z, value.W);
    }


    public void Uniform(int id, Vector4D<uint> value){
        gl.Uniform4(id, value.X, value.Y, value.Z, value.W);
    }

    public void Uniform(int id, Matrix3X3<float> value){
        // TODO: Verify
        gl.UniformMatrix3(id, false,
            MemoryMarshal.CreateReadOnlySpan(ref Unsafe.As<Matrix3X3<float>, float>(ref value), 9));
    }


    public void Uniform(int id, Matrix4X4<float> value){
        // TODO: Verify
        gl.UniformMatrix3(id, false,
            MemoryMarshal.CreateReadOnlySpan(ref Unsafe.As<Matrix4X4<float>, float>(ref value), 16));
    }

    private int location(String name){
        int location = gl.GetUniformLocation(_id, name);
        return location;
    }

    public GL gl { get; }
}

public class GlModuleFactory : IDisposable{
    List<IModule> _shaders = new();

    /// <summary>
    /// TODO: Compute shaders
    /// </summary>
    /// <param name="gl"></param>
    /// <param name="vertexShaderFile"></param>
    /// <param name="fragmentShaderFile"></param>
    /// <returns></returns>
    /// <exception cref="Exception"></exception>
    public IModule Build(GL gl, string vertexShaderFile, string fragmentShaderFile){
        try {
            unsafe {
                string vertexSource = File.ReadAllText(vertexShaderFile);
                string fragmentSource = File.ReadAllText(fragmentShaderFile);

                uint vertexShader = CompileShader(gl, ShaderType.VertexShader, vertexSource);
                uint fragmentShader = CompileShader(gl, ShaderType.FragmentShader, fragmentSource);

                // Link shaders into a program
                var program = gl.CreateProgram();
                gl.AttachShader(program, vertexShader);
                gl.AttachShader(program, fragmentShader);
                gl.LinkProgram(program);

                // Check for linking errors
                if (gl.GetProgram(program, GLEnum.LinkStatus) == 0) {
                    string infoLog = gl.GetProgramInfoLog(program);
                    throw new Exception($"Program linking failed: {infoLog}");
                }

                // Clean up shaders after linking
                gl.DetachShader(program, vertexShader);
                gl.DetachShader(program, fragmentShader);
                gl.DeleteShader(vertexShader);
                gl.DeleteShader(fragmentShader);
                
                var shader = new GlModule( gl, program, [ vertexShader, fragmentShader] );
                
                _shaders.Add(shader);
                return shader;
            }
        } catch (Exception e) {
            throw new Exception($"Failed to link program: {vertexShaderFile}\n{fragmentShaderFile}", e);
        }
        return null;
    }

    uint CompileShader(GL gl, ShaderType type, string source){
        // Create the shader object
        uint shader = gl.CreateShader(type);

        // Set the source code and compile
        gl.ShaderSource(shader, source);
        gl.CompileShader(shader);

        // Check for compilation errors
        if (gl.GetShader(shader, GLEnum.CompileStatus) == 0) {
            string infoLog = gl.GetShaderInfoLog(shader);
            throw new Exception($"{type} shader compilation failed: {infoLog}");
        }

        return shader;
    }

    public void Dispose(){
        foreach (var shader in _shaders) {
            shader.Dispose();
        }
        _shaders.Clear();
    }
}
