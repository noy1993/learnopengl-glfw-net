
using Silk.NET.GLFW;
using Silk.NET.OpenGLES;
using System.Numerics;

namespace StencilTesting
{
    //模板测试
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
                Console.WriteLine("Failed to create gl.FW window");
                GLFW.Terminate();
            }
            GLFW.MakeContextCurrent(window);
            GLFW.SetFramebufferSizeCallback(window, framebuffer_size_callback);

            gl = GL.GetApi(new GlfwContext(GLFW, window));
            OpenGL.Extension.Shader.sgl = gl;
            OpenGL.Extension.Shader shader = new OpenGL.Extension.Shader("./stencil_testing.vert", "./stencil_testing.frag");
            var cubeVertices = new float[]{
                // positions          // texture Coords
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
            var planeVertices = new float[]{
                // positions          // texture Coords (note we set these higher than 1 (together with GL_REPEAT as texture wrapping mode). this will cause the floor texture to repeat)
                 5.0f, -0.5f,  5.0f,  2.0f, 0.0f,
                -5.0f, -0.5f,  5.0f,  0.0f, 0.0f,
                -5.0f, -0.5f, -5.0f,  0.0f, 2.0f,

                 5.0f, -0.5f,  5.0f,  2.0f, 0.0f,
                -5.0f, -0.5f, -5.0f,  0.0f, 2.0f,
                 5.0f, -0.5f, -5.0f,  2.0f, 2.0f
            };

            //cube
            var cubeVAO = gl.GenVertexArray();
            gl.BindVertexArray(cubeVAO);

            var cubeVBO = gl.GenBuffer();
            gl.BindBuffer(BufferTargetARB.ArrayBuffer, cubeVBO);
            fixed (float* v = &cubeVertices[0])
            {
                gl.BufferData(BufferTargetARB.ArrayBuffer, (nuint)(sizeof(float) * cubeVertices.Length), v, BufferUsageARB.StreamDraw);
            }

            gl.EnableVertexAttribArray(0);
            gl.VertexAttribPointer(0, 3, VertexAttribPointerType.Float, false, 5 * sizeof(float), null);
            gl.EnableVertexAttribArray(1);
            gl.VertexAttribPointer(1, 2, VertexAttribPointerType.Float, false, 5 * sizeof(float), (void*)(3 * sizeof(float)));
            gl.BindVertexArray(0);

            //plane
            var planeVAO = gl.GenVertexArray();
            gl.BindVertexArray(planeVAO);

            var planeVBO = gl.GenBuffer();
            gl.BindBuffer(BufferTargetARB.ArrayBuffer, planeVBO);
            fixed (float* v = &planeVertices[0])
            {
                gl.BufferData(BufferTargetARB.ArrayBuffer, (nuint)(sizeof(float) * planeVertices.Length), v, BufferUsageARB.StreamDraw);
            }

            gl.EnableVertexAttribArray(0);
            gl.VertexAttribPointer(0, 3, VertexAttribPointerType.Float, false, 5 * sizeof(float), null);
            gl.EnableVertexAttribArray(1);
            gl.VertexAttribPointer(1, 2, VertexAttribPointerType.Float, false, 5 * sizeof(float), (void*)(3 * sizeof(float)));
            gl.BindVertexArray(0);

            shader.Use();
            var cubeTexture = loadTexture("./marble.jpg", 0);
            var floorTexture = loadTexture("./metal.png", 1);

            shader.Use();
            shader.SetInt("texturel", 0);

            gl.Enable(EnableCap.DepthTest);
            gl.DepthFunc(DepthFunction.Less);
            //gl.DepthMask(false);
            var camera = new OpenGL.Extension.Camera();
            gl.ClearColor(0.1f, 0.1f, 0.1f, 1.0f);
            while (!GLFW.WindowShouldClose(window))
            {
                camera.ProcessInput(GLFW, window);

                gl.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);

                var currentFrame = (float)GLFW.GetTime();
                deltaTime = currentFrame - lastFrame;
                lastFrame = currentFrame;

                shader.Use();
                gl.BindVertexArray(cubeVAO);

                gl.ActiveTexture(TextureUnit.Texture0);
                gl.BindTexture(TextureTarget.Texture2D, cubeTexture);

                var projection = Matrix4x4.CreatePerspectiveFieldOfView(camera.Zoom, 800f / 600f, 0.1f, 100f);
                shader.SetMatrix4x4("projection", projection);
                var view = Matrix4x4.CreateLookAt(camera.Position, camera.Position + camera.Front, camera.WorldUp);
                shader.SetMatrix4x4("view", camera.ViewMatrix);

                var model = Matrix4x4.CreateTranslation(new Vector3(-1.0f, 0.0f, -1.0f));
                shader.SetMatrix4x4("model", model);
                gl.DrawArrays(PrimitiveType.Triangles, 0, 36);

                model = Matrix4x4.CreateTranslation(new Vector3(2.0f, 0.0f, 0.0f));
                shader.SetMatrix4x4("model", model);
                gl.DrawArrays(PrimitiveType.Triangles, 0, 36);

                gl.DepthMask(false);
                gl.BindVertexArray(planeVAO);
                gl.BindTexture(TextureTarget.Texture2D, floorTexture);
                shader.SetMatrix4x4("model", Matrix4x4.Identity);
                gl.DrawArrays(PrimitiveType.Triangles, 0, 6);
                gl.DepthMask(true);

                GLFW.SwapBuffers(window);
                GLFW.PollEvents();
            }
        }

        // timing
        static float deltaTime = 0.0f;
        static float lastFrame = 0.0f;

        private static uint loadTexture(string path, int index)
        {
            var textureID = gl.GenTexture();
            using var bitmap = SkiaSharp.SKBitmap.Decode(path);
            gl.BindTexture(TextureTarget.Texture2D, textureID);
            gl.TexImage2D(TextureTarget.Texture2D, 0, InternalFormat.Rgb, (uint)bitmap.Width, (uint)bitmap.Height, 0, PixelFormat.Bgra, PixelType.UnsignedByte, bitmap.GetPixelSpan());
            gl.GenerateMipmap(TextureTarget.Texture2D);

            gl.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapS, (int)GLEnum.Repeat); // set texture wrapping to gl._REPEAT (default wrapping method)
            gl.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapT, (int)GLEnum.Repeat);

            // set texture filtering parameters
            gl.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)GLEnum.LinearMipmapLinear);
            gl.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)GLEnum.Linear);
            return textureID;
        }

        private unsafe static void framebuffer_size_callback(WindowHandle* window, int width, int height)
        {
            gl.Viewport(0, 0, (uint)width, (uint)height);
        }
    }
}