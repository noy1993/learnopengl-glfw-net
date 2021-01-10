using System;
using System.IO;
using GLFW;
using static OpenGL.Gl;

namespace HelloTriangle
{
    class Program
    {
        static void Main(string[] args)
        {
            Glfw.Init();
            Glfw.WindowHint(Hint.ContextVersionMajor, 3);
            Glfw.WindowHint(Hint.ContextVersionMinor, 3);
            Glfw.WindowHint(Hint.OpenglProfile, Profile.Core);
            //Glfw.WindowHint(Hint.OpenglForwardCompatible, true);

            var window = Glfw.CreateWindow(800, 800, "LearnOpenGL", Monitor.None, Window.None);
            if (window == Window.None)
            {
                Console.WriteLine("Failed to create GLFW window");
                Glfw.Terminate();
            }
            Glfw.MakeContextCurrent(window);
            Glfw.SetFramebufferSizeCallback(window, framebuffer_size_callback);
            Import(Glfw.GetProcAddress);

            while (!Glfw.WindowShouldClose(window))
            {
                processInput(window);
                glClearColor(0.2f, 0.3f, 0.3f, 1.0f);
                glClear(GL_COLOR_BUFFER_BIT);
                Glfw.SwapBuffers(window);
                Glfw.PollEvents();
            }
            Glfw.Terminate();
        }

        private static void processInput(Window window)
        {
            if (Glfw.GetKey(window, Keys.Escape) == InputState.Press)
            {
                Glfw.SetWindowShouldClose(window, true);
            }
        }
        private static void framebuffer_size_callback(IntPtr window, int width, int height)
        {
            glViewport(0, 0, width, height);
        }
    }
}