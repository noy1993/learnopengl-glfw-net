using Silk.NET.GLFW;
using Silk.NET.OpenGLES;
using System;
using System.IO;
using System.Threading;

namespace HelloTriangle
{
    class Program
    {
        static readonly Glfw GLFW = Glfw.GetApi();
        static GL gl;

        unsafe static void Main(string[] args)
        {
            GLFW.Init();

            var window = GLFW.CreateWindow(800, 800, "LearnOpenGL", null, null);
            if (window == null)
            {
                Console.WriteLine("Failed to create GLFW window");
                GLFW.Terminate();
            }
            GLFW.MakeContextCurrent(window);
            GLFW.SetFramebufferSizeCallback(window, framebuffer_size_callback);
            var dd = GLFW.GetProcAddress;

            gl = GL.GetApi(new GlfwContext(GLFW, window));

            while (!GLFW.WindowShouldClose(window))
            {
                processInput(window);
                gl.ClearColor(0.2f, 0.3f, 0.3f, 1.0f);
                gl.Clear(ClearBufferMask.ColorBufferBit);

                GLFW.SwapBuffers(window);
                GLFW.PollEvents();
                Thread.Sleep(10);
            }
            GLFW.Terminate();
        }

        private static unsafe void framebuffer_size_callback(WindowHandle* window, int width, int height)
        {
            gl.ClearColor(0.2f, 0.3f, 0.3f, 1.0f);
            GLFW.SwapBuffers(window);
        }

        private unsafe static void processInput(WindowHandle* window)
        {
            if (GLFW.GetKey(window, Keys.Escape) == (int)InputAction.Press)
            {
                GLFW.SetWindowShouldClose(window, true);
            }
        }
    }
}