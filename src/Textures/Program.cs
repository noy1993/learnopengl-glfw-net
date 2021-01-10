using GLFW;
using System;
using System.Drawing;
using System.Runtime.InteropServices;
using static OpenGL.Gl;

namespace Textures
{
    class Program
    {
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
            Glfw.SetFramebufferSizeCallback(window, framebuffer_size_callback);
            Import(Glfw.GetProcAddress);

            Shaders.Shader shader = new Shaders.Shader("./texture.vert", "./texture.frag");
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
            glVertexAttribPointer(0, 3, GL_FLOAT, false, 8 * sizeof(float), NULL);
            //启用顶点属性
            glEnableVertexAttribArray(0);
            //告诉OpenGL该如何解析顶点数据
            glVertexAttribPointer(1, 3, GL_FLOAT, false, 8 * sizeof(float), (void*)(3 * sizeof(float)));
            //启用顶点属性
            glEnableVertexAttribArray(1);
            //告诉OpenGL该如何解析顶点数据
            glVertexAttribPointer(2, 2, GL_FLOAT, false, 8 * sizeof(float), (void*)(6 * sizeof(float)));
            //启用顶点属性
            glEnableVertexAttribArray(2);

            //加载贴图
            var texture = glGenTexture();
            glBindTexture(GL_TEXTURE_2D, texture);

            glTexParameteri(GL_TEXTURE_2D, GL_TEXTURE_WRAP_S, GL_REPEAT);   // set texture wrapping to GL_REPEAT (default wrapping method)
            glTexParameteri(GL_TEXTURE_2D, GL_TEXTURE_WRAP_T, GL_REPEAT);
            // set texture filtering parameters
            glTexParameteri(GL_TEXTURE_2D, GL_TEXTURE_MIN_FILTER, GL_LINEAR);
            glTexParameteri(GL_TEXTURE_2D, GL_TEXTURE_MAG_FILTER, GL_LINEAR);

            var data = new Bitmap("./container.jpg");
            var map = data.LockBits(new Rectangle(0, 0, data.Width, data.Height), System.Drawing.Imaging.ImageLockMode.ReadWrite, data.PixelFormat);

            glTexImage2D(GL_TEXTURE_2D, 0, GL_RGB, data.Width, data.Height, 0, GL_BGR, GL_UNSIGNED_BYTE, map.Scan0);

            glGenerateMipmap(GL_TEXTURE_2D);
            data.UnlockBits(map);
            data.Dispose();

            shader.Use();

            while (!Glfw.WindowShouldClose(window))
            {
                //渲染背景
                glClearColor(0.2f, 0.3f, 0.3f, 1.0f);
                glClear(GL_COLOR_BUFFER_BIT);

                //绘制物体
                //glBindTexture(GL_TEXTURE_2D, texture);
                //shader.Use();
                //glBindVertexArray(VAO);
                glDrawElements(GL_TRIANGLES, 6, GL_UNSIGNED_INT, null);

                Glfw.SwapBuffers(window);
                Glfw.PollEvents();
            }

            glDeleteVertexArray(VAO);
            glDeleteBuffer(VBO);
            glDeleteBuffer(EBO);
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

            Shaders.Shader shader = new Shaders.Shader("./texture.vert", "./texture2.frag");
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
            glVertexAttribPointer(0, 3, GL_FLOAT, false, 8 * sizeof(float), NULL);
            //启用顶点属性
            glEnableVertexAttribArray(0);
            //告诉OpenGL该如何解析顶点数据
            glVertexAttribPointer(1, 3, GL_FLOAT, false, 8 * sizeof(float), (void*)(3 * sizeof(float)));
            //启用顶点属性
            glEnableVertexAttribArray(1);
            //告诉OpenGL该如何解析顶点数据
            glVertexAttribPointer(2, 2, GL_FLOAT, false, 8 * sizeof(float), (void*)(6 * sizeof(float)));
            //启用顶点属性
            glEnableVertexAttribArray(2);

