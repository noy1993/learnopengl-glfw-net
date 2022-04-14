using System;
using System.IO;
using OpenTK.Graphics.ES30;
using OpenTK.Windowing.GraphicsLibraryFramework;

namespace HelloTriangle
{
    class Program
    {
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

            GL.LoadBindings(new GLFWBindingsContext());
            while (!GLFW.WindowShouldClose(window))
            {
                processInput(window);
                GL.ClearColor(0.2f, 0.3f, 0.3f, 1.0f);
                GL.Clear(ClearBufferMask.ColorBufferBit);

                GLFW.SwapBuffers(window);
                GLFW.PollEvents();
            }
            GLFW.Terminate();
        }

        private static unsafe void framebuffer_size_callback(Window* window, int width, int height)
        {
            GL.Viewport(0, 0, width, height);
        }

        private unsafe static void processInput(Window* window)
        {
            if (GLFW.GetKey(window, Keys.Escape) == InputAction.Press)
            {
                GLFW.SetWindowShouldClose(window, true);
            }
        }
    }
}