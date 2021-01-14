using System;
using System.IO;
using System.Numerics;
using static OpenGL.Gl;

namespace Shaders
{
    class Shader :IDisposable
    {
        public uint ID { get;private set; }
        public Shader(string vertexPath,string fragmentPath)
        {
            var vShaderCode = File.ReadAllText(vertexPath);
            var fShaderCode = File.ReadAllText(fragmentPath);

            var vertex = glCreateShader(GL_VERTEX_SHADER);
            glShaderSource(vertex, vShaderCode);
            glCompileShader(vertex);
            CheckCompileErrors(vertex, Type.VERTEX);

            var fragment = glCreateShader(GL_FRAGMENT_SHADER);
            glShaderSource(fragment, fShaderCode);
            glCompileShader(fragment);
            CheckCompileErrors(fragment, Type.FRAGMENT);

            ID = glCreateProgram();
            glAttachShader(ID, vertex);
            glAttachShader(ID, fragment);
            glLinkProgram(ID);
            CheckCompileErrors(ID, Type.PROGRAM);

            glDeleteShader(vertex);
            glDeleteShader(fragment);
        }
        public void Use()
        {
            glUseProgram(ID);
        }
        public void SetBool(string name, bool value)
        {
            SetInt(name, Convert.ToInt32(value));
        }
        public void SetFloat(string name, float value)
        {
            glUniform1f(glGetUniformLocation(ID, name), value);
        }
        public void SetInt(string name, int value)
        {
            glUniform1i(glGetUniformLocation(ID, name), value);
        }
        public unsafe void SetMatrix4x4(string name, Matrix4x4 value)
        {
            glUniformMatrix4fv(glGetUniformLocation(ID, name),1,false, (float*)&value);
        }

        private unsafe void CheckCompileErrors(uint shader, Type type)
        {
            int success;
            if (type != Type.PROGRAM)
            {
                glGetShaderiv(shader, GL_COMPILE_STATUS, &success);
                if (!Convert.ToBoolean(success))
                {
                    var info = glGetShaderInfoLog(shader);
                    Console.WriteLine($"ERROR::SHADER_COMPILATION_ERROR of type: {type} \n{info}");
                    Console.WriteLine("---------------------------------------------------------");
                }
            }
            else
            {
                glGetProgramiv(shader, GL_LINK_STATUS, &success);
                if (!Convert.ToBoolean(success))
                {
                    var info = glGetProgramInfoLog(shader);
                    Console.WriteLine($"ERROR::PROGRAM_LINKING_ERROR  of type: {type} \n{info}");
                    Console.WriteLine("---------------------------------------------------------");
                }
            }
        }

        public void Dispose()
        {
            glDeleteProgram(ID);
        }

        enum Type
        {
            VERTEX,
            FRAGMENT,
            PROGRAM
        }
    }
}