            //加载贴图
            var texture1 = glGenTexture();
            glBindTexture(GL_TEXTURE_2D, texture1);

            glTexParameteri(GL_TEXTURE_2D, GL_TEXTURE_WRAP_S, GL_REPEAT);   // set texture wrapping to GL_REPEAT (default wrapping method)
            glTexParameteri(GL_TEXTURE_2D, GL_TEXTURE_WRAP_T, GL_REPEAT);
            // set texture filtering parameters
            glTexParameteri(GL_TEXTURE_2D, GL_TEXTURE_MIN_FILTER, GL_LINEAR);
            glTexParameteri(GL_TEXTURE_2D, GL_TEXTURE_MAG_FILTER, GL_LINEAR);

            var data = new Bitmap("./container.jpg");
            var map = data.LockBits(new Rectangle(0, 0, data.Width, data.Height), System.Drawing.Imaging.ImageLockMode.ReadWrite, data.PixelFormat);
            glTexImage2D(GL_TEXTURE_2D, 0, GL_RGB, data.Width, data.Height, 0, GL_BGR, GL_UNSIGNED_BYTE, map.Scan0);
            glGenerateMipmap(GL_TEXTURE_2D);

            data.UnlockBits(map);
            data.Dispose();

            var texture2 = glGenTexture();
            glBindTexture(GL_TEXTURE_2D, texture2);

            glTexParameteri(GL_TEXTURE_2D, GL_TEXTURE_WRAP_S, GL_REPEAT);   // set texture wrapping to GL_REPEAT (default wrapping method)
            glTexParameteri(GL_TEXTURE_2D, GL_TEXTURE_WRAP_T, GL_REPEAT);
            // set texture filtering parameters
            glTexParameteri(GL_TEXTURE_2D, GL_TEXTURE_MIN_FILTER, GL_LINEAR);
            glTexParameteri(GL_TEXTURE_2D, GL_TEXTURE_MAG_FILTER, GL_LINEAR);

            var data2 = new Bitmap("./awesomeface.png");
            data2.RotateFlip(RotateFlipType.RotateNoneFlipY);
            var map2 = data2.LockBits(new Rectangle(0, 0, data2.Width, data2.Height), System.Drawing.Imaging.ImageLockMode.ReadWrite, data2.PixelFormat);
            glTexImage2D(GL_TEXTURE_2D, 0, GL_RGBA, data2.Width, data2.Height, 0, GL_BGRA, GL_UNSIGNED_BYTE, map2.Scan0);
            glGenerateMipmap(GL_TEXTURE_2D);

            data2.UnlockBits(map2);
            data2.Dispose();

            shader.Use();
            shader.SetInt("texture1", 0);
            shader.SetInt("texture2", 1);
            while (!Glfw.WindowShouldClose(window))
            {
                //渲染背景
                glClearColor(0.2f, 0.3f, 0.3f, 1.0f);
                glClear(GL_COLOR_BUFFER_BIT);

                //绘制物体
                glActiveTexture(GL_TEXTURE0);
                glBindTexture(GL_TEXTURE_2D, texture1);
                glActiveTexture(GL_TEXTURE1);
                glBindTexture(GL_TEXTURE_2D, texture2);

                //shader.Use();
                //glBindVertexArray(VAO);
                glDrawElements(GL_TRIANGLES, 6, GL_UNSIGNED_INT, NULL);

                Glfw.SwapBuffers(window);
                Glfw.PollEvents();
            }

            glDeleteVertexArray(VAO);
            glDeleteBuffer(VBO);
            glDeleteBuffer(EBO);
            Glfw.Terminate();
        }
        static unsafe void Main(string[] args)
        {
            P2();
            //P1();
        }
        private static void framebuffer_size_callback(IntPtr window, int width, int height)
        {
            glViewport(0, 0, width, height);
        }
    }
}
