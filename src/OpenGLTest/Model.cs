
using Mesh;
using Silk.NET.OpenGLES;
using SkiaSharp;
using Texture = Mesh.Texture;

namespace Model
{
    public class Model
    {
        public Model(GL gl, string path)
        {
            this.gl = gl;
            loadModel(path);
        }

        private void loadModel(string path)
        {
            using Assimp.AssimpContext context = new Assimp.AssimpContext();
            var scene = context.ImportFile(path, Assimp.PostProcessSteps.Triangulate);
            if (scene == null || scene.SceneFlags == Assimp.SceneFlags.Incomplete)
            {
                Console.WriteLine("error"); return;
            }

            directory = Path.GetDirectoryName(path);
            processNode(scene.RootNode, scene);
        }

        private void processNode(Assimp.Node node, Assimp.Scene scene)
        {
            for (int i = 0; i < node.MeshCount; i++)
            {
                var mesh = scene.Meshes[node.MeshIndices[i]];
                meshes.Add(processMesh(mesh, scene));
            }
            for (int i = 0; i < node.ChildCount; i++)
            {
                processNode(node.Children[i], scene);
            }
        }

        private Mesh.Mesh processMesh(Assimp.Mesh mesh, Assimp.Scene scene)
        {
            var vertices = new List<Vertex>();
            var indices = new List<uint>();
            var textures = new List<Texture>();

            for (int i = 0; i < mesh.VertexCount; i++)
            {
                var vertex = new Vertex();
                var v = mesh.Vertices[i];

                vertex.Position = new System.Numerics.Vector3(v.X, v.Y, v.Z);
                if (mesh.HasNormals)
                {
                    var n = mesh.Normals[i];
                    vertex.Normal = new System.Numerics.Vector3(n.X, n.Y, n.Z);
                }

                if (mesh.TextureCoordinateChannels[0].Count > 0)
                {
                    var tex = mesh.TextureCoordinateChannels[0][i];
                    vertex.TexCoords = new System.Numerics.Vector2(tex.X, tex.Y);
                }
                else
                {
                    vertex.TexCoords = new System.Numerics.Vector2(0f, 0f);
                }
                vertices.Add(vertex);
            }

            for (int i = 0; i < mesh.FaceCount; i++)
            {
                var face = mesh.Faces[i];
                for (int j = 0; j < face.IndexCount; j++)
                {
                    indices.Add((uint)face.Indices[j]);
                }
            }

            if (mesh.MaterialIndex >= 0)
            {
                var mat = scene.Materials[mesh.MaterialIndex];
                var diffuseMaps = loadMaterialTextures(mat, Assimp.TextureType.Diffuse, "texture_diffuse");
                var specularMaps = loadMaterialTextures(mat, Assimp.TextureType.Specular, "texture_specular");

                textures.AddRange(diffuseMaps);
                textures.AddRange(specularMaps);
            }
            return new Mesh.Mesh(gl, vertices, indices, textures);
        }

        private List<Texture> loadMaterialTextures(Assimp.Material mat, Assimp.TextureType type, string typename)
        {
            List<Texture> textures = new List<Texture>();
            for (int i = 0; i < mat.GetMaterialTextureCount(type); i++)
            {
                mat.GetMaterialTexture(type, i, out var str);

                textures.Add(new Texture()
                {
                    Id = TextureFromFile(str.FilePath),
                    Type = typename,
                    FileName = str.FilePath
                });
            }
            return textures;
        }

        Dictionary<string, uint> file_texture = new Dictionary<string, uint>();
        private uint TextureFromFile(string image_filename)
        {
            if (file_texture.ContainsKey(image_filename))
            {
                return file_texture[image_filename];
            }
            else
            {
                var texture = gl.GenTexture();
                gl.BindTexture(TextureTarget.Texture2D, texture);

                gl.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapS, (int)GLEnum.Repeat); // set texture wrapping to gl._REPEAT (default wrapping method)
                gl.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapT, (int)GLEnum.Repeat);

                // set texture filtering parameters
                gl.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)GLEnum.Linear);
                gl.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)GLEnum.Linear);

                gl.GenerateMipmap(TextureTarget.Texture2D);
                using var bitmap = SKBitmap.Decode(Path.Combine(directory, image_filename));
                gl.TexImage2D(TextureTarget.Texture2D, 0, InternalFormat.Rgba, (uint)bitmap.Width, (uint)bitmap.Height, 0, PixelFormat.Bgra, PixelType.UnsignedByte, bitmap.GetPixelSpan());
                file_texture.Add(image_filename, texture);
                return texture;
            }

        }

        public void Draw(OpenGL.Extension.Shader shader)
        {
            for (int i = 0; i < meshes.Count; i++)
            {
                meshes[i].Draw(shader);
            }
        }

        List<Mesh.Mesh> meshes = new List<Mesh.Mesh>();
        string directory;
        private readonly GL gl;
    }
}