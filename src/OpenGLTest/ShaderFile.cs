using Silk.NET.OpenGLES;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace OpenGLTest
{
    public class ShaderFile
    {
        private readonly GL gl;
        public uint ID { get; private set; }
        public ShaderFile(GL gl, string vertexPath, string fragmentPath)
        {
            this.gl = gl;
            var vShaderCode = File.ReadAllText(vertexPath);
            var fShaderCode = File.ReadAllText(fragmentPath);

            var vertex = this.gl.CreateShader(ShaderType.VertexShader);
            this.gl.ShaderSource(vertex, vShaderCode);
            this.gl.CompileShader(vertex);
            CheckCompileErrors(vertex, Type.VERTEX);

            var fragment = this.gl.CreateShader(ShaderType.FragmentShader);
            this.gl.ShaderSource(fragment, fShaderCode);
            this.gl.CompileShader(fragment);
            CheckCompileErrors(fragment, Type.FRAGMENT);

            ID = this.gl.CreateProgram();
            this.gl.AttachShader(ID, vertex);
            this.gl.AttachShader(ID, fragment);
            this.gl.LinkProgram(ID);
            CheckCompileErrors(ID, Type.PROGRAM);

            this.gl.DeleteShader(vertex);
            this.gl.DeleteShader(fragment);
        }
        public void Use()
        {
            gl.UseProgram(ID);
        }
        public void SetBool(string name, bool value)
        {
            SetInt(name, Convert.ToInt32(value));
        }
        public void SetFloat(string name, float value)
        {
            gl.Uniform1(gl.GetUniformLocation(ID, name), value);
        }
        public void SetInt(string name, int value)
        {
            gl.Uniform1(gl.GetUniformLocation(ID, name), value);
        }
        public void SetVec3(string name, Vector3 value)
        {
            gl.Uniform3(gl.GetUniformLocation(ID, name), value);
        }

        public unsafe void SetMatrix4x4(string name, Matrix4x4 value)
        {
            var index = gl.GetUniformLocation(ID, name);
            gl.UniformMatrix4(index, 1, false, (float*)&value);
        }

        //public unsafe void SetMaterial(string name, IMaterial value)
        //{
        //    value.SetMaterial(this, name);
        //}

        //public void SetMaterial(string name, IMaterial value, int index)
        //{
        //    value.SetMaterial(this, $"{name}[{index}]");
        //}

        //public void SetMaterialArray(string name, IEnumerable<IMaterial> value)
        //{
        //    int index = 0;
        //    foreach (var material in value)
        //    {
        //        material.SetMaterial(this, $"{name}[{index}]");
        //        index++;
        //    }
        //}

        //public unsafe void SetLight(string name, ILight value)
        //{
        //    value.SetLight(this, name);
        //}
        //public void SetLight(string name, ILight value, int index)
        //{
        //    value.SetLight(this, $"{name}[{index}]");
        //}

        //public void SetPointLights(string name, IEnumerable<PointLight> value)
        //{
        //    int index = 0;
        //    foreach (var material in value)
        //    {
        //        material.SetLight(this, $"{name}[{index}]");
        //        index++;
        //    }
        //}

        private unsafe void CheckCompileErrors(uint shader, Type type)
        {
            int success;
            if (type != Type.PROGRAM)
            {
                gl.GetShader(shader, ShaderParameterName.CompileStatus, out success);
                if (!Convert.ToBoolean(success))
                {
                    var info = gl.GetShaderInfoLog(shader);
                    Console.WriteLine($"ERROR::SHADER_COMPILATION_ERROR of type: {type} \n{info}");
                    Console.WriteLine("---------------------------------------------------------");
                }
            }
            else
            {
                gl.GetProgram(shader, ProgramPropertyARB.LinkStatus, &success);
                if (!Convert.ToBoolean(success))
                {
                    var info = gl.GetProgramInfoLog(shader);
                    Console.WriteLine($"ERROR::PROGRAM_LINKING_ERROR  of type: {type} \n{info}");
                    Console.WriteLine("---------------------------------------------------------");
                }
            }
        }

        public void Delete()
        {
            gl.DeleteProgram(ID);
        }

        enum Type
        {
            VERTEX,
            FRAGMENT,
            PROGRAM
        }
    }
}
