
using Silk.NET.OpenGLES;
using System.Drawing;

namespace OpenGLTest
{
    public class DepthReader : FramebufferReader
    {
        public DepthReader(GL gl) : base(gl)
        {
        }

        public override void Clear()
        {
            gl.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);
        }

        //public byte[] GetDepths(Framebuffer fb, Point[] points, uint texture, uint width, uint height)
        //{
        //    var depths = points.Select(p => fb.GetPixel(p.X, p.Y).First());
        //    fb.Delete();
        //    return depths.ToArray();
        //}

        //public byte GetDepth(int x, int y, uint texture, uint width, uint height)
        //{
        //    var depths = GetDepths(new[] { new Point(x, y) }, texture, width, height);
        //    return depths[0];
        //}
    }
}