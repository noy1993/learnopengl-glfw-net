using Silk.NET.GLFW;
using Silk.NET.OpenGLES;
using SkiaSharp;
using System.Numerics;

namespace Camera
{
    class Program
    {
        static readonly Glfw GLFW = Glfw.GetApi();
        internal static GL gl;

        public static void Main()
        {
            DrawCameraMoveView();
        }

        public unsafe static void DrawCameraView()
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
            var shader = new OpenGL.Extension.Shader("./coordinate_systems.vert", "./coordinate_systems.frag");

            // activate shader
            shader.Use();

            var vertices = new float[] {
                -0.5f, -0.5f, -0.5f,  0.0f, 0.0f,
                 0.5f, -0.5f, -0.5f,  1.0f, 0.0f,
                 0.5f,  0.5f, -0.5f,  1.0f, 1.0f,
                 0.5f,  0.5f, -0.5f,  1.0f, 1.0f,
                -0.5f,  0.5f, -0.5f,  0.0f, 1.0f,
                -0.5f, -0.5f, -0.5f,  0.0f, 0.0f,

                -0.5f, -0.5f,  0.5f,  0.0f, 0.0f,
                 0.5f, -0.5f,  0.5f,  1.0f, 0.0f,
                 0.5f,  0.5f,  0.5f,  1.0f, 1.0f,
                 0.5f,  0.5f,  0.5f,  1.0f, 1.0f,
                -0.5f,  0.5f,  0.5f,  0.0f, 1.0f,
                -0.5f, -0.5f,  0.5f,  0.0f, 0.0f,

                -0.5f,  0.5f,  0.5f,  1.0f, 0.0f,
                -0.5f,  0.5f, -0.5f,  1.0f, 1.0f,
                -0.5f, -0.5f, -0.5f,  0.0f, 1.0f,
                -0.5f, -0.5f, -0.5f,  0.0f, 1.0f,
                -0.5f, -0.5f,  0.5f,  0.0f, 0.0f,
                -0.5f,  0.5f,  0.5f,  1.0f, 0.0f,

                 0.5f,  0.5f,  0.5f,  1.0f, 0.0f,
                 0.5f,  0.5f, -0.5f,  1.0f, 1.0f,
                 0.5f, -0.5f, -0.5f,  0.0f, 1.0f,
                 0.5f, -0.5f, -0.5f,  0.0f, 1.0f,
                 0.5f, -0.5f,  0.5f,  0.0f, 0.0f,
                 0.5f,  0.5f,  0.5f,  1.0f, 0.0f,

                -0.5f, -0.5f, -0.5f,  0.0f, 1.0f,
                 0.5f, -0.5f, -0.5f,  1.0f, 1.0f,
                 0.5f, -0.5f,  0.5f,  1.0f, 0.0f,
                 0.5f, -0.5f,  0.5f,  1.0f, 0.0f,
                -0.5f, -0.5f,  0.5f,  0.0f, 0.0f,
                -0.5f, -0.5f, -0.5f,  0.0f, 1.0f,

                -0.5f,  0.5f, -0.5f,  0.0f, 1.0f,
                 0.5f,  0.5f, -0.5f,  1.0f, 1.0f,
                 0.5f,  0.5f,  0.5f,  1.0f, 0.0f,
                 0.5f,  0.5f,  0.5f,  1.0f, 0.0f,
                -0.5f,  0.5f,  0.5f,  0.0f, 0.0f,
                -0.5f,  0.5f, -0.5f,  0.0f, 1.0f
            };

            Vector3[] cubePositions = new Vector3[]
            {
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

            //send buffer
            var vbo = gl.GenBuffer();
            gl.BindBuffer(BufferTargetARB.ArrayBuffer, vbo);
            fixed (float* v = vertices)
            {
                gl.BufferData(BufferTargetARB.ArrayBuffer, (nuint)(vertices.Length * sizeof(float)), v, BufferUsageARB.StaticDraw);
            }

            //data
            var vao = gl.GenBuffer();
            gl.BindVertexArray(vao);
            gl.VertexAttribPointer(0, 3, VertexAttribPointerType.Float, false, 5 * sizeof(float), null);
            gl.EnableVertexAttribArray(0);

            gl.VertexAttribPointer(1, 2, VertexAttribPointerType.Float, false, 5 * sizeof(float), (void*)(3 * sizeof(float)));
            gl.EnableVertexAttribArray(1);

            var texture = gl.GenTexture();
            gl.ActiveTexture(TextureUnit.Texture0);
            gl.BindTexture(TextureTarget.Texture2D, texture);

            gl.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapS, (int)GLEnum.Repeat);
            gl.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapS, (int)GLEnum.Repeat);

            gl.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)GLEnum.LinearMipmapLinear);
            gl.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)GLEnum.Linear);

            using var bitmap = SKBitmap.Decode("./container.jpg");
            gl.TexImage2D(TextureTarget.Texture2D, 0, InternalFormat.Rgba, (uint)bitmap.Width, (uint)bitmap.Height, 0, PixelFormat.Bgra, PixelType.UnsignedByte, bitmap.GetPixelSpan());
            gl.GenerateMipmap(TextureTarget.Texture2D);

            var texture2 = gl.GenTexture();
            gl.ActiveTexture(TextureUnit.Texture1);
            gl.BindTexture(TextureTarget.Texture2D, texture2);

            gl.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapS, (int)GLEnum.Repeat);
            gl.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapS, (int)GLEnum.Repeat);

            gl.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)GLEnum.LinearMipmapLinear);
            gl.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)GLEnum.Linear);

            using var bitmap2 = SKBitmap.Decode("./awesomeface.png");
            gl.TexImage2D(TextureTarget.Texture2D, 0, InternalFormat.Rgba, (uint)bitmap2.Width, (uint)bitmap2.Height, 0, PixelFormat.Bgra, PixelType.UnsignedByte, bitmap2.GetPixelSpan());
            gl.GenerateMipmap(TextureTarget.Texture2D);

            gl.Enable(EnableCap.DepthTest);

            shader.SetInt("texture1", 0);
            shader.SetInt("texture2", 1);

            //静态变换矩阵
            var projection = Matrix4x4.CreatePerspectiveFieldOfView(MathF.PI / 4f, 800 / 600, 0.1f, 100f);
            shader.SetMatrix4x4("projection", projection);
            shader.Use();

            while (!GLFW.WindowShouldClose(window))
            {
                gl.ClearColor(0.2f, 0.3f, 0.3f, 1.0f);
                gl.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);

                //动态模型矩阵
                for (int i = 0; i < cubePositions.Length; i++)
                {
                    var model = Matrix4x4.CreateTranslation(cubePositions[i]);
                    var time = (float)GLFW.GetTime();

                    var angle = MathF.PI / 4f;//* (i + 1);
                    var modelR = Matrix4x4.CreateRotationY(angle * time, Vector3.Zero) * Matrix4x4.CreateRotationX(angle * time, Vector3.Zero);
                    model = modelR * model;
                    shader.SetMatrix4x4("model", model);

                    float radius = 10.0f;
                    float camX = MathF.Sin(time) * radius;
                    float camZ = MathF.Cos(time) * radius;
                    var view = Matrix4x4.CreateLookAt(new Vector3(camX, 0, camZ), new Vector3(0, 0, 0), new Vector3(0, 1, 0));
                    shader.SetMatrix4x4("view", view);
                    gl.DrawArrays(PrimitiveType.Triangles, 0, 36);//正方体6个面 12个三角面 12 * 3 = 36 个顶点
                }

                GLFW.SwapBuffers(window);
                GLFW.PollEvents();
            }
        }


        public unsafe static void DrawCameraMoveView()
        {
            GLFW.Init();

            var window = GLFW.CreateWindow(800, 600, "LearnOpenGL", null, null);
            if (window == null)
            {
                Console.WriteLine("Failed to create GLFW window");
                GLFW.Terminate();
            }

            GLFW.MakeContextCurrent(window);
            //GLFW.SetInputMode(window, CursorStateAttribute.Cursor,CursorModeValue.CursorDisabled);
            GLFW.SetFramebufferSizeCallback(window, framebuffer_size_callback);
            GLFW.SetCursorPosCallback(window, cursor_pos_callback);
            GLFW.SetScrollCallback(window, scroll_callback);

            gl = GL.GetApi(new GlfwContext(GLFW, window));

            OpenGL.Extension.Shader.sgl = gl;
            var shader = new OpenGL.Extension.Shader("./coordinate_systems.vert", "./coordinate_systems.frag");

            // activate shader
            shader.Use();

            var vertices = new float[] {
                -0.5f, -0.5f, -0.5f,  0.0f, 0.0f,
                 0.5f, -0.5f, -0.5f,  1.0f, 0.0f,
                 0.5f,  0.5f, -0.5f,  1.0f, 1.0f,
                 0.5f,  0.5f, -0.5f,  1.0f, 1.0f,
                -0.5f,  0.5f, -0.5f,  0.0f, 1.0f,
                -0.5f, -0.5f, -0.5f,  0.0f, 0.0f,

                -0.5f, -0.5f,  0.5f,  0.0f, 0.0f,
                 0.5f, -0.5f,  0.5f,  1.0f, 0.0f,
                 0.5f,  0.5f,  0.5f,  1.0f, 1.0f,
                 0.5f,  0.5f,  0.5f,  1.0f, 1.0f,
                -0.5f,  0.5f,  0.5f,  0.0f, 1.0f,
                -0.5f, -0.5f,  0.5f,  0.0f, 0.0f,

                -0.5f,  0.5f,  0.5f,  1.0f, 0.0f,
                -0.5f,  0.5f, -0.5f,  1.0f, 1.0f,
                -0.5f, -0.5f, -0.5f,  0.0f, 1.0f,
                -0.5f, -0.5f, -0.5f,  0.0f, 1.0f,
                -0.5f, -0.5f,  0.5f,  0.0f, 0.0f,
                -0.5f,  0.5f,  0.5f,  1.0f, 0.0f,

                 0.5f,  0.5f,  0.5f,  1.0f, 0.0f,
                 0.5f,  0.5f, -0.5f,  1.0f, 1.0f,
                 0.5f, -0.5f, -0.5f,  0.0f, 1.0f,
                 0.5f, -0.5f, -0.5f,  0.0f, 1.0f,
                 0.5f, -0.5f,  0.5f,  0.0f, 0.0f,
                 0.5f,  0.5f,  0.5f,  1.0f, 0.0f,

                -0.5f, -0.5f, -0.5f,  0.0f, 1.0f,
                 0.5f, -0.5f, -0.5f,  1.0f, 1.0f,
                 0.5f, -0.5f,  0.5f,  1.0f, 0.0f,
                 0.5f, -0.5f,  0.5f,  1.0f, 0.0f,
                -0.5f, -0.5f,  0.5f,  0.0f, 0.0f,
                -0.5f, -0.5f, -0.5f,  0.0f, 1.0f,

                -0.5f,  0.5f, -0.5f,  0.0f, 1.0f,
                 0.5f,  0.5f, -0.5f,  1.0f, 1.0f,
                 0.5f,  0.5f,  0.5f,  1.0f, 0.0f,
                 0.5f,  0.5f,  0.5f,  1.0f, 0.0f,
                -0.5f,  0.5f,  0.5f,  0.0f, 0.0f,
                -0.5f,  0.5f, -0.5f,  0.0f, 1.0f
            };

            Vector3[] cubePositions = new Vector3[]
            {
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

            //send buffer
            var vbo = gl.GenBuffer();
            gl.BindBuffer(BufferTargetARB.ArrayBuffer, vbo);
            fixed (float* v = vertices)
            {
                gl.BufferData(BufferTargetARB.ArrayBuffer, (nuint)(vertices.Length * sizeof(float)), v, BufferUsageARB.StaticDraw);
            }

            //data
            var vao = gl.GenBuffer();
            gl.BindVertexArray(vao);
            gl.VertexAttribPointer(0, 3, VertexAttribPointerType.Float, false, 5 * sizeof(float), null);
            gl.EnableVertexAttribArray(0);

            gl.VertexAttribPointer(1, 2, VertexAttribPointerType.Float, false, 5 * sizeof(float), (void*)(3 * sizeof(float)));
            gl.EnableVertexAttribArray(1);

            var texture = gl.GenTexture();
            gl.ActiveTexture(TextureUnit.Texture0);
            gl.BindTexture(TextureTarget.Texture2D, texture);

            gl.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapS, (int)GLEnum.Repeat);
            gl.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapS, (int)GLEnum.Repeat);

            gl.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)GLEnum.LinearMipmapLinear);
            gl.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)GLEnum.Linear);

            using var bitmap = SKBitmap.Decode("./container.jpg");
            gl.TexImage2D(TextureTarget.Texture2D, 0, InternalFormat.Rgba, (uint)bitmap.Width, (uint)bitmap.Height, 0, PixelFormat.Bgra, PixelType.UnsignedByte, bitmap.GetPixelSpan());
            gl.GenerateMipmap(TextureTarget.Texture2D);

            var texture2 = gl.GenTexture();
            gl.ActiveTexture(TextureUnit.Texture1);
            gl.BindTexture(TextureTarget.Texture2D, texture2);

            gl.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapS, (int)GLEnum.Repeat);
            gl.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapS, (int)GLEnum.Repeat);

            gl.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)GLEnum.LinearMipmapLinear);
            gl.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)GLEnum.Linear);

            using var bitmap2 = SKBitmap.Decode("./awesomeface.png");
            gl.TexImage2D(TextureTarget.Texture2D, 0, InternalFormat.Rgba, (uint)bitmap2.Width, (uint)bitmap2.Height, 0, PixelFormat.Bgra, PixelType.UnsignedByte, bitmap2.GetPixelSpan());
            gl.GenerateMipmap(TextureTarget.Texture2D);

            gl.Enable(EnableCap.DepthTest);

            shader.SetInt("texture1", 0);
            shader.SetInt("texture2", 1);

            shader.Use();

            while (!GLFW.WindowShouldClose(window))
            {
                processInput(window);
                gl.ClearColor(0.2f, 0.3f, 0.3f, 1.0f);
                gl.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);

                var projection = Matrix4x4.CreatePerspectiveFieldOfView(fov, 800f / 600f, 0.1f, 100f);
                shader.SetMatrix4x4("projection", projection);

                var view = Matrix4x4.CreateLookAt(cameraPos, cameraPos + cameraFront, cameraUp);
                shader.SetMatrix4x4("view", view);

                for (int i = 0; i < cubePositions.Length; i++)
                {
                    var model = Matrix4x4.CreateTranslation(cubePositions[i]);
                    var time = (float)GLFW.GetTime();

                    var angle = MathF.PI / 4f;//* (i + 1);
                    var modelR = Matrix4x4.CreateRotationY(angle * time, Vector3.Zero) * Matrix4x4.CreateRotationX(angle * time, Vector3.Zero);
                    model = modelR * model;
                    shader.SetMatrix4x4("model", model);

                    gl.DrawArrays(PrimitiveType.Triangles, 0, 36);//正方体6个面 12个三角面 12 * 3 = 36 个顶点
                }

                GLFW.SwapBuffers(window);
                GLFW.PollEvents();
            }
        }

        static Vector3 cameraPos = new Vector3(0, 0, 3f);
        static Vector3 cameraFront = new Vector3(0, 0, -1f);
        static Vector3 cameraUp = new Vector3(0, 1f, 0);

        static float deltaTime = 0f;
        static float lastFrame = 0f;
        static float yaw = 1.5f * MathF.PI;
        static float pitch = 0;
        private static unsafe void processInput(WindowHandle* window)
        {
            float currentFrame = (float)GLFW.GetTime();
            deltaTime = currentFrame - lastFrame;
            lastFrame = currentFrame;

            var cameraSpeed = 2.5f * deltaTime;
            if (GLFW.GetKey(window, Keys.W) == (int)InputAction.Press)
            {
                cameraPos += cameraSpeed * cameraFront;
            }
            if (GLFW.GetKey(window, Keys.S) == (int)InputAction.Press)
            {
                cameraPos -= cameraSpeed * cameraFront;
            }
            if (GLFW.GetKey(window, Keys.A) == (int)InputAction.Press)
            {
                cameraPos -= Vector3.Normalize(Vector3.Cross(cameraFront, cameraUp)) * cameraSpeed;
            }
            if (GLFW.GetKey(window, Keys.D) == (int)InputAction.Press)
            {
                cameraPos += Vector3.Normalize(Vector3.Cross(cameraFront, cameraUp)) * cameraSpeed;
            }

            var cameraRotate = 0.5f * deltaTime;
            if (GLFW.GetKey(window, Keys.Q) == (int)InputAction.Press)
            {
                yaw -= cameraRotate;
                var x = MathF.Cos(yaw);
                var z = MathF.Sin(yaw);
                cameraFront = new Vector3(x, 0, z);
            }
            if (GLFW.GetKey(window, Keys.E) == (int)InputAction.Press)
            {
                yaw += cameraRotate;
                var x = MathF.Cos(yaw);
                var z = MathF.Sin(yaw);
                cameraFront = new Vector3(x, 0, z);
            }
        }

        private static unsafe void framebuffer_size_callback(WindowHandle* window, int width, int height)
        {
            gl.Viewport(0, 0, (uint)width, (uint)height);
        }

        static float fov = MathF.PI / 3;
        private static unsafe void scroll_callback(WindowHandle* window, double offsetX, double offsetY)
        {
            if (fov >= 0.01f && fov <= MathF.PI / 4)
                fov -= (float)offsetY / 90 * MathF.PI;
            if (fov <= 0.01f)
                fov = 0.01f;
            if (fov >= MathF.PI / 4)
                fov = MathF.PI / 4;
        }

        static double lastX = 400;
        static double lastY = 300;

        static bool first = true;
        private static unsafe void cursor_pos_callback(WindowHandle* window, double x, double y)
        {
            if (first)
            {
                lastX = x;
                lastY = y;
                first = false;
            }

            var xoffset = x - lastX;
            var yoffset = lastY - y;
            lastX = x;
            lastY = y;

            yaw += (float)xoffset / 400f;
            pitch += (float)yoffset / 300f;
            if (pitch > 0.5 * MathF.PI) pitch = 0.45f * MathF.PI;
            if (pitch < -0.5 * MathF.PI) pitch = -0.45f * MathF.PI;

            var x1 = MathF.Cos(yaw) * MathF.Cos(pitch);
            var z1 = MathF.Sin(yaw) * MathF.Cos(pitch);
            var y1 = MathF.Sin(pitch);
            cameraFront = new Vector3(x1, y1, z1);
        }
    }
}
