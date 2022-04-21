using Silk.NET.GLFW;
using Silk.NET.OpenGLES;
using System.Numerics;

namespace BasicLighting
{
    class Program
    {
        static readonly Glfw GLFW = Glfw.GetApi();
        internal static GL gl;

        static Vector3 coral = new Vector3(1f, 0.5f, 0.31f);
        static Vector3 lightColor = new Vector3(1f, 1f, 1f);
        static Vector3 toyColor = coral;
        public unsafe static void Main()
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
            OpenGL.Extension.Shader.sgl = gl;
            OpenGL.Extension.Shader shader = new OpenGL.Extension.Shader("./colors.vert", "./colors.frag");
            OpenGL.Extension.Shader lightshader = new OpenGL.Extension.Shader("./light.vert", "./light.frag");

            OpenGL.Extension.Camera camera = new OpenGL.Extension.Camera();
            var vertices = new float[] {
                -0.5f, -0.5f, -0.5f, 
                 0.5f, -0.5f, -0.5f, 
                 0.5f,  0.5f, -0.5f, 
                 0.5f,  0.5f, -0.5f, 
                -0.5f,  0.5f, -0.5f, 
                -0.5f, -0.5f, -0.5f, 

                -0.5f, -0.5f,  0.5f, 
                 0.5f, -0.5f,  0.5f, 
                 0.5f,  0.5f,  0.5f, 
                 0.5f,  0.5f,  0.5f, 
                -0.5f,  0.5f,  0.5f, 
                -0.5f, -0.5f,  0.5f, 

                -0.5f,  0.5f,  0.5f, 
                -0.5f,  0.5f, -0.5f, 
                -0.5f, -0.5f, -0.5f, 
                -0.5f, -0.5f, -0.5f, 
                -0.5f, -0.5f,  0.5f, 
                -0.5f,  0.5f,  0.5f, 

                 0.5f,  0.5f,  0.5f, 
                 0.5f,  0.5f, -0.5f, 
                 0.5f, -0.5f, -0.5f, 
                 0.5f, -0.5f, -0.5f, 
                 0.5f, -0.5f,  0.5f, 
                 0.5f,  0.5f,  0.5f, 

                -0.5f, -0.5f, -0.5f, 
                 0.5f, -0.5f, -0.5f, 
                 0.5f, -0.5f,  0.5f, 
                 0.5f, -0.5f,  0.5f, 
                -0.5f, -0.5f,  0.5f, 
                -0.5f, -0.5f, -0.5f, 

                -0.5f,  0.5f, -0.5f, 
                 0.5f,  0.5f, -0.5f, 
                 0.5f,  0.5f,  0.5f, 
                 0.5f,  0.5f,  0.5f, 
                -0.5f,  0.5f,  0.5f, 
                -0.5f,  0.5f, -0.5f, 
            };


            uint VBO = gl.GenBuffer();

            gl.BindBuffer(BufferTargetARB.ArrayBuffer, VBO);
            fixed (float* v = &vertices[0])
            {
                gl.BufferData(BufferTargetARB.ArrayBuffer, (nuint)(sizeof(float) * vertices.Length), v, BufferUsageARB.StaticDraw);
            }

            uint VAO = gl.GenVertexArray();
            //告诉OpenGL该如何解析顶点数据
            gl.VertexAttribPointer(0, 3, VertexAttribPointerType.Float, false, 3 * sizeof(float), null);
            //启用顶点属性
            gl.EnableVertexAttribArray(0);

            var lightPos = new Vector3(2, 2, 2);

            Matrix4x4 model;
            while (!GLFW.WindowShouldClose(window))
            {
                camera.ProcessInput(GLFW, window);

                //渲染背景
                gl.ClearColor(0.2f, 0.3f, 0.3f, 1.0f);
                gl.Clear(ClearBufferMask.ColorBufferBit);

                shader.Use();
                var projection = Matrix4x4.CreatePerspectiveFieldOfView(camera.Zoom, 800f / 600f, 0.1f, 100f);
                shader.SetMatrix4x4("projection", projection);
                var view = Matrix4x4.CreateLookAt(camera.Pos, camera.Pos + camera.Front, camera.WorldUp);
                shader.SetMatrix4x4("view", camera.ViewMatrix);

                //draw object
                shader.SetVec3("objectColor", toyColor);
                shader.SetVec3("lightColor", lightColor);
                model = Matrix4x4.CreateTranslation(new Vector3(0, 0, 0));
                shader.SetMatrix4x4("model", model);
                gl.DrawArrays(PrimitiveType.Triangles, 0, 36);

                //draw light
                lightshader.Use();
                lightshader.SetMatrix4x4("projection", projection);
                lightshader.SetMatrix4x4("view", view);

                lightshader.SetVec3("lightColor", lightColor);
                model = Matrix4x4.CreateTranslation(lightPos);
                lightshader.SetMatrix4x4("model", model);
                gl.DrawArrays(PrimitiveType.Triangles, 0, 36);

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
