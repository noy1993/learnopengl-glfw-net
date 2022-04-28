using Silk.NET.Assimp;
using Silk.NET.GLFW;
using Silk.NET.OpenGLES;

namespace Model
{
    public class Model
    {
        public Model(string path)
        {
            loadModel(path);
        }

        private void loadModel(string path)
        {
            var assimp = Assimp.GetApi();

            assimp.ImportFile(path);
        }

        public void Draw(OpenGL.Extension.Shader shader)
        {
            for (int i = 0; i < meshes.Count; i++)
            {
                meshes[i].Draw(shader);
            }
        }

        List<Mesh.Mesh> meshes;
        string directory;
    }

    class Program
    {
        static readonly Glfw GLFW = Glfw.GetApi();
        internal static GL gl;
        static unsafe void Main(string[] args)
        {

        }
    }
}