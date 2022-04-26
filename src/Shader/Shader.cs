using Silk.NET.OpenGLES;
using System.Numerics;

namespace OpenGL.Extension
{
    public class Shader
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
        public void SetVec3(string name, Vector3 value)
        {
            sgl.Uniform3(sgl.GetUniformLocation(ID, name), value);
        }

        public unsafe void SetMatrix4x4(string name, Matrix4x4 value)
        {
            var index = sgl.GetUniformLocation(ID, name);
            sgl.UniformMatrix4(index, 1, false, (float*)&value);
        }

        public unsafe void SetMaterial(string name, IMaterial value)
        {
            value.SetMaterial(this, name);
        }

        public unsafe void SetLight(string name, Light value)
        {
            SetVec3($"{name}.ambient", value.Ambient);
            SetVec3($"{name}.diffuse", value.Diffuse);
            SetVec3($"{name}.specular", value.Specular);
            SetVec3($"{name}.position", value.Position);
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

        public void Delete()
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