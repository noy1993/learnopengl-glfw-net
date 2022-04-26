using Silk.NET.OpenGLES;
using SkiaSharp;
using System.Numerics;

namespace OpenGL.Extension
{
    public interface IMaterial
    {
        void SetMaterial(Shader shader, string name);
    }

    public class GLBitmap
    {
        public uint Texture { get; }
        public int TextureIndex => (int)TextureUnit - 33984;
        public TextureUnit TextureUnit { get; }

        static GL Gl;
        public GLBitmap(GL gl, string bitmap_path, TextureUnit unit)
        {
            Gl = gl;
            Texture = gl.GenTexture();
            TextureUnit = unit;

            gl.ActiveTexture(unit);//由于0号单元 默认是激活的，所以可以不写
            gl.BindTexture(TextureTarget.Texture2D, Texture);

            gl.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapS, (int)GLEnum.Repeat); // set texture wrapping to gl._REPEAT (default wrapping method)
            gl.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapT, (int)GLEnum.Repeat);

            // set texture filtering parameters
            gl.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)GLEnum.Linear);
            gl.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)GLEnum.Linear);

            using var bitmap = SKBitmap.Decode(bitmap_path);
            gl.TexImage2D(TextureTarget.Texture2D, 0, InternalFormat.Rgba, (uint)bitmap.Width, (uint)bitmap.Height, 0, PixelFormat.Bgra, PixelType.UnsignedByte, bitmap.GetPixelSpan());

            gl.GenerateMipmap(TextureTarget.Texture2D);
        }
    }

    public struct MaterialDiffuseMap : IMaterial
    {
        public GLBitmap Diffuse;
        public Vector3 Specular;
        public float Shininess;
        public void SetMaterial(Shader shader, string name)
        {
            shader.SetInt($"{name}.diffuse", Diffuse.TextureIndex);
            shader.SetVec3($"{name}.specular", Specular);
            shader.SetFloat($"{name}.shininess", Shininess);
        }
    }
    public struct MaterialMap : IMaterial
    {
        public GLBitmap Diffuse;
        public GLBitmap Specular;
        public float Shininess;
        public void SetMaterial(Shader shader, string name)
        {
            shader.SetInt($"{name}.diffuse", Diffuse.TextureIndex);
            shader.SetInt($"{name}.specular", Specular.TextureIndex);
            shader.SetFloat($"{name}.shininess", Shininess);
        }
    }

    public struct Material : IMaterial
    {
        public Vector3 Ambient;
        public Vector3 Diffuse;
        public Vector3 Specular;
        public float Shininess;

        public void SetMaterial(Shader shader, string name)
        {
            shader.SetVec3($"{name}.ambient", Ambient);
            shader.SetVec3($"{name}.diffuse", Diffuse);
            shader.SetVec3($"{name}.specular", Specular);
            shader.SetFloat($"{name}.shininess", Shininess);
        }
    }
}