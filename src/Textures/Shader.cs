
using Silk.NET.OpenGLES;
using System;
using System.IO;

namespace Shaders
{
    class Shader : IDisposable
    {
        static GL GL = Textures.Program.gl;
        public uint ID { get; private set; }
        public Shader(string vertexPath, string fragmentPath)
        {
            var vShaderCode = File.ReadAllText(vertexPath);
            var fShaderCode = File.ReadAllText(fragmentPath);

            var vertex = GL.CreateShader(ShaderType.VertexShader);
            GL.ShaderSource(vertex, vShaderCode);
            GL.CompileShader(vertex);
            CheckCompileErrors(vertex, Type.VERTEX);

            var fragment = GL.CreateShader(ShaderType.FragmentShader);
            GL.ShaderSource(fragment, fShaderCode);
            GL.CompileShader(fragment);
            CheckCompileErrors(fragment, Type.FRAGMENT);

            ID = GL.CreateProgram();
            GL.AttachShader(ID, vertex);
            GL.AttachShader(ID, fragment);
            GL.LinkProgram(ID);
            CheckCompileErrors(ID, Type.PROGRAM);

            GL.DeleteShader(vertex);
            GL.DeleteShader(fragment);
        }
        public void Use()
        {
            GL.UseProgram(ID);
        }
        public void SetBool(string name, bool value)
        {
            SetInt(name, Convert.ToInt32(value));
        }
        public void SetFloat(string name, float value)
        {
            GL.Uniform1(GL.GetUniformLocation(ID, name), value);
        }
        public void SetInt(string name, int value)
        {
            GL.Uniform1(GL.GetUniformLocation(ID, name), value);
        }

        private unsafe void CheckCompileErrors(uint shader, Type type)
        {
            int success;
            if (type != Type.PROGRAM)
            {
                GL.GetShader(shader, ShaderParameterName.CompileStatus, out success);
                if (!Convert.ToBoolean(success))
                {
                    var info = GL.GetShaderInfoLog(shader);
                    Console.WriteLine($"ERROR::SHADER_COMPILATION_ERROR of type: {type} \n{info}");
                    Console.WriteLine("---------------------------------------------------------");
                }
            }
            else
            {
                GL.GetProgram(shader, ProgramPropertyARB.LinkStatus, &success);
                if (!Convert.ToBoolean(success))
                {
                    var info = GL.GetProgramInfoLog(shader);
                    Console.WriteLine($"ERROR::PROGRAM_LINKING_ERROR  of type: {type} \n{info}");
                    Console.WriteLine("---------------------------------------------------------");
                }
            }
        }

        public void Dispose()
        {
            GL.DeleteProgram(ID);
        }

        enum Type
        {
            VERTEX,
            FRAGMENT,
            PROGRAM
        }
    }
}
