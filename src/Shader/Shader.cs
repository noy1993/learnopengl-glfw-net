
using Silk.NET.OpenGLES;
using System.Numerics;

namespace OpenGL.Extension
{
    public class Shader : IDisposable
    {
        public static GL sgl { get; set; }
        public uint ID { get; private set; }
        public Shader(string vertexPath, string fragmentPath)
        {
            var vShaderCode = File.ReadAllText(vertexPath);
            var fShaderCode = File.ReadAllText(fragmentPath);

            var vertex = sgl.CreateShader(ShaderType.VertexShader);
            sgl.ShaderSource(vertex, vShaderCode);
            sgl.CompileShader(vertex);
            CheckCompileErrors(vertex, Type.VERTEX);

            var fragment = sgl.CreateShader(ShaderType.FragmentShader);
            sgl.ShaderSource(fragment, fShaderCode);
            sgl.CompileShader(fragment);
            CheckCompileErrors(fragment, Type.FRAGMENT);

            ID = sgl.CreateProgram();
            sgl.AttachShader(ID, vertex);
            sgl.AttachShader(ID, fragment);
            sgl.LinkProgram(ID);
            CheckCompileErrors(ID, Type.PROGRAM);

            sgl.DeleteShader(vertex);
            sgl.DeleteShader(fragment);
        }
        public void Use()
        {
            sgl.UseProgram(ID);
        }
        public void SetBool(string name, bool value)
        {
            SetInt(name, Convert.ToInt32(value));
        }
        public void SetFloat(string name, float value)
        {
            sgl.Uniform1(sgl.GetUniformLocation(ID, name), value);
        }
        public void SetInt(string name, int value)
        {
            sgl.Uniform1(sgl.GetUniformLocation(ID, name), value);
        }

        public unsafe void SetMatrix4x4(string name, Matrix4x4 value)
        {
            var index = sgl.GetUniformLocation(ID, name);
            sgl.UniformMatrix4(index, 1, false, (float*)&value);
        }

        private unsafe void CheckCompileErrors(uint shader, Type type)
        {
            int success;
            if (type != Type.PROGRAM)
            {
                sgl.GetShader(shader, ShaderParameterName.CompileStatus, out success);
                if (!Convert.ToBoolean(success))
                {
                    var info = sgl.GetShaderInfoLog(shader);
                    Console.WriteLine($"ERROR::SHADER_COMPILATION_ERROR of type: {type} \n{info}");
                    Console.WriteLine("---------------------------------------------------------");
                }
            }
            else
            {
                sgl.GetProgram(shader, ProgramPropertyARB.LinkStatus, &success);
                if (!Convert.ToBoolean(success))
                {
                    var info = sgl.GetProgramInfoLog(shader);
                    Console.WriteLine($"ERROR::PROGRAM_LINKING_ERROR  of type: {type} \n{info}");
                    Console.WriteLine("---------------------------------------------------------");
                }
            }
        }

        public void Dispose()
        {
            sgl.DeleteProgram(ID);
        }

        enum Type
        {
            VERTEX,
            FRAGMENT,
            PROGRAM
        }
    }
}