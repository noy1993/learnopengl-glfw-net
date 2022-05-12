
using Silk.NET.OpenGLES;

namespace OpenGLTest
{
    public unsafe abstract class FramebufferReader
    {
        //resources
        private const string texUniform = "screenTexture";
        private const string depth_fragment_shader = @"#version 300 es
precision mediump float;
in vec2 TexCoords;
uniform sampler2D screenTexture;
void main()
{    
    FragColor = texture(screenTexture, TexCoords);
}
";
        private const string depth_vertex_shader = @"#version 300 es
layout (location = 0) in vec2 aPos;
layout (location = 1) in vec2 aTexCoord;

out vec2 TexCoords;

void main()
{
    TexCoords = aTexCoord;    
    gl_Position = vec4(aPos, 0.0, 1.0);
}
";
        private static float[] screen_vertices = new float[]
        {
             // positions   // texCoords
             1.0f,  1.0f,  0.0f, 1.0f,
             1.0f, -1.0f,  0.0f, 0.0f,
             1.0f, -1.0f,  1.0f, 0.0f,

             1.0f,  1.0f,  0.0f, 1.0f,
             1.0f, -1.0f,  1.0f, 0.0f,
             1.0f,  1.0f,  1.0f, 1.0f
        };

        protected readonly ShaderFile program;
        protected readonly GL gl;
        protected uint screenVAO;

        public FramebufferReader(GL gl)
        {
            program = new ShaderFile(gl, depth_vertex_shader, depth_fragment_shader);
            screenVAO = gl.GenVertexArray();
            gl.BindVertexArray(screenVAO);

            var screenVBO = gl.GenBuffer();
            gl.BindBuffer(BufferTargetARB.ArrayBuffer, screenVBO);
            fixed (float* v = &screen_vertices[0])
            {
                gl.BufferData(BufferTargetARB.ArrayBuffer, (nuint)(sizeof(float) * screen_vertices.Length), v, BufferUsageARB.StaticDraw);
            }
            gl.EnableVertexAttribArray(0);
            gl.VertexAttribPointer(0, 2, VertexAttribPointerType.Float, false, 4 * sizeof(float), null);
            gl.EnableVertexAttribArray(1);
            gl.VertexAttribPointer(1, 2, VertexAttribPointerType.Float, false, 4 * sizeof(float), (void*)(2 * sizeof(float)));

            gl.BindVertexArray(0);
            this.gl = gl;
            this.framebuffer = framebuffer;
        }

        public abstract void Clear();

        public void Draw(uint texture)
        {
            program.Use();
            gl.ActiveTexture(TextureUnit.Texture0);
            gl.BindTexture(TextureTarget.Texture2D, texture);
            program.SetInt(texUniform, 0);

            gl.BindVertexArray(screenVAO);
            gl.ClearColor(0, 0, 0, 0);
            Clear();
            gl.DrawArrays(PrimitiveType.Triangles, 0, 6);

            gl.BindVertexArray(0);
        }

        public virtual void Delete()
        {
            gl.DeleteVertexArray(screenVAO);
            program.Delete();
        }
    }
}