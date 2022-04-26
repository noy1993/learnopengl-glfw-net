using Silk.NET.GLFW;
using Silk.NET.OpenGLES;
using System.Numerics;

namespace BasicLighting
{
    class Program
    {
        static readonly Glfw GLFW = Glfw.GetApi();
        internal static GL gl;

        public unsafe static void Main()
        {
            BoxMap();
        }

        public unsafe static void BoxDiffuseMap()
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
            OpenGL.Extension.Shader.sgl = gl;
            OpenGL.Extension.Shader shader = new OpenGL.Extension.Shader("./lighting_maps.vert", "./lighting_maps.frag");
            OpenGL.Extension.Shader lightshader = new OpenGL.Extension.Shader("./light.vert", "./light.frag");

            var camera = new OpenGL.Extension.Camera();

            var map = new OpenGL.Extension.GLBitmap(gl, "./container2.png", TextureUnit.Texture0);
            var mat = new OpenGL.Extension.MaterialDiffuseMap()
            {
                Diffuse = map,
                Specular = new Vector3(0.5f, 0.5f, 0.5f),
                Shininess = 32.0f
            };

            var light = new OpenGL.Extension.Light()
            {
                Ambient = new Vector3(0.2f, 0.2f, 0.2f),
                Diffuse = new Vector3(0.5f, 0.5f, 0.5f),

                Specular = new Vector3(1.0f, 1.0f, 1.0f),
                Position = new Vector3(1.2f, 1f, 2f),
            };

            var vertices = new float[] {
                // positions          // normals           // texture coords
                -0.5f, -0.5f, -0.5f,  0.0f,  0.0f, -1.0f,  0.0f,  0.0f,
                 0.5f, -0.5f, -0.5f,  0.0f,  0.0f, -1.0f,  1.0f,  0.0f,
                 0.5f,  0.5f, -0.5f,  0.0f,  0.0f, -1.0f,  1.0f,  1.0f,
                 0.5f,  0.5f, -0.5f,  0.0f,  0.0f, -1.0f,  1.0f,  1.0f,
                -0.5f,  0.5f, -0.5f,  0.0f,  0.0f, -1.0f,  0.0f,  1.0f,
                -0.5f, -0.5f, -0.5f,  0.0f,  0.0f, -1.0f,  0.0f,  0.0f,

                -0.5f, -0.5f,  0.5f,  0.0f,  0.0f,  1.0f,  0.0f,  0.0f,
                 0.5f, -0.5f,  0.5f,  0.0f,  0.0f,  1.0f,  1.0f,  0.0f,
                 0.5f,  0.5f,  0.5f,  0.0f,  0.0f,  1.0f,  1.0f,  1.0f,
                 0.5f,  0.5f,  0.5f,  0.0f,  0.0f,  1.0f,  1.0f,  1.0f,
                -0.5f,  0.5f,  0.5f,  0.0f,  0.0f,  1.0f,  0.0f,  1.0f,
                -0.5f, -0.5f,  0.5f,  0.0f,  0.0f,  1.0f,  0.0f,  0.0f,

                -0.5f,  0.5f,  0.5f, -1.0f,  0.0f,  0.0f,  1.0f,  0.0f,
                -0.5f,  0.5f, -0.5f, -1.0f,  0.0f,  0.0f,  1.0f,  1.0f,
                -0.5f, -0.5f, -0.5f, -1.0f,  0.0f,  0.0f,  0.0f,  1.0f,
                -0.5f, -0.5f, -0.5f, -1.0f,  0.0f,  0.0f,  0.0f,  1.0f,
                -0.5f, -0.5f,  0.5f, -1.0f,  0.0f,  0.0f,  0.0f,  0.0f,
                -0.5f,  0.5f,  0.5f, -1.0f,  0.0f,  0.0f,  1.0f,  0.0f,

                 0.5f,  0.5f,  0.5f,  1.0f,  0.0f,  0.0f,  1.0f,  0.0f,
                 0.5f,  0.5f, -0.5f,  1.0f,  0.0f,  0.0f,  1.0f,  1.0f,
                 0.5f, -0.5f, -0.5f,  1.0f,  0.0f,  0.0f,  0.0f,  1.0f,
                 0.5f, -0.5f, -0.5f,  1.0f,  0.0f,  0.0f,  0.0f,  1.0f,
                 0.5f, -0.5f,  0.5f,  1.0f,  0.0f,  0.0f,  0.0f,  0.0f,
                 0.5f,  0.5f,  0.5f,  1.0f,  0.0f,  0.0f,  1.0f,  0.0f,

                -0.5f, -0.5f, -0.5f,  0.0f, -1.0f,  0.0f,  0.0f,  1.0f,
                 0.5f, -0.5f, -0.5f,  0.0f, -1.0f,  0.0f,  1.0f,  1.0f,
                 0.5f, -0.5f,  0.5f,  0.0f, -1.0f,  0.0f,  1.0f,  0.0f,
                 0.5f, -0.5f,  0.5f,  0.0f, -1.0f,  0.0f,  1.0f,  0.0f,
                -0.5f, -0.5f,  0.5f,  0.0f, -1.0f,  0.0f,  0.0f,  0.0f,
                -0.5f, -0.5f, -0.5f,  0.0f, -1.0f,  0.0f,  0.0f,  1.0f,

                -0.5f,  0.5f, -0.5f,  0.0f,  1.0f,  0.0f,  0.0f,  1.0f,
                 0.5f,  0.5f, -0.5f,  0.0f,  1.0f,  0.0f,  1.0f,  1.0f,
                 0.5f,  0.5f,  0.5f,  0.0f,  1.0f,  0.0f,  1.0f,  0.0f,
                 0.5f,  0.5f,  0.5f,  0.0f,  1.0f,  0.0f,  1.0f,  0.0f,
                -0.5f,  0.5f,  0.5f,  0.0f,  1.0f,  0.0f,  0.0f,  0.0f,
                -0.5f,  0.5f, -0.5f,  0.0f,  1.0f,  0.0f,  0.0f,  1.0f
            };

            uint VBO = gl.GenBuffer();

            gl.BindBuffer(BufferTargetARB.ArrayBuffer, VBO);
            fixed (float* v = &vertices[0])
            {
                gl.BufferData(BufferTargetARB.ArrayBuffer, (nuint)(sizeof(float) * vertices.Length), v, BufferUsageARB.StaticDraw);
            }

            uint VAO = gl.GenVertexArray();
            gl.VertexAttribPointer(0, 3, VertexAttribPointerType.Float, false, 8 * sizeof(float), null);
            gl.EnableVertexAttribArray(0);
            gl.VertexAttribPointer(1, 3, VertexAttribPointerType.Float, false, 8 * sizeof(float), (void*)(3 * sizeof(float)));
            gl.EnableVertexAttribArray(1);
            gl.VertexAttribPointer(2, 2, VertexAttribPointerType.Float, false, 8 * sizeof(float), (void*)(6 * sizeof(float)));
            gl.EnableVertexAttribArray(2);

            shader.SetMaterial("maiterial", mat);
            shader.SetLight("light", light);

            Matrix4x4 model;
            while (!GLFW.WindowShouldClose(window))
            {
                camera.ProcessInput(GLFW, window);

                float radius = 10.0f;
                var time = (float)GLFW.GetTime();
                float x = MathF.Sin(time) * radius;
                float y = MathF.Cos(time) * radius;

                light.Position = new Vector3(x, 1, y);

                //渲染背景
                gl.ClearColor(light.Ambient.X, light.Ambient.Y, light.Ambient.Z, 1f);
                gl.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);

                shader.Use();
                var projection = Matrix4x4.CreatePerspectiveFieldOfView(camera.Zoom, 800f / 600f, 0.1f, 100f);
                shader.SetMatrix4x4("projection", projection);
                var view = Matrix4x4.CreateLookAt(camera.Position, camera.Position + camera.Front, camera.WorldUp);
                shader.SetMatrix4x4("view", camera.ViewMatrix);

                model = Matrix4x4.CreateTranslation(new Vector3(0, 0, 0));
                shader.SetMatrix4x4("model", model);
                shader.SetVec3("viewPos", camera.Position);

                shader.SetLight("light", light);
                shader.SetMaterial("material", mat);

                gl.DrawArrays(PrimitiveType.Triangles, 0, 36);

                lightshader.Use();
                lightshader.SetMatrix4x4("projection", projection);
                lightshader.SetMatrix4x4("view", view);
                lightshader.SetVec3("lightColor", light.Specular);
                model = Matrix4x4.CreateTranslation(light.Position);
                lightshader.SetMatrix4x4("model", model);


                gl.DrawArrays(PrimitiveType.Triangles, 0, 36);
                GLFW.SwapBuffers(window);
                GLFW.PollEvents();
            }
        }

        public unsafe static void BoxMap()
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
            OpenGL.Extension.Shader.sgl = gl;
            OpenGL.Extension.Shader shader = new OpenGL.Extension.Shader("./lighting_maps.vert", "./lighting_maps2.frag");
            OpenGL.Extension.Shader lightshader = new OpenGL.Extension.Shader("./light.vert", "./light.frag");

            var camera = new OpenGL.Extension.Camera();

            var diffuse_map = new OpenGL.Extension.GLBitmap(gl, "./container2.png", TextureUnit.Texture0);
            var specular_map1 = new OpenGL.Extension.GLBitmap(gl, "./container2_specular.png", TextureUnit.Texture1);
            var mat = new OpenGL.Extension.MaterialMap()
            {
                Diffuse = diffuse_map,
                Specular = specular_map1,
                Shininess = 16.0f
            };

            var light = new OpenGL.Extension.Light()
            {
                Ambient = new Vector3(0.2f, 0.2f, 0.2f),
                Diffuse = new Vector3(0.5f, 0.5f, 0.5f),

                Specular = new Vector3(1.0f, 1.0f, 1.0f),
                Position = new Vector3(1.2f, 1f, 2f),
            };

            var vertices = new float[] {
                // positions          // normals           // texture coords
                -0.5f, -0.5f, -0.5f,  0.0f,  0.0f, -1.0f,  0.0f,  0.0f,
                 0.5f, -0.5f, -0.5f,  0.0f,  0.0f, -1.0f,  1.0f,  0.0f,
                 0.5f,  0.5f, -0.5f,  0.0f,  0.0f, -1.0f,  1.0f,  1.0f,
                 0.5f,  0.5f, -0.5f,  0.0f,  0.0f, -1.0f,  1.0f,  1.0f,
                -0.5f,  0.5f, -0.5f,  0.0f,  0.0f, -1.0f,  0.0f,  1.0f,
                -0.5f, -0.5f, -0.5f,  0.0f,  0.0f, -1.0f,  0.0f,  0.0f,

                -0.5f, -0.5f,  0.5f,  0.0f,  0.0f,  1.0f,  0.0f,  0.0f,
                 0.5f, -0.5f,  0.5f,  0.0f,  0.0f,  1.0f,  1.0f,  0.0f,
                 0.5f,  0.5f,  0.5f,  0.0f,  0.0f,  1.0f,  1.0f,  1.0f,
                 0.5f,  0.5f,  0.5f,  0.0f,  0.0f,  1.0f,  1.0f,  1.0f,
                -0.5f,  0.5f,  0.5f,  0.0f,  0.0f,  1.0f,  0.0f,  1.0f,
                -0.5f, -0.5f,  0.5f,  0.0f,  0.0f,  1.0f,  0.0f,  0.0f,

                -0.5f,  0.5f,  0.5f, -1.0f,  0.0f,  0.0f,  1.0f,  0.0f,
                -0.5f,  0.5f, -0.5f, -1.0f,  0.0f,  0.0f,  1.0f,  1.0f,
                -0.5f, -0.5f, -0.5f, -1.0f,  0.0f,  0.0f,  0.0f,  1.0f,
                -0.5f, -0.5f, -0.5f, -1.0f,  0.0f,  0.0f,  0.0f,  1.0f,
                -0.5f, -0.5f,  0.5f, -1.0f,  0.0f,  0.0f,  0.0f,  0.0f,
                -0.5f,  0.5f,  0.5f, -1.0f,  0.0f,  0.0f,  1.0f,  0.0f,

                 0.5f,  0.5f,  0.5f,  1.0f,  0.0f,  0.0f,  1.0f,  0.0f,
                 0.5f,  0.5f, -0.5f,  1.0f,  0.0f,  0.0f,  1.0f,  1.0f,
                 0.5f, -0.5f, -0.5f,  1.0f,  0.0f,  0.0f,  0.0f,  1.0f,
                 0.5f, -0.5f, -0.5f,  1.0f,  0.0f,  0.0f,  0.0f,  1.0f,
                 0.5f, -0.5f,  0.5f,  1.0f,  0.0f,  0.0f,  0.0f,  0.0f,
                 0.5f,  0.5f,  0.5f,  1.0f,  0.0f,  0.0f,  1.0f,  0.0f,

                -0.5f, -0.5f, -0.5f,  0.0f, -1.0f,  0.0f,  0.0f,  1.0f,
                 0.5f, -0.5f, -0.5f,  0.0f, -1.0f,  0.0f,  1.0f,  1.0f,
                 0.5f, -0.5f,  0.5f,  0.0f, -1.0f,  0.0f,  1.0f,  0.0f,
                 0.5f, -0.5f,  0.5f,  0.0f, -1.0f,  0.0f,  1.0f,  0.0f,
                -0.5f, -0.5f,  0.5f,  0.0f, -1.0f,  0.0f,  0.0f,  0.0f,
                -0.5f, -0.5f, -0.5f,  0.0f, -1.0f,  0.0f,  0.0f,  1.0f,

                -0.5f,  0.5f, -0.5f,  0.0f,  1.0f,  0.0f,  0.0f,  1.0f,
                 0.5f,  0.5f, -0.5f,  0.0f,  1.0f,  0.0f,  1.0f,  1.0f,
                 0.5f,  0.5f,  0.5f,  0.0f,  1.0f,  0.0f,  1.0f,  0.0f,
                 0.5f,  0.5f,  0.5f,  0.0f,  1.0f,  0.0f,  1.0f,  0.0f,
                -0.5f,  0.5f,  0.5f,  0.0f,  1.0f,  0.0f,  0.0f,  0.0f,
                -0.5f,  0.5f, -0.5f,  0.0f,  1.0f,  0.0f,  0.0f,  1.0f
            };

            uint VBO = gl.GenBuffer();

            gl.BindBuffer(BufferTargetARB.ArrayBuffer, VBO);
            fixed (float* v = &vertices[0])
            {
                gl.BufferData(BufferTargetARB.ArrayBuffer, (nuint)(sizeof(float) * vertices.Length), v, BufferUsageARB.StaticDraw);
            }

            uint VAO = gl.GenVertexArray();
            gl.VertexAttribPointer(0, 3, VertexAttribPointerType.Float, false, 8 * sizeof(float), null);
            gl.EnableVertexAttribArray(0);
            gl.VertexAttribPointer(1, 3, VertexAttribPointerType.Float, false, 8 * sizeof(float), (void*)(3 * sizeof(float)));
            gl.EnableVertexAttribArray(1);
            gl.VertexAttribPointer(2, 2, VertexAttribPointerType.Float, false, 8 * sizeof(float), (void*)(6 * sizeof(float)));
            gl.EnableVertexAttribArray(2);

            shader.SetMaterial("maiterial", mat);
            shader.SetLight("light", light);

            Matrix4x4 model;
            while (!GLFW.WindowShouldClose(window))
            {
                camera.ProcessInput(GLFW, window);

                float radius = 10.0f;
                var time = (float)GLFW.GetTime();
                float x = MathF.Sin(time) * radius;
                float y = MathF.Cos(time) * radius;

                light.Position = new Vector3(x, 2, y);

                //渲染背景
                gl.ClearColor(light.Ambient.X, light.Ambient.Y, light.Ambient.Z, 1f);
                gl.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);

                shader.Use();
                var projection = Matrix4x4.CreatePerspectiveFieldOfView(camera.Zoom, 800f / 600f, 0.1f, 100f);
                shader.SetMatrix4x4("projection", projection);
                var view = Matrix4x4.CreateLookAt(camera.Position, camera.Position + camera.Front, camera.WorldUp);
                shader.SetMatrix4x4("view", camera.ViewMatrix);

                model = Matrix4x4.CreateTranslation(new Vector3(0, 0, 0));
                shader.SetMatrix4x4("model", model);
                shader.SetVec3("viewPos", camera.Position);

                shader.SetLight("light", light);
                shader.SetMaterial("material", mat);

                gl.DrawArrays(PrimitiveType.Triangles, 0, 36);

                lightshader.Use();
                lightshader.SetMatrix4x4("projection", projection);
                lightshader.SetMatrix4x4("view", view);
                lightshader.SetVec3("lightColor", light.Specular);
                model = Matrix4x4.CreateTranslation(light.Position);
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
