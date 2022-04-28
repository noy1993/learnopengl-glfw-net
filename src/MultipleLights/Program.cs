
using Silk.NET.GLFW;
using Silk.NET.OpenGLES;
using System.Numerics;

namespace LightCasters
{

    class Program
    {
        static readonly Glfw GLFW = Glfw.GetApi();
        internal static GL gl;

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
            gl.Enable(EnableCap.DepthTest);
            OpenGL.Extension.Shader.sgl = gl;
            OpenGL.Extension.Shader shader = new("./multiple_lights.vert", "./multiple_lights.frag");
            OpenGL.Extension.Shader lightshader = new("./light.vert", "./light.frag");

            var camera = new OpenGL.Extension.Camera();

            var map = new OpenGL.Extension.GLBitmap(gl, "./container2.png", TextureUnit.Texture0);
            var mat = new OpenGL.Extension.MaterialDiffuseMap()
            {
                Diffuse = map,
                Specular = new Vector3(0.5f, 0.5f, 0.5f),
                Shininess = 32.0f
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

            var cubePositions = new Vector3[]{
                new Vector3( 0.0f,  0.0f,  0.0f),
                new Vector3( 2.0f,  5.0f, -15.0f),
                new Vector3(-1.5f, -2.2f, -2.5f),
                new Vector3(-3.8f, -2.0f, -12.3f),
                new Vector3( 2.4f, -0.4f, -3.5f),
                new Vector3(-1.7f,  3.0f, -7.5f),
                new Vector3( 1.3f, -2.0f, -2.5f),
                new Vector3( 1.5f,  2.0f, -2.5f),
                new Vector3( 1.5f,  0.2f, -1.5f),
                new Vector3(-1.3f,  1.0f, -1.5f)
            };

            var pointLightPositions = new Vector3[]{
                new Vector3( 0.7f,  0.2f,  2.0f),
                new Vector3( 2.3f, -3.3f, -4.0f),
                new Vector3(-4.0f,  2.0f, -12.0f),
                new Vector3( 0.0f,  0.0f, -3.0f)
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
            shader.Use();
            shader.SetMaterial("maiterial", mat);
            var dirLight = new OpenGL.Extension.DirLight()
            {
                Ambient = new Vector3(0.05f, 0.05f, 0.05f),
                Diffuse = new Vector3(0.4f, 0.4f, 0.4f),

                Specular = new Vector3(0.5f, 0.5f, 0.5f),
                Direction = new Vector3(-0.2f, -1.0f, -0.3f),
            };
            shader.SetLight("dirLight", dirLight);

            var pointLights = pointLightPositions.Select(p => new OpenGL.Extension.PointLight
            {
                Position = p,
                Ambient = new Vector3(0.05f, 0.05f, 0.05f),
                Diffuse = new Vector3(0.8f, 0.8f, 0.8f),
                Specular = new Vector3(1f, 1f, 1f),
                Constant = 1f,
                Linear = 0.09f,
                Quadratic = 0.032f
            });
            shader.SetPointLights("pointLights", pointLights);
            var spotLight = new OpenGL.Extension.SpotLight()
            {
                Ambient = new Vector3(0.1f, 0.1f, 0.1f),
                Diffuse = new Vector3(0.8f, 0.8f, 0.8f),

                Specular = new Vector3(1.0f, 1.0f, 1.0f),
                Position = camera.Position,
                Direction = camera.Front,

                Constant = 1,
                Linear = 0.09f,
                Quadratic = 0.032f,
                CutOff = MathF.Cos(MathF.PI * 0.07f),
                OuterCutOff = MathF.Cos(MathF.PI * 0.1f),
            };
            shader.SetLight("spotLight", spotLight);

            Matrix4x4 model;
            while (!GLFW.WindowShouldClose(window))
            {
                camera.ProcessInput(GLFW, window);

                float radius = 10.0f;
                var time = (float)GLFW.GetTime();
                float x = MathF.Sin(time) * radius;
                float y = MathF.Cos(time) * radius;

                //渲染背景
                gl.ClearColor(dirLight.Ambient.X, dirLight.Ambient.Y, dirLight.Ambient.Z, 1f);
                gl.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);

                shader.Use();
                var projection = Matrix4x4.CreatePerspectiveFieldOfView(camera.Zoom, 800f / 600f, 0.1f, 100f);
                shader.SetMatrix4x4("projection", projection);
                var view = Matrix4x4.CreateLookAt(camera.Position, camera.Position + camera.Front, camera.WorldUp);
                shader.SetMatrix4x4("view", camera.ViewMatrix);

                shader.SetVec3("viewPos", camera.Position);

                spotLight.Position = camera.Position;
                spotLight.Direction = camera.Front;
                shader.SetLight("spotLight", spotLight);
                shader.SetMaterial("material", mat);

                for (int i = 0; i < cubePositions.Length; i++)
                {
                    model = Matrix4x4.CreateTranslation(cubePositions[i]);
                    float angle = i * MathF.PI / 4 + time;
                    model = OpenGL.Extension.MatrixExtension.Rotation(angle, new Vector3(1f, 0.3f, 0.5f)) * model;

                    shader.SetMatrix4x4("model", model);
                    gl.DrawArrays(PrimitiveType.Triangles, 0, 36);
                }

                lightshader.Use();
                lightshader.SetMatrix4x4("projection", projection);
                lightshader.SetMatrix4x4("view", view);
                lightshader.SetVec3("lightColor", new Vector3(1f, 1f, 1f));

                for (int i = 0; i < pointLightPositions.Length; i++)
                {
                    model = Matrix4x4.CreateTranslation(pointLightPositions[i]);
                    model = Matrix4x4.CreateScale(0.2f) * model;
                    lightshader.SetMatrix4x4("model", model);
                    gl.DrawArrays(PrimitiveType.Triangles, 0, 36);
                }

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