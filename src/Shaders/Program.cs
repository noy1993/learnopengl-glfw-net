using Silk.NET.GLFW;
using Silk.NET.OpenGLES;
using System;
using System.IO;
using System.Runtime.InteropServices;

namespace Shaders
{
    class Program
    {
        static readonly Glfw GLFW = Glfw.GetApi();
        internal static GL gl;
        static unsafe void Main(string[] args)
        {
            P1();
            P2();
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
            int vertexColorLocation = gl.GetUniformLocation(shaderProgram, "color");

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

            while (!GLFW.WindowShouldClose(window))
            {
                //渲染背景
                gl.ClearColor(0.2f, 0.3f, 0.3f, 1.0f);
                gl.Clear(ClearBufferMask.ColorBufferBit);

                //绘制物体
                gl.UseProgram(shaderProgram);
                var timeValue = GLFW.GetTime();
                float greenValue = (float)Math.Sin(timeValue) / 2.0f + 0.5f;
                gl.Uniform4(vertexColorLocation, 0.0f, greenValue, 0.0f, 1.0f);
                gl.BindVertexArray(VAO);
                gl.DrawArrays(PrimitiveType.Triangles, 0, 3);

                GLFW.SwapBuffers(window);
                GLFW.PollEvents();
            }

            gl.DeleteVertexArray(VAO);
            gl.DeleteBuffer(VBO);
            gl.DeleteProgram(shaderProgram);
            GLFW.Terminate();
        }

        static unsafe void P2()
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

            using var shader = new Shader("./triangle2.vert", "./triangle2.frag");
            var vertices = new[]
            {
                // 位置              // 颜色
                 0.5f, -0.5f, 0.0f,  1.0f, 0.0f, 0.0f,   // 右下
                -0.5f, -0.5f, 0.0f,  0.0f, 1.0f, 0.0f,   // 左下
                 0.0f,  0.5f, 0.0f,  0.0f, 0.0f, 1.0f    // 顶部
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
            gl.VertexAttribPointer(0, 3, VertexAttribPointerType.Float, false, 6 * sizeof(float), null);
            //启用顶点属性
            gl.EnableVertexAttribArray(0);

            gl.VertexAttribPointer(1, 3, VertexAttribPointerType.Float, false, 6 * sizeof(float), (void*)(3 * sizeof(float)));
            //启用顶点属性
            gl.EnableVertexAttribArray(1);

            while (!GLFW.WindowShouldClose(window))
            {
                //渲染背景
                gl.ClearColor(0.2f, 0.3f, 0.3f, 1.0f);
                gl.Clear(ClearBufferMask.ColorBufferBit);

                //绘制物体
                shader.Use();
                gl.BindVertexArray(VAO);
                gl.DrawArrays(PrimitiveType.Triangles, 0, 3);

                GLFW.SwapBuffers(window);
                GLFW.PollEvents();
            }

            gl.DeleteVertexArray(VAO);
            gl.DeleteBuffer(VBO);
            GLFW.Terminate();
        }

        private unsafe static void framebuffer_size_callback(WindowHandle* window, int width, int height)
        {
            gl.Viewport(0, 0, (uint)width, (uint)height);
        }
    }
}
