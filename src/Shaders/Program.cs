using GLFW;
using System;
using System.IO;
using System.Runtime.InteropServices;
using static OpenGL.Gl;

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
            int vertexColorLocation = glGetUniformLocation(shaderProgram, "color");

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
                var timeValue = Glfw.Time;
                float greenValue = (float)Math.Sin(timeValue) / 2.0f + 0.5f;
                glUniform4f(vertexColorLocation, 0.0f, greenValue, 0.0f, 1.0f);
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

        static unsafe void P2()
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

            using Shader shader = new Shader("./triangle2.vert", "./triangle2.frag");
            var vertices = new[]
            {
                // 位置              // 颜色
                 0.5f, -0.5f, 0.0f,  1.0f, 0.0f, 0.0f,   // 右下
                -0.5f, -0.5f, 0.0f,  0.0f, 1.0f, 0.0f,   // 左下
                 0.0f,  0.5f, 0.0f,  0.0f, 0.0f, 1.0f    // 顶部
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
            glVertexAttribPointer(0, 3, GL_FLOAT, false, 6 * sizeof(float), NULL);
            //启用顶点属性
            glEnableVertexAttribArray(0);

            glVertexAttribPointer(1, 3, GL_FLOAT, false, 6 * sizeof(float), NULL);
            //启用顶点属性
            glEnableVertexAttribArray(1);

            while (!Glfw.WindowShouldClose(window))
            {
                //渲染背景
                glClearColor(0.2f, 0.3f, 0.3f, 1.0f);
                glClear(GL_COLOR_BUFFER_BIT);

                //绘制物体
                shader.Use();
                glBindVertexArray(VAO);
                glDrawArrays(GL_TRIANGLES, 0, 3);

                Glfw.SwapBuffers(window);
                Glfw.PollEvents();
            }

            glDeleteVertexArray(VAO);
            glDeleteBuffer(VBO);
            Glfw.Terminate();
        }

        private static void framebuffer_size_callback(IntPtr window, int width, int height)
        {
            glViewport(0, 0, width, height);
        }
    }
}
