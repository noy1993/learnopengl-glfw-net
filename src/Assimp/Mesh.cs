using Silk.NET.OpenGLES;
using System.Runtime.InteropServices;

namespace OpenGL.Extension
{
    public class Mesh
    {
        public Mesh(GL gl, List<Vertex> vertices, List<uint> indices, List<Texture> textures)
        {
            this.gl = gl;
            Vertices = vertices;
            Indices = indices;
            Textures = textures;
            SetupMesh();
        }

        public List<Vertex> Vertices { get; }
        public List<uint> Indices { get; }
        public List<Texture> Textures { get; }

        public unsafe void Draw(OpenGL.Extension.Shader shader)
        {
            gl.BindVertexArray(vao);
            gl.DrawElements(PrimitiveType.Triangles, (uint)Indices.Count, DrawElementsType.UnsignedInt, null);
            gl.BindVertexArray(0);
        }

        readonly GL gl;
        uint vao, vbo, ebo;
        unsafe void SetupMesh()
        {
            var size = (uint)Marshal.SizeOf<Vertex>();

            vbo = gl.GenBuffer();
            gl.BindBuffer(BufferTargetARB.ArrayBuffer, vbo);
            gl.BufferData(BufferTargetARB.ArrayBuffer, (ReadOnlySpan<Vertex>)CollectionsMarshal.AsSpan(Vertices), BufferUsageARB.StaticDraw);

            vao = gl.GenVertexArray();
            gl.BindVertexArray(vao);

            gl.EnableVertexAttribArray(0);
            gl.VertexAttribPointer(0, 3, VertexAttribPointerType.Float, false, size, null);
            gl.EnableVertexAttribArray(1);
            gl.VertexAttribPointer(1, 3, VertexAttribPointerType.Float, false, size, (void*)(3 * sizeof(float)));
            gl.EnableVertexAttribArray(2);
            gl.VertexAttribPointer(2, 2, VertexAttribPointerType.Float, false, size, (void*)(5 * sizeof(float)));


            ebo = gl.GenBuffer();
            gl.BindBuffer(BufferTargetARB.ElementArrayBuffer, ebo);
            gl.BufferData(BufferTargetARB.ElementArrayBuffer, (ReadOnlySpan<uint>)CollectionsMarshal.AsSpan(Indices), BufferUsageARB.StaticDraw);
        }
    }
}
