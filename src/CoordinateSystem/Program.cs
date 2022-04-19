using Silk.NET.GLFW;
using Silk.NET.OpenGLES;
using SkiaSharp;
using System.Numerics;

namespace Transformations
{
    class Program
    {
        static readonly Glfw GLFW = Glfw.GetApi();
        internal static GL gl;

        public static void Main()
        {
            Project();
        }

        public unsafe static void Project()
        {
            GLFW.Init();

            var window = GLFW.CreateWindow(800, 600, "LearnOpenGL", null, null);
            if (window == null)
            {
                Console.WriteLine("Failed to create GLFW window");
                GLFW.Terminate();
            }
            GLFW.MakeContextCurrent(window);
            GLFW.SetFramebufferSizeCallback(window, framebuffer_size_callback);
            gl = GL.GetApi(new GlfwContext(GLFW, window));

            OpenGL.Extension.Shader.sgl = gl;
            var shader = new OpenGL.Extension.Shader("./coordinate_systems.vert", "./coordinate_systems.frag");

            // activate shader
            shader.Use();

            var vertices = new[]
            {
                // positions         // texture coords
                 0.5f,  0.5f, 0.0f,  1.0f, 1.0f, // top right
                 0.5f, -0.5f, 0.0f,  1.0f, 0.0f, // bottom right
                -0.5f, -0.5f, 0.0f,  0.0f, 0.0f, // bottom left
                -0.5f,  0.5f, 0.0f,  0.0f, 1.0f  // top left 
            };
            var indices = new uint[]{
                0, 1, 3, // first triangle
                1, 2, 3  // second triangle
            };

            //data
            var vao = gl.GenBuffer();
            gl.BindVertexArray(vao);

            //send buffer
            var vbo = gl.GenBuffer();
            gl.BindBuffer(BufferTargetARB.ArrayBuffer, vbo);
            fixed (float* v = vertices)
            {
                gl.BufferData(BufferTargetARB.ArrayBuffer, (nuint)(vertices.Length * sizeof(float)), v, BufferUsageARB.StaticDraw);
            }

            gl.VertexAttribPointer(0, 3, VertexAttribPointerType.Float, false, 5 * sizeof(float), null);
            gl.EnableVertexAttribArray(0);

            gl.VertexAttribPointer(1, 2, VertexAttribPointerType.Float, false, 5 * sizeof(float), (void*)(3 * sizeof(float)));
            gl.EnableVertexAttribArray(1);

            //index
            var ebo = gl.GenBuffer();
            gl.BindBuffer(BufferTargetARB.ElementArrayBuffer, ebo);
            fixed (uint* v = indices)
            {
                gl.BufferData(BufferTargetARB.ElementArrayBuffer, (nuint)(indices.Length * sizeof(uint)), v, BufferUsageARB.StaticDraw);
            }

            var texture = gl.GenTexture();
            gl.ActiveTexture(TextureUnit.Texture0);
            gl.BindTexture(TextureTarget.Texture2D, texture);

            gl.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapS, (int)GLEnum.Repeat);
            gl.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapS, (int)GLEnum.Repeat);

            gl.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)GLEnum.LinearMipmapLinear);
            gl.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)GLEnum.Linear);

            using var bitmap = SKBitmap.Decode("./container.jpg");
            gl.TexImage2D(TextureTarget.Texture2D, 0, InternalFormat.Rgba, (uint)bitmap.Width, (uint)bitmap.Height, 0, PixelFormat.Bgra, PixelType.UnsignedByte, bitmap.GetPixelSpan());
            gl.GenerateMipmap(TextureTarget.Texture2D);

            //变换矩阵
            var model = Matrix4x4.CreateRotationX(-55f, new Vector3(1f, 1f, 1f));
            var view = Matrix4x4.CreateTranslation(new Vector3(0f, 0f, -3f));
            var projection = Matrix4x4.CreatePerspective(800, 600, 0.1f, 100f);

            var Loc1 = gl.GetUniformLocation(shader.ID, "model");
            var Loc2 = gl.GetUniformLocation(shader.ID, "view");
            var Loc3 = gl.GetUniformLocation(shader.ID, "project");


            while (!GLFW.WindowShouldClose(window))
            {
                gl.ClearColor(0.2f, 0.3f, 0.3f, 1.0f);
                gl.Clear(ClearBufferMask.ColorBufferBit);

                //将矩阵传入着色器
                //shader.SetMatrix4x4("model", model);
                //shader.SetMatrix4x4("view", projection);
                //shader.SetMatrix4x4("project", view);

                gl.DrawElements(PrimitiveType.Triangles, (uint)indices.Length, DrawElementsType.UnsignedInt, null);

                GLFW.SwapBuffers(window);
                GLFW.PollEvents();
            }
        }

        private static unsafe void framebuffer_size_callback(WindowHandle* window, int width, int height)
        {
            gl.Viewport(0, 0, (uint)width, (uint)height);
        }
    }
}