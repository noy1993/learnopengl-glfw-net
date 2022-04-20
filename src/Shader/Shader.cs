
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

    public class Camera
    {
        const float YAW = -90.0f;
        const float PITCH = 0.0f;
        const float SPEED = 2.5f;
        const float SENSITIVITY = 0.1f;
        const float ZOOM = 45.0f;

        public Vector3 Pos { get; set; }
        public Vector3 Front { get; set; }
        public Vector3 WorldUp { get; set; }

        public Vector3 Right => Vector3.Normalize(Vector3.Cross(Front, WorldUp));
        public Vector3 Up => Vector3.Normalize(Vector3.Cross(Right, Front));

        public float Yaw { get; set; }
        public float Pitch { get; set; }
        public float MovementSpeed { get; set; }
        public float Zoom { get; set; }
        public float MouseSensitivity { get; set; }
        public Camera()
        {
            Pos = new Vector3(0, 0, 0);
            Front = new Vector3(0, 0, -1);
            WorldUp = new Vector3(0, 1, 0);

            Yaw = YAW;
            Pitch = PITCH;

            MovementSpeed = SPEED;
            MouseSensitivity = SENSITIVITY;
            Zoom = ZOOM;
        }

        public Camera(float posX, float posY, float posZ, float upX, float upY, float upZ, float yaw, float pitch)
        {
            Pos = new Vector3(posX, posY, posZ);
            Front = new Vector3(0, 0, -1);
            WorldUp = new Vector3(upX, upY, upZ);

            Yaw = yaw;
            Pitch = pitch;

            MovementSpeed = SPEED;
            MouseSensitivity = SENSITIVITY;
            Zoom = ZOOM;
        }

        public void ProcessKeyboard(float xoffset, float yoffset, bool constrainPitch = true)
        {
            xoffset *= MouseSensitivity;
            yoffset *= MouseSensitivity;

            Yaw += xoffset;
            Pitch += yoffset;

            if (constrainPitch)
            {
                if (Pitch > 89.0f) Pitch = 89.0f;
                else if (Pitch < -89.0f) Pitch = -89.0f;
            }
        }

        public void ProcessMouseScroll(float yoffset)
        {
            if (Zoom < 1.0f)
            {
                Zoom = 1.0f;
            }
            else if (Zoom > 45.0f)
            {
                Zoom = 45.0f;
            }
            else
            {
                Zoom -= yoffset;
            }
        }


    }
}