using Silk.NET.GLFW;
using Silk.NET.OpenGLES;
using SkiaSharp;
using System;
using System.Drawing;
using System.Numerics;
using System.Runtime.InteropServices;

namespace Transformations
{
    class Program
    {
        static readonly Glfw GLFW = Glfw.GetApi();
        internal static GL gl;

        static void Main(string[] args)
        {
            P1();
        }
        static unsafe void P1()
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

            Shaders.Shader shader = new Shaders.Shader("./texture.vert", "./texture.frag");
            var vertices = new[]
            {
                // positions        // texture coords
                 0.5f,  0.5f, 0.0f, 1.0f, 1.0f, // top right
                 0.5f, -0.5f, 0.0f, 1.0f, 0.0f, // bottom right
                -0.5f, -0.5f, 0.0f, 0.0f, 0.0f, // bottom left
                -0.5f,  0.5f, 0.0f, 0.0f, 1.0f  // top left 
            };
            var indices = new uint[]{
                0, 1, 3, // first triangle
                1, 2, 3  // second triangle
            };

            //绑定 VAO 与 VBO
            uint VAO = gl.GenVertexArray();
            uint VBO = gl.GenBuffer();
            uint EBO = gl.GenBuffer();

            gl.BindVertexArray(VAO);

            //把顶点数组复制到缓冲中供OpenGL使用
            gl.BindBuffer(BufferTargetARB.ArrayBuffer, VBO);
            fixed (float* v = vertices)
            {
                gl.BufferData(BufferTargetARB.ArrayBuffer, (nuint)(vertices.Length * sizeof(float)), v, BufferUsageARB.StaticDraw);
            }

            gl.BindBuffer(BufferTargetARB.ElementArrayBuffer, EBO);
            fixed (uint* v = indices)
            {
                gl.BufferData(BufferTargetARB.ElementArrayBuffer, (nuint)(indices.Length * sizeof(uint)), v, BufferUsageARB.StaticDraw);
            }

            //告诉OpenGL该如何解析顶点数据
            gl.VertexAttribPointer(0, 3, VertexAttribPointerType.Float, false, 5 * sizeof(float), null);
            //启用顶点属性
            gl.EnableVertexAttribArray(0);
            //告诉OpenGL该如何解析顶点数据
            gl.VertexAttribPointer(1, 2, VertexAttribPointerType.Float, false, 5 * sizeof(float), (void*)(3 * sizeof(float)));
            //启用顶点属性
            gl.EnableVertexAttribArray(1);

            //加载贴图
            var texture1 = gl.GenTexture();
            gl.ActiveTexture(TextureUnit.Texture0); //在绑定纹理之前先激活纹理单元
            gl.BindTexture(TextureTarget.Texture2D, texture1);
            

            gl.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapS, (int)GLEnum.Repeat);   // set texture wrapping to GL_REPEAT (default wrapping method)
            gl.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapT, (int)GLEnum.Repeat);
            // set texture filtering parameters
            gl.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)GLEnum.LinearMipmapLinear);
            gl.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)GLEnum.Linear);

            using var bitmap = SKBitmap.Decode("./container.jpg");
            gl.TexImage2D(TextureTarget.Texture2D, 0, InternalFormat.Rgba, (uint)bitmap.Width, (uint)bitmap.Height, 0, PixelFormat.Bgra, PixelType.UnsignedByte, bitmap.GetPixelSpan());
            gl.GenerateMipmap(TextureTarget.Texture2D);

            var texture2 = gl.GenTexture();
            gl.ActiveTexture(TextureUnit.Texture1);
            gl.BindTexture(TextureTarget.Texture2D, texture2);
           

            gl.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapS, (int)GLEnum.Repeat);   // set texture wrapping to GL_REPEAT (default wrapping method)
            gl.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapT, (int)GLEnum.Repeat);
            // set texture filtering parameters
            gl.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)GLEnum.LinearMipmapLinear);
            gl.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)GLEnum.Linear);

            using var bitmap2 = SKBitmap.Decode("./awesomeface.png");
            gl.TexImage2D(TextureTarget.Texture2D, 0, InternalFormat.Rgba, (uint)bitmap2.Width, (uint)bitmap2.Height, 0, PixelFormat.Bgra, PixelType.UnsignedByte, bitmap2.GetPixelSpan());
            gl.GenerateMipmap(TextureTarget.Texture2D);

            // tell opengl for each sampler to which texture unit it belongs to (only has to be done once)
            shader.Use();
            shader.SetInt("texture1", 0);
            shader.SetInt("texture2", 1);
            while (!GLFW.WindowShouldClose(window))
            {
                //渲染背景
                gl.ClearColor(0.2f, 0.3f, 0.3f, 1.0f);
                gl.Clear(ClearBufferMask.ColorBufferBit);

                // create transformations
                Matrix4x4 transform = Matrix4x4.CreateTranslation(0.5f, -0.5f, 0);
                transform = Matrix4x4.CreateRotationZ((float)GLFW.GetTime()) * transform;

                shader.SetMatrix4x4("transform", transform);
                gl.DrawElements(PrimitiveType.Triangles, (uint)indices.Length, DrawElementsType.UnsignedInt, null);

                GLFW.SwapBuffers(window);
                GLFW.PollEvents();
            }
        }
        private unsafe static void framebuffer_size_callback(WindowHandle* window, int width, int height)
        {
            gl.Viewport(0, 0, (uint)width, (uint)height);
        }
    }
}
