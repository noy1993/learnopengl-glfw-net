using OpenTK.Graphics.ES30;
using OpenTK.Windowing.GraphicsLibraryFramework;
using System;
using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices;

namespace HelloTriangle
{
    class Program
    {
        static unsafe void Main(string[] args)
        {
            TriangleProgram();
        }

        static unsafe void TriangleProgram()
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
        static unsafe void RecProgram()
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

            //由于已经程序已经拷贝到了GPU 了 可以删除着色器
            GL.DeleteShader(vertexShader);
            GL.DeleteShader(fragmentShader);

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
            int VAO = GL.GenVertexArray();
            int VBO = GL.GenBuffer();
            int EBO = GL.GenBuffer();

            GL.BindVertexArray(VAO);

            //把顶点数组复制到缓冲中供OpenGL使用
            GL.BindBuffer(BufferTarget.ArrayBuffer, VBO);
            var arr = Marshal.UnsafeAddrOfPinnedArrayElement(vertices, 0);
            GL.BufferData(BufferTarget.ArrayBuffer, sizeof(float) * vertices.Length, arr, BufferUsageHint.StaticDraw);

            GL.BindBuffer(BufferTarget.ElementArrayBuffer, EBO);
            var arr2 = Marshal.UnsafeAddrOfPinnedArrayElement(indices, 0);
            GL.BufferData(BufferTarget.ElementArrayBuffer, sizeof(uint) * indices.Length, arr2, BufferUsageHint.StaticDraw);

            //告诉OpenGL该如何解析顶点数据
            GL.VertexAttribPointer(0, 3, VertexAttribPointerType.Float, false, 3 * sizeof(float), 0);
            //启用顶点属性
            GL.EnableVertexAttribArray(0);
            while (!GLFW.WindowShouldClose(window))
            {
                processInput(window);
                //渲染背景
                GL.ClearColor(0.2f, 0.3f, 0.3f, 1.0f);
                GL.Clear(ClearBufferMask.ColorBufferBit);

                //绘制物体
                GL.UseProgram(shaderProgram);
                GL.BindVertexArray(VAO);
                GL.DrawElements(PrimitiveType.Triangles, 6, DrawElementsType.UnsignedInt, IntPtr.Zero);

                GLFW.SwapBuffers(window);
                GLFW.PollEvents();
            }

            GL.DeleteVertexArray(VAO);
            GL.DeleteBuffer(VBO);
            GL.DeleteProgram(shaderProgram);
            GLFW.Terminate();
        }
        private unsafe static void processInput(Window* window)
        {
            //if (GLFW.GetKey(window, Keys.G) == InputAction.Press)
            //{
            //    GL.mo
            //    GL.PolygonMode(GL_FRONT_AND_BACK, GL_LINE);
            //}
            //if (GLFW.GetKey(window, Keys.B) == InputAction.Press)
            //{
            //    GL.PolygonMode(GL_FRONT_AND_BACK, GL_FILL);
            //}
        }

        private unsafe static void framebuffer_size_callback(Window* window, int width, int height)
        {
            GL.Viewport(0, 0, width, height);
        }
    }
}
