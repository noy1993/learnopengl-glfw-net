using GLFW;
using System;
using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices;
using static OpenGL.Gl;

namespace HelloTriangle
{
    class Program
    {
        static unsafe void Main(string[] args)
        {
           
            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();
            for (int i = 0; i < 10000; i++)
            {
                Glfw.Init();
            }
            stopwatch.Stop();
            Console.WriteLine(stopwatch.ElapsedTicks);
            Console.ReadKey();
            ////TriangleProgram();
            //RecProgram();
        }

        static unsafe void TriangleProgram()
        {
            Glfw.Init();
            Glfw.WindowHint(Hint.ContextVersionMajor, 3);
            Glfw.WindowHint(Hint.ContextVersionMinor, 3);
            Glfw.WindowHint(Hint.OpenglProfile, Profile.Core);

            var window = Glfw.CreateWindow(800, 600, "LearnOpenGL", Monitor.None, Window.None);
            if (window == Window.None)
            {
                Console.WriteLine("Failed to create GLFW window");
                Glfw.Terminate();
            }
            Glfw.MakeContextCurrent(window);
            Glfw.SetFramebufferSizeCallback(window, framebuffer_size_callback);
            Import(Glfw.GetProcAddress);
           
            //创建顶点着色器 并编译
            uint vertexShader = glCreateShader(GL_VERTEX_SHADER);
            glShaderSource(vertexShader, File.ReadAllText("./triangle.vert"));
            glCompileShader(vertexShader);
            //创建片段着色器 并编译
            uint fragmentShader = glCreateShader(GL_FRAGMENT_SHADER);
            glShaderSource(fragmentShader, File.ReadAllText("./triangle.frag"));
            glCompileShader(fragmentShader);
            //创建着色器程序，并将着色器挂接上去
            uint shaderProgram = glCreateProgram();
            glAttachShader(shaderProgram, vertexShader);
            glAttachShader(shaderProgram, fragmentShader);
            glLinkProgram(shaderProgram);

            //由于已经程序已经拷贝到了GPU 了 可以删除着色器
            glDeleteShader(vertexShader);
            glDeleteShader(fragmentShader);

            var vertices = new[]
            {
                -0.5f, -0.5f, 0.0f,
                0.5f, -0.5f, 0.0f,
                0.0f,  0.5f, 0.0f
            };
            //绑定 VAO 与 VBO
            uint VAO = glGenVertexArray();
            uint VBO = glGenBuffer();
            glBindVertexArray(VAO);

            //把顶点数组复制到缓冲中供OpenGL使用
            glBindBuffer(GL_ARRAY_BUFFER, VBO);
            var arr = Marshal.UnsafeAddrOfPinnedArrayElement(vertices, 0);
            glBufferData(GL_ARRAY_BUFFER, sizeof(float) * vertices.Length, arr, GL_STATIC_DRAW);

            //告诉OpenGL该如何解析顶点数据
            glVertexAttribPointer(0, 3, GL_FLOAT, false, 3 * sizeof(float), NULL);
            //启用顶点属性
            glEnableVertexAttribArray(0);
            while (!Glfw.WindowShouldClose(window))
            {
                //渲染背景
                glClearColor(0.2f, 0.3f, 0.3f, 1.0f);
                glClear(GL_COLOR_BUFFER_BIT);

                //绘制物体
                glUseProgram(shaderProgram);
                glBindVertexArray(VAO);
                glDrawArrays(GL_TRIANGLES, 0, 3);

                Glfw.SwapBuffers(window);
                Glfw.PollEvents();
            }

            glDeleteVertexArray(VAO);
            glDeleteBuffer(VBO);
            glDeleteProgram(shaderProgram);
            Glfw.Terminate();
        }
        static unsafe void RecProgram()
        {
            Glfw.Init();
            Glfw.WindowHint(Hint.ContextVersionMajor, 3);
            Glfw.WindowHint(Hint.ContextVersionMinor, 3);
            Glfw.WindowHint(Hint.OpenglProfile, Profile.Core);

            var window = Glfw.CreateWindow(800, 600, "LearnOpenGL", Monitor.None, Window.None);
            if (window == Window.None)
            {
                Console.WriteLine("Failed to create GLFW window");
                Glfw.Terminate();
            }
            Glfw.MakeContextCurrent(window);
            Glfw.SetFramebufferSizeCallback(window, framebuffer_size_callback);
            Import(Glfw.GetProcAddress);

            //创建顶点着色器 并编译
            uint vertexShader = glCreateShader(GL_VERTEX_SHADER);
            glShaderSource(vertexShader, File.ReadAllText("./triangle.vert"));
            glCompileShader(vertexShader);
            //创建片段着色器 并编译
            uint fragmentShader = glCreateShader(GL_FRAGMENT_SHADER);
            glShaderSource(fragmentShader, File.ReadAllText("./triangle.frag"));
            glCompileShader(fragmentShader);
            //创建着色器程序，并将着色器挂接上去
            uint shaderProgram = glCreateProgram();
            glAttachShader(shaderProgram, vertexShader);
            glAttachShader(shaderProgram, fragmentShader);
            glLinkProgram(shaderProgram);

            //由于已经程序已经拷贝到了GPU 了 可以删除着色器
            glDeleteShader(vertexShader);
            glDeleteShader(fragmentShader);

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
            uint VAO = glGenVertexArray();
            uint VBO = glGenBuffer();
            uint EBO = glGenBuffer();

            glBindVertexArray(VAO);

            //把顶点数组复制到缓冲中供OpenGL使用
            glBindBuffer(GL_ARRAY_BUFFER, VBO);
            var arr = Marshal.UnsafeAddrOfPinnedArrayElement(vertices, 0);
            glBufferData(GL_ARRAY_BUFFER, sizeof(float) * vertices.Length, arr, GL_STATIC_DRAW);

            glBindBuffer(GL_ELEMENT_ARRAY_BUFFER, EBO);
            var arr2 = Marshal.UnsafeAddrOfPinnedArrayElement(indices, 0);
            glBufferData(GL_ELEMENT_ARRAY_BUFFER, sizeof(uint) * indices.Length, arr2, GL_STATIC_DRAW);

            //告诉OpenGL该如何解析顶点数据
            glVertexAttribPointer(0, 3, GL_FLOAT, false, 3 * sizeof(float), NULL);
            //启用顶点属性
            glEnableVertexAttribArray(0);
            while (!Glfw.WindowShouldClose(window))
            {
                processInput(window);
                //渲染背景
                glClearColor(0.2f, 0.3f, 0.3f, 1.0f);
                glClear(GL_COLOR_BUFFER_BIT);

                //绘制物体
                glUseProgram(shaderProgram);
                glBindVertexArray(VAO);
                glDrawElements(GL_TRIANGLES, 6, GL_UNSIGNED_INT, NULL);

                Glfw.SwapBuffers(window);
                Glfw.PollEvents();
            }

            glDeleteVertexArray(VAO);
            glDeleteBuffer(VBO);
            glDeleteProgram(shaderProgram);
            Glfw.Terminate();
        }
        private static void processInput(Window window)
        {
            if (Glfw.GetKey(window, Keys.G) == InputState.Press)
            {
                glPolygonMode(GL_FRONT_AND_BACK, GL_LINE);
            }
            if (Glfw.GetKey(window, Keys.B) == InputState.Press)
            {
                glPolygonMode(GL_FRONT_AND_BACK, GL_FILL);
            }
        }
        private static void framebuffer_size_callback(IntPtr window, int width, int height)
        {
            glViewport(0, 0, width, height);
        }
    }
}
