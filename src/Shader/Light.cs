using System.Numerics;

namespace OpenGL.Extension
{
    public interface ILight
    {
        void SetLight(Shader shader, string name);
    }

    public struct PointLight : ILight
    {
        public Vector3 Position;

        public float Constant;
        public float Linear;
        public float Quadratic;

        public Vector3 Ambient;
        public Vector3 Diffuse;
        public Vector3 Specular;

        public void SetLight(Shader shader, string name)
        {
            shader.SetVec3($"{name}.ambient", Ambient);
            shader.SetVec3($"{name}.diffuse", Diffuse);
            shader.SetVec3($"{name}.specular", Specular);
            shader.SetVec3($"{name}.position", Position);

            shader.SetFloat($"{name}.constant", Constant);
            shader.SetFloat($"{name}.linear", Linear);
            shader.SetFloat($"{name}.quadratic", Quadratic);
        }
    }

    public struct DirLight : ILight
    {
        public Vector3 Direction;

        public Vector3 Ambient;
        public Vector3 Diffuse;
        public Vector3 Specular;
        public void SetLight(Shader shader, string name)
        {
            shader.SetVec3($"{name}.ambient", Ambient);
            shader.SetVec3($"{name}.diffuse", Diffuse);
            shader.SetVec3($"{name}.specular", Specular);
            shader.SetVec3($"{name}.direction", Direction);
        }
    }

    public struct SpotLight : ILight
    {
        public Vector3 Position;
        public Vector3 Direction;

        public float CutOff;
        public float OuterCutOff;

        public float Constant;
        public float Linear;
        public float Quadratic;

        public Vector3 Ambient;
        public Vector3 Diffuse;
        public Vector3 Specular;

        public void SetLight(Shader shader, string name)
        {
            shader.SetVec3($"{name}.ambient", Ambient);
            shader.SetVec3($"{name}.diffuse", Diffuse);
            shader.SetVec3($"{name}.specular", Specular);
            shader.SetVec3($"{name}.position", Position);
            shader.SetVec3($"{name}.direction", Direction);

            shader.SetFloat($"{name}.cutOff", CutOff);
            shader.SetFloat($"{name}.outerCutOff", OuterCutOff);

            shader.SetFloat($"{name}.constant", Constant);
            shader.SetFloat($"{name}.linear", Linear);
            shader.SetFloat($"{name}.quadratic", Quadratic);
        }
    }

    public struct Light : ILight
    {
        public Vector3 Position;
        public Vector3 Ambient;
        public Vector3 Diffuse;
        public Vector3 Specular;

        public void SetLight(Shader shader, string name)
        {
            shader.SetVec3($"{name}.ambient", Ambient);
            shader.SetVec3($"{name}.diffuse", Diffuse);
            shader.SetVec3($"{name}.specular", Specular);
            shader.SetVec3($"{name}.position", Position);
        }
    }
}