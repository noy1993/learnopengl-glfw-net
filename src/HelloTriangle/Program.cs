using Silk.NET.GLFW;
using Silk.NET.OpenGLES;
using System;
using System.IO;
using System.Numerics;
using System.Text;
using System.Threading;

namespace HelloTriangle
{
    class Program
    {
        static unsafe void Main(string[] args)
        {
            TriangleProgram();
            //RecProgram();
           
        }
        static readonly Glfw GLFW = Glfw.GetApi();
        static GL gl;
        static unsafe void TriangleProgram()
        {
            GLFW.Init();
            var window = GLFW.CreateWindow(800, 600, "LearnOpenGL", null, null);
            if (window == null)
            {
                GLFW.Terminate();
                Glfw.ThrowExceptions();
                return;
            }
            GLFW.MakeContextCurrent(window);
            GLFW.SetFramebufferSizeCallback(window, framebuffer_size_callback);

            gl = GL.GetApi(new GlfwContext(GLFW, window));

            //gl.Enable(EnableCap.ScissorTest);
            //gl.Scissor(0, 0, 100, 100);
            
            //创建顶点着色器 并编译
            uint vertexShader = gl.CreateShader(ShaderType.VertexShader);
            gl.ShaderSource(vertexShader, File.ReadAllText("./triangle.vert"));
            gl.CompileShader(vertexShader);
            //创建片段着色器 并编译
            uint fragmentShader = gl.CreateShader(ShaderType.FragmentShader);
            gl.ShaderSource(fragmentShader, File.ReadAllText("./triangle.frag"));
            gl.CompileShader(fragmentShader);
            //创建着色器程序，并将着色器挂接上去
            uint shaderProgram = gl.CreateProgram();
            gl.AttachShader(shaderProgram, vertexShader);
            gl.AttachShader(shaderProgram, fragmentShader);
            gl.LinkProgram(shaderProgram);

            //由于已经程序已经拷贝到了GPU 了 可以删除着色器
            gl.DeleteShader(vertexShader);
            gl.DeleteShader(fragmentShader);

            var vertices = new[]
            {
                -0.5f, -0.5f, 0.0f,
                0.5f, -0.5f, 0.0f,
                0.0f,  0.5f, 0.0f
            };
            //绑定 VAO 与 VBO
            uint VAO = gl.GenVertexArray();
            uint VBO = gl.GenBuffer();
            gl.BindVertexArray(VAO);

            //把顶点数组复制到缓冲中供OpenGL使用
            gl.BindBuffer(BufferTargetARB.ArrayBuffer, VBO);

            fixed (float* v = &vertices[0])
            {
                gl.BufferData(BufferTargetARB.ArrayBuffer, (nuint)(sizeof(float) * vertices.Length), v, BufferUsageARB.StaticDraw);
            }

            //告诉OpenGL该如何解析顶点数据
            gl.VertexAttribPointer(0, 3, VertexAttribPointerType.Float, false, 3 * sizeof(float), null);
            //启用顶点属性
            gl.EnableVertexAttribArray(0);

            gl.UseProgram(shaderProgram);

            while (!GLFW.WindowShouldClose(window))
            {
                //渲染背景
                gl.ClearColor(0.2f, 0.3f, 0.3f, 1.0f);
                gl.Clear(ClearBufferMask.ColorBufferBit|ClearBufferMask.StencilBufferBit);
                //绘制物体
                gl.BindVertexArray(VAO);

                gl.StencilMask(0xff);
                gl.StencilFunc(StencilFunction.Always, 1, 0xff);

                gl.DrawArrays(PrimitiveType.Triangles, 0, 3);

                GLFW.SwapBuffers(window);
              
                GLFW.PollEvents();

                gl.StencilMask(0xff);
                gl.StencilFunc(StencilFunction.Always, 1, 0xff);
                
                GLFW.GetCursorPos(window, out var x, out var y);
                gl.ReadPixels<int>((int)x, (int)y, 1, 1, PixelFormat.StencilIndex, PixelType.Int, out var indexd);
                Console.WriteLine($"x{x},y{y},{indexd}");
            }

            gl.DeleteVertexArray(VAO);
            gl.DeleteBuffer(VBO);
            gl.DeleteProgram(shaderProgram);
            GLFW.Terminate();
        }

        static unsafe void RecProgram()
        {
            GLFW.Init();
            var window = GLFW.CreateWindow(800, 600, "LearnOpenGL", null, null);
            if (window == null)
            {
                Console.WriteLine("Failed to create GLFW window");
                GLFW.Terminate();
                return;
            }
            GLFW.MakeContextCurrent(window);
            GLFW.SetFramebufferSizeCallback(window, framebuffer_size_callback);

            gl = GL.GetApi(new GlfwContext(GLFW, window));

            //创建顶点着色器 并编译
            uint vertexShader = gl.CreateShader(ShaderType.VertexShader);
            gl.ShaderSource(vertexShader, File.ReadAllText("./triangle.vert"));
            gl.CompileShader(vertexShader);
            //创建片段着色器 并编译
            uint fragmentShader = gl.CreateShader(ShaderType.FragmentShader);
            gl.ShaderSource(fragmentShader, File.ReadAllText("./triangle.frag"));
            gl.CompileShader(fragmentShader);
            //创建着色器程序，并将着色器挂接上去
            uint shaderProgram = gl.CreateProgram();
            gl.AttachShader(shaderProgram, vertexShader);
            gl.AttachShader(shaderProgram, fragmentShader);
            gl.LinkProgram(shaderProgram);


            //由于已经程序已经拷贝到了GPU 了 可以删除着色器
            gl.DeleteShader(vertexShader);
            gl.DeleteShader(fragmentShader);

            var vertices = new[]
            {
                0.5f, 0.5f, 0.0f,   // 右上角
                0.5f, -0.5f, 0.0f,  // 右下角
                -0.5f, -0.5f, 0.0f, // 左下角
                -0.5f, 0.5f, 0.0f   // 左上角
            };
            var indices = new uint[]
            {
                0,1,3,
                1,2,3
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
            gl.VertexAttribPointer(0, 3, VertexAttribPointerType.Float, false, 3 * sizeof(float), null);
            //启用顶点属性
            gl.EnableVertexAttribArray(0);
            gl.ClearColor(0.2f, 0.3f, 0.3f, 1.0f);

            gl.BindVertexArray(VAO);
            gl.UseProgram(shaderProgram);

            while (!GLFW.WindowShouldClose(window))
            {
                //渲染背景

                gl.Clear(ClearBufferMask.ColorBufferBit);

                //绘制物体
                gl.DrawElements(PrimitiveType.Triangles, (uint)indices.Length, DrawElementsType.UnsignedInt, null);

                GLFW.SwapBuffers(window);
                GLFW.PollEvents();
            }

            gl.DeleteVertexArray(VAO);
            gl.DeleteBuffer(VBO);
            gl.DeleteProgram(shaderProgram);
            GLFW.Terminate();
        }

        static unsafe void TestVAO()
        {
            GLFW.Init();
            var window = GLFW.CreateWindow(800, 600, "LearnOpenGL", null, null);
            if (window == null)
            {
                Console.WriteLine("Failed to create GLFW window");
                GLFW.Terminate();
                return;
            }
            GLFW.MakeContextCurrent(window);
            GLFW.SetFramebufferSizeCallback(window, framebuffer_size_callback);

            gl = GL.GetApi(new GlfwContext(GLFW, window));

            //创建顶点着色器 并编译
            uint vertexShader = gl.CreateShader(ShaderType.VertexShader);
            gl.ShaderSource(vertexShader, File.ReadAllText("./triangle.vert"));
            gl.CompileShader(vertexShader);
            //创建片段着色器 并编译
            uint fragmentShader = gl.CreateShader(ShaderType.FragmentShader);
            gl.ShaderSource(fragmentShader, File.ReadAllText("./triangle.frag"));
            gl.CompileShader(fragmentShader);
            //创建着色器程序，并将着色器挂接上去
            uint shaderProgram = gl.CreateProgram();
            gl.AttachShader(shaderProgram, vertexShader);
            gl.AttachShader(shaderProgram, fragmentShader);
            gl.LinkProgram(shaderProgram);


            //由于已经程序已经拷贝到了GPU 了 可以删除着色器
            gl.DeleteShader(vertexShader);
            gl.DeleteShader(fragmentShader);

            var vertices = new[]
            {
                0.5f, 0.5f, 0.0f,   // 右上角
                0.5f, -0.5f, 0.0f,  // 右下角
                -0.5f, -0.5f, 0.0f, // 左下角
                -0.5f, 0.5f, 0.0f   // 左上角
            };
            var indices = new uint[]
            {
                0,1,3,
                1,2,3
            };

            //tri 绑定 VAO 与 VBO
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
            gl.VertexAttribPointer(0, 3, VertexAttribPointerType.Float, false, 3 * sizeof(float), null);
            //启用顶点属性
            gl.EnableVertexAttribArray(0);
            gl.BindVertexArray(0);

            //tri
            //绑定 VAO 与 VBO
            uint VAO2 = gl.GenVertexArray();
            uint VBO2 = gl.GenBuffer();
            gl.BindVertexArray(VAO2);

            //把顶点数组复制到缓冲中供OpenGL使用
            gl.BindBuffer(BufferTargetARB.ArrayBuffer, VBO2);
            var vertices2 = new[]
{
                -1f, -1f, 0.0f,
                1f, -1f, 0.0f,
                0.0f,  0.5f, 0.0f
            };
            fixed (float* v = &vertices2[0])
            {
                gl.BufferData(BufferTargetARB.ArrayBuffer, (nuint)(sizeof(float) * vertices2.Length), v, BufferUsageARB.StaticDraw);
            }

            //告诉OpenGL该如何解析顶点数据
            gl.VertexAttribPointer(0, 3, VertexAttribPointerType.Float, false, 3 * sizeof(float), null);
            //启用顶点属性
            gl.EnableVertexAttribArray(0);


            gl.ClearColor(0.2f, 0.3f, 0.3f, 1.0f);
            while (!GLFW.WindowShouldClose(window))
            {
                //渲染背景

                gl.Clear(ClearBufferMask.ColorBufferBit);

                //绘制物体
                gl.UseProgram(shaderProgram);
                gl.BindVertexArray(VAO);
                //gl.DrawArrays(PrimitiveType.Triangles, 0, 3);
                gl.DrawElements(PrimitiveType.Triangles, (uint)indices.Length, DrawElementsType.UnsignedInt, null);

                gl.BindVertexArray(VAO2);
                //gl.DrawArrays(PrimitiveType.Triangles, 0, 3);
                gl.DrawArrays(PrimitiveType.Triangles, 0, 3);

                GLFW.SwapBuffers(window);
                GLFW.PollEvents();
            }

            gl.DeleteVertexArray(VAO);
            gl.DeleteBuffer(VBO);
            gl.DeleteProgram(shaderProgram);
            GLFW.Terminate();
        }

        private unsafe static void framebuffer_size_callback(WindowHandle* window, int width, int height)
        {
            gl.Viewport(0, 0, (uint)width, (uint)height);
        }
    }
}
