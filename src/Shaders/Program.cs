using OpenTK.Graphics.ES30;
using OpenTK.Windowing.GraphicsLibraryFramework;
using System;
using System.IO;
using System.Runtime.InteropServices;

namespace Shaders
{
    class Program
    {
        static unsafe void Main(string[] args)
        {
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


            GL.LoadBindings(new GLFWBindingsContext()); ;

            //创建顶点着色器 并编译
            int vertexShader = GL.CreateShader(ShaderType.VertexShader);
            GL.ShaderSource(vertexShader, File.ReadAllText("./triangle.vert"));
            GL.CompileShader(vertexShader);
            //创建片段着色器 并编译
            int fragmentShader = GL.CreateShader(ShaderType.FragmentShader);
            GL.ShaderSource(fragmentShader, File.ReadAllText("./triangle.frag"));
            GL.CompileShader(fragmentShader);
            //创建着色器程序，并将着色器挂接上去
            int shaderProgram = GL.CreateProgram();
            GL.AttachShader(shaderProgram, vertexShader);
            GL.AttachShader(shaderProgram, fragmentShader);
            GL.LinkProgram(shaderProgram);
            int vertexColorLocation = GL.GetUniformLocation(shaderProgram, "color");

            //由于已经程序已经拷贝到了GPU 了 可以删除着色器
            GL.DeleteShader(vertexShader);
            GL.DeleteShader(fragmentShader);

            var vertices = new[]
            {
                -0.5f, -0.5f, 0.0f,
                0.5f, -0.5f, 0.0f,
                0.0f,  0.5f, 0.0f
            };
            //绑定 VAO 与 VBO
            int VAO = GL.GenVertexArray();
            int VBO = GL.GenBuffer();
            GL.BindVertexArray(VAO);

            //把顶点数组复制到缓冲中供OpenGL使用
            GL.BindBuffer(BufferTarget.ArrayBuffer, VBO);
            var arr = Marshal.UnsafeAddrOfPinnedArrayElement(vertices, 0);
            GL.BufferData(BufferTarget.ArrayBuffer, sizeof(float) * vertices.Length, arr, BufferUsageHint.StaticDraw);

            //告诉OpenGL该如何解析顶点数据
            GL.VertexAttribPointer(0, 3, VertexAttribPointerType.Float, false, 3 * sizeof(float), 0);
            //启用顶点属性
            GL.EnableVertexAttribArray(0);

            while (!GLFW.WindowShouldClose(window))
            {
                //渲染背景
                GL.ClearColor(0.2f, 0.3f, 0.3f, 1.0f);
                GL.Clear(ClearBufferMask.ColorBufferBit);

                //绘制物体
                GL.UseProgram(shaderProgram);
                var timeValue = GLFW.GetTime();
                float greenValue = (float)Math.Sin(timeValue) / 2.0f + 0.5f;
                GL.Uniform4(vertexColorLocation, 0.0f, greenValue, 0.0f, 1.0f);
                GL.BindVertexArray(VAO);
                GL.DrawArrays(PrimitiveType.Triangles, 0, 3);

                GLFW.SwapBuffers(window);
                GLFW.PollEvents();
            }

            GL.DeleteVertexArray(VAO);
            GL.DeleteBuffer(VBO);
            GL.DeleteProgram(shaderProgram);
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

            GL.LoadBindings(new GLFWBindingsContext());

            using var shader = new Shader("./triangle2.vert", "./triangle2.frag");
            var vertices = new[]
            {
                // 位置              // 颜色
                 0.5f, -0.5f, 0.0f,  1.0f, 0.0f, 0.0f,   // 右下
                -0.5f, -0.5f, 0.0f,  0.0f, 1.0f, 0.0f,   // 左下
                 0.0f,  0.5f, 0.0f,  0.0f, 0.0f, 1.0f    // 顶部
            };
            //绑定 VAO 与 VBO
            int VAO = GL.GenVertexArray();
            int VBO = GL.GenBuffer();
            GL.BindVertexArray(VAO);

            //把顶点数组复制到缓冲中供OpenGL使用
            GL.BindBuffer(BufferTarget.ArrayBuffer, VBO);
            var arr = Marshal.UnsafeAddrOfPinnedArrayElement(vertices, 0);
            GL.BufferData(BufferTarget.ArrayBuffer, sizeof(float) * vertices.Length, arr, BufferUsageHint.StaticDraw);

            //告诉OpenGL该如何解析顶点数据
            GL.VertexAttribPointer(0, 3, VertexAttribPointerType.Float, false, 6 * sizeof(float), 0);
            //启用顶点属性
            GL.EnableVertexAttribArray(0);

            GL.VertexAttribPointer(1, 3, VertexAttribPointerType.Float, false, 6 * sizeof(float), (IntPtr)(3 * sizeof(float)));
            //启用顶点属性
            GL.EnableVertexAttribArray(1);

            while (!GLFW.WindowShouldClose(window))
            {
                //渲染背景
                GL.ClearColor(0.2f, 0.3f, 0.3f, 1.0f);
                GL.Clear(ClearBufferMask.ColorBufferBit);

                //绘制物体
                shader.Use();
                GL.BindVertexArray(VAO);
                GL.DrawArrays(PrimitiveType.Triangles, 0, 3);

                GLFW.SwapBuffers(window);
                GLFW.PollEvents();
            }

            GL.DeleteVertexArray(VAO);
            GL.DeleteBuffer(VBO);
            GLFW.Terminate();
        }

        private unsafe static void framebuffer_size_callback(Window* window, int width, int height)
        {
            GL.Viewport(0, 0, width, height);
        }
    }
}
