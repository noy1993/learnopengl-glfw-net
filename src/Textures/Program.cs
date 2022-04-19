using Silk.NET.GLFW;
using Silk.NET.OpenGLES;
using SkiaSharp;
using System;
using System.Drawing;
using System.IO;
using System.Runtime.InteropServices;

namespace Textures
{
    class Program
    {
        static readonly Glfw GLFW = Glfw.GetApi();
        internal static GL gl;
        static unsafe void P1()
        {
            GLFW.Init();

            var window = GLFW.CreateWindow(800, 600, "LearnOpenGL", null, null);
            if (window == null)
            {
                Console.WriteLine("Failed to create gl.FW window");
                GLFW.Terminate();
            }
            GLFW.MakeContextCurrent(window);
            GLFW.SetFramebufferSizeCallback(window, framebuffer_size_callback);

            gl = GL.GetApi(new GlfwContext(GLFW, window));
            OpenGL.Extension.Shader.sgl = gl;
            OpenGL.Extension.Shader shader = new OpenGL.Extension.Shader("./texture.vert", "./texture.frag");
            var vertices = new[]
            {
                // positions          // colors           // texture coords
                 0.5f,  0.5f, 0.0f,   1.0f, 0.0f, 0.0f,   1.0f, 1.0f, // top right
                 0.5f, -0.5f, 0.0f,   0.0f, 1.0f, 0.0f,   1.0f, 0.0f, // bottom right
                -0.5f, -0.5f, 0.0f,   0.0f, 0.0f, 1.0f,   0.0f, 0.0f, // bottom left
                -0.5f,  0.5f, 0.0f,   1.0f, 1.0f, 0.0f,   0.0f, 1.0f  // top left 
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
            fixed (float* v = &vertices[0])
            {
                gl.BufferData(BufferTargetARB.ArrayBuffer, (nuint)(sizeof(float) * vertices.Length), v, BufferUsageARB.StaticDraw);
            }

            gl.BindBuffer(BufferTargetARB.ElementArrayBuffer, EBO);
            fixed (uint* i = &indices[0])
            {
                gl.BufferData(BufferTargetARB.ElementArrayBuffer, (nuint)(sizeof(uint) * indices.Length), i, BufferUsageARB.StaticDraw);
            }

            //告诉OpenGL该如何解析顶点数据
            gl.VertexAttribPointer(0, 3, VertexAttribPointerType.Float, false, 8 * sizeof(float), null);
            //启用顶点属性
            gl.EnableVertexAttribArray(0);
            //告诉OpenGL该如何解析顶点数据
            gl.VertexAttribPointer(1, 3, VertexAttribPointerType.Float, false, 8 * sizeof(float), (void*)(3 * sizeof(float)));
            //启用顶点属性
            gl.EnableVertexAttribArray(1);
            //告诉OpenGL该如何解析顶点数据
            gl.VertexAttribPointer(2, 2, VertexAttribPointerType.Float, false, 8 * sizeof(float), (void*)(6 * sizeof(float)));
            //启用顶点属性
            gl.EnableVertexAttribArray(2);

            //加载贴图
            var texture = gl.GenTexture();
            gl.ActiveTexture(TextureUnit.Texture0);//由于0号单元 默认是激活的，所以可以不写
            gl.BindTexture(TextureTarget.Texture2D, texture);

            gl.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapS, (int)GLEnum.Repeat); // set texture wrapping to gl._REPEAT (default wrapping method)
            gl.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapT, (int)GLEnum.Repeat);

            // set texture filtering parameters
            gl.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)GLEnum.Linear);
            gl.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)GLEnum.Linear);

            var bitmap = SKBitmap.Decode("./container.jpg");
            gl.TexImage2D(TextureTarget.Texture2D, 0, InternalFormat.Rgba, (uint)bitmap.Width, (uint)bitmap.Height, 0, PixelFormat.Bgra, PixelType.UnsignedByte, bitmap.GetPixelSpan());

            gl.GenerateMipmap(TextureTarget.Texture2D);
            shader.Use();
            while (!GLFW.WindowShouldClose(window))
            {
                //渲染背景
                gl.ClearColor(0.2f, 0.3f, 0.3f, 1.0f);
                gl.Clear(ClearBufferMask.ColorBufferBit);

                //绘制物体
                 
                gl.DrawElements(PrimitiveType.Triangles, (uint)indices.Length, DrawElementsType.UnsignedInt, null);

                GLFW.SwapBuffers(window);
                GLFW.PollEvents();
            }

            gl.DeleteVertexArray(VAO);
            gl.DeleteBuffer(VBO);
            gl.DeleteBuffer(EBO);
            GLFW.Terminate();
        }

        static unsafe void P2()
        {
            GLFW.Init();

            var window = GLFW.CreateWindow(800, 600, "LearnOpenGL", null, null);
            if (window == null)
            {
                Console.WriteLine("Failed to create gl.FW window");
                GLFW.Terminate();
            }
            GLFW.MakeContextCurrent(window);
            GLFW.SetFramebufferSizeCallback(window, framebuffer_size_callback);

            gl = GL.GetApi(new GlfwContext(GLFW, window));

            OpenGL.Extension.Shader.sgl = gl;
            OpenGL.Extension.Shader shader = new OpenGL.Extension.Shader("./texture.vert", "./texture2.frag");
            var vertices = new[]
            {
                // positions          // colors           // texture coords
                 0.5f,  0.5f, 0.0f,   1.0f, 0.0f, 0.0f,   1.0f, 1.0f, // top right
                 0.5f, -0.5f, 0.0f,   0.0f, 1.0f, 0.0f,   1.0f, 0.0f, // bottom right
                -0.5f, -0.5f, 0.0f,   0.0f, 0.0f, 1.0f,   0.0f, 0.0f, // bottom left
                -0.5f,  0.5f, 0.0f,   1.0f, 1.0f, 0.0f,   0.0f, 1.0f  // top left 
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
            fixed (float* v = &vertices[0])
            {
                gl.BufferData(BufferTargetARB.ArrayBuffer, (nuint)(sizeof(float) * vertices.Length), v, BufferUsageARB.StaticDraw);
            }

            gl.BindBuffer(BufferTargetARB.ElementArrayBuffer, EBO);
            var arr2 = Marshal.UnsafeAddrOfPinnedArrayElement(indices, 0);
            fixed (uint* i = &indices[0])
            {
                gl.BufferData(BufferTargetARB.ElementArrayBuffer, (nuint)(sizeof(uint) * indices.Length), i, BufferUsageARB.StaticDraw);
            }

            //告诉OpenGL该如何解析顶点数据
            gl.VertexAttribPointer(0, 3, VertexAttribPointerType.Float, false, 8 * sizeof(float), null);
            //启用顶点属性
            gl.EnableVertexAttribArray(0);
            //告诉OpenGL该如何解析顶点数据
            gl.VertexAttribPointer(1, 3, VertexAttribPointerType.Float, false, 8 * sizeof(float), (void*)(3 * sizeof(float)));
            //启用顶点属性
            gl.EnableVertexAttribArray(1);
            //告诉OpenGL该如何解析顶点数据
            gl.VertexAttribPointer(2, 2, VertexAttribPointerType.Float, false, 8 * sizeof(float), (void*)(6 * sizeof(float)));
            //启用顶点属性
            gl.EnableVertexAttribArray(2);

            //加载贴图
            var texture1 = gl.GenTexture();
            gl.BindTexture(TextureTarget.Texture2D, texture1);

            gl.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapS, (int)GLEnum.Repeat);   // set texture wrapping to gl._REPEAT (default wrapping method)
            gl.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapT, (int)GLEnum.Repeat);
            // set texture filtering parameters
            gl.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)GLEnum.Linear);
            gl.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)GLEnum.Linear);

            using var data = SKBitmap.Decode("./container.jpg");

            gl.TexImage2D(TextureTarget.Texture2D, 0, InternalFormat.Rgba, (uint)data.Width, (uint)data.Height, 0, PixelFormat.Bgra, PixelType.UnsignedByte, data.GetPixelSpan());
            gl.GenerateMipmap(TextureTarget.Texture2D);

            var texture2 = gl.GenTexture();
            gl.BindTexture(TextureTarget.Texture2D, texture2);

            gl.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapS, (int)GLEnum.Repeat);   // set texture wrapping to gl._REPEAT (default wrapping method)
            gl.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapT, (int)GLEnum.Repeat);
            // set texture filtering parameters
            gl.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)GLEnum.Linear);
            gl.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)GLEnum.Linear);

            using var bitmap = SKBitmap.Decode("./awesomeface.png");
            using (var canvas = new SKCanvas(bitmap))
            {
                canvas.RotateDegrees(180, bitmap.Width / 2, bitmap.Height / 2);
                canvas.DrawBitmap(bitmap.Copy(), 0, 0);
            }

            gl.TexImage2D(TextureTarget.Texture2D, 0, InternalFormat.Rgba, (uint)bitmap.Width, (uint)bitmap.Height, 0, PixelFormat.Bgra, PixelType.UnsignedByte, bitmap.GetPixels().ToPointer());
            gl.GenerateMipmap(TextureTarget.Texture2D);

            shader.Use();
            shader.SetInt("texture1", 0);
            shader.SetInt("texture2", 1);
            while (!GLFW.WindowShouldClose(window))
            {
                //渲染背景
                gl.ClearColor(0.2f, 0.3f, 0.3f, 1.0f);
                gl.Clear(ClearBufferMask.ColorBufferBit);

                //绘制物体
                gl.ActiveTexture(TextureUnit.Texture0);
                gl.BindTexture(TextureTarget.Texture2D, texture1);
                gl.ActiveTexture(TextureUnit.Texture1);
                gl.BindTexture(TextureTarget.Texture2D, texture2);

                //shader.Use();
                //glBindVertexArray(VAO);
                gl.DrawElements(PrimitiveType.Triangles, (uint)indices.Length, DrawElementsType.UnsignedInt, null);

                GLFW.SwapBuffers(window);
                GLFW.PollEvents();
            }

            gl.DeleteVertexArray(VAO);
            gl.DeleteBuffer(VBO);
            gl.DeleteBuffer(EBO);
            GLFW.Terminate();
        }

        static unsafe void Main(string[] args)
        {
            //P1();
            P2();
        }
        private unsafe static void framebuffer_size_callback(WindowHandle* window, int width, int height)
        {
            gl.Viewport(0, 0, (uint)width, (uint)height);
        }
    }
}
