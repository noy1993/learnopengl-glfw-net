using Silk.NET.OpenGLES;
using System.Numerics;

namespace Colors
{
    class Program
    {
        public static void Main()
        {
            var coral = new Vector3(1f, 0.5f, 0.31f);//创建一个 珊瑚红(Coral)色


            var lightColor = new Vector3(1f, 1f, 1f);
            var toyColor = coral;
            var result = lightColor * toyColor;


            OpenGL.Extension.Shader shader = new OpenGL.Extension.Shader("./colors.vert", "./colors.frag");

            shader.Use();

            shader.SetVec3("objectColor", toyColor);
            shader.SetVec3("lightColor", lightColor);

        }
    }
}
