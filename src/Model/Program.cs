using Silk.NET.GLFW;
using Silk.NET.OpenGLES;
using System.Numerics;
using Shader = OpenGL.Extension.Shader;

namespace Model
{
    class Program
    {
        static readonly Glfw GLFW = Glfw.GetApi();
        internal static GL gl;
        static unsafe void Main(string[] args)
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
            gl.Enable(EnableCap.DepthTest);


         
            Shader.sgl = gl;
            var shader = new Shader("./model_loading.vert", "./model_loading.frag");
            var camera = new OpenGL.Extension.Camera();
            shader.Use();

            var path = @"C:\Users\noy\Pictures\backpack\backpack.obj";
            var ourModel = new Model(gl, path);
          
            var postion = new Vector3(1.2f, 1f, 2f);
            var light = new OpenGL.Extension.DirLight()
            {
                Ambient = new Vector3(0.2f, 0.2f, 0.2f),
                Diffuse = new Vector3(0.5f, 0.5f, 0.5f),

                Specular = new Vector3(1.0f, 1.0f, 1.0f),
                Direction = new Vector3(0, 0, 0) - postion,
            };

           
            shader.SetVec3("viewPos", postion);
            shader.SetLight("dirLight", light);

            Matrix4x4 model;
            while (!GLFW.WindowShouldClose(window))
            {
                camera.ProcessInput(GLFW, window);

                //gl.ClearColor(light.Ambient.X, light.Ambient.Y, light.Ambient.Z, 1f);
                gl.ClearColor(0.5f, 0.5f, 0.5f, 1f);
                gl.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);

                shader.Use();
                var projection = Matrix4x4.CreatePerspectiveFieldOfView(camera.Zoom, 800f / 600f, 0.1f, 100f);
                shader.SetMatrix4x4("projection", projection);
                var view = Matrix4x4.CreateLookAt(camera.Position, camera.Position + camera.Front, camera.WorldUp);
                shader.SetMatrix4x4("view", camera.ViewMatrix);

                model = Matrix4x4.CreateTranslation(new Vector3(0, 0, 0));
                shader.SetMatrix4x4("model", model);
                shader.SetVec3("viewPos", camera.Position);

                ourModel.Draw(shader);

                GLFW.SwapBuffers(window);
                GLFW.PollEvents();
            }
        }
        private static unsafe void framebuffer_size_callback(WindowHandle* window, int width, int height)
        {
            gl.Viewport(0, 0, (uint)width, (uint)height);
        }
    }
}