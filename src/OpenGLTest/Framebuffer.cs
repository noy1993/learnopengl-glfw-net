using Silk.NET.OpenGLES;

namespace OpenGLTest
{
    public unsafe class Framebuffer
    {
        private readonly GL gl;
        public readonly uint framebuffer;
        public readonly uint renderbuffer;
        public readonly uint texture;
        public readonly uint depthTexture;

        private bool disposed = false;

        private DepthReader? _depthReader;
        public DepthReader DepthReader
        {
            get
            {
                if (_depthReader == null)
                {
                    _depthReader = new DepthReader(gl);
                }
                return _depthReader;
            }
        }

        public Framebuffer(GL gl, uint width, uint height, bool withDepth = false)
        {
            if (width == 0 || height == 0)
            {
                throw new ArgumentException(nameof(width) + " or " + nameof(height));
            }

            framebuffer = gl.GenFramebuffer();
            gl.BindFramebuffer(FramebufferTarget.Framebuffer, framebuffer);

            //如果不需要读取深度，则使用renderbuffer，反之使用 texture
            if (withDepth)
            {
                depthTexture = gl.GenTexture();
                gl.BindTexture(TextureTarget.Texture2D, texture);
                gl.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)GLEnum.Nearest);
                gl.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)GLEnum.Nearest);
                gl.TexImage2D(TextureTarget.Texture2D, 0, InternalFormat.Depth24Stencil8, width, height, 0, PixelFormat.Rgba, PixelType.UnsignedByte, null);
                gl.FramebufferTexture2D(FramebufferTarget.Framebuffer, FramebufferAttachment.DepthStencilAttachment, TextureTarget.Texture2D, depthTexture, 0);
            }
            else
            {
                renderbuffer = gl.GenRenderbuffer();
                gl.BindRenderbuffer(RenderbufferTarget.Renderbuffer, renderbuffer);
                gl.RenderbufferStorage(RenderbufferTarget.Renderbuffer, InternalFormat.Depth24Stencil8, width, height);
                gl.FramebufferRenderbuffer(FramebufferTarget.Framebuffer, FramebufferAttachment.DepthStencilAttachment, RenderbufferTarget.Renderbuffer, renderbuffer);
            }

            //将渲染图像保存到该帧
            texture = gl.GenTexture();
            gl.ActiveTexture(TextureUnit.Texture0);
            gl.BindTexture(TextureTarget.Texture2D, texture);
            gl.TexImage2D(TextureTarget.Texture2D, 0, InternalFormat.Rgba, width, height, 0, PixelFormat.Rgba, PixelType.UnsignedByte, null);

            gl.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)GLEnum.Nearest);
            gl.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)GLEnum.Nearest);

            gl.FramebufferTexture2D(FramebufferTarget.Framebuffer, FramebufferAttachment.ColorAttachment0, TextureTarget.Texture2D, texture, 0);

            if (gl.CheckFramebufferStatus(FramebufferTarget.Framebuffer) != GLEnum.FramebufferComplete)
            {
                throw new Exception("Framebuffer is not complete!");
            }

            this.gl = gl;
            Width = width;
            Height = height;
            WithDepth = withDepth;
        }

        public uint Width { get; }
        public uint Height { get; }
        public bool WithDepth { get; }

        public void Bind()
        {
            gl.BindFramebuffer(FramebufferTarget.Framebuffer, framebuffer);
        }

        public void Unbind()
        {
            gl.BindFramebuffer(FramebufferTarget.Framebuffer, 0);
        }

        public byte[] GetPixel(int x, int y)
        {
            var result = new byte[4];
            gl.ReadPixels(x, y, 1, 1, PixelFormat.Rgba, PixelType.UnsignedByte, result.AsSpan());
            return result;
        }

        public void Delete()
        {
            if (disposed) return;

            gl.DeleteFramebuffer(framebuffer);
            gl.DeleteTexture(texture);
            if (WithDepth)
                gl.DeleteTexture(depthTexture);
            else
                gl.DeleteRenderbuffer(renderbuffer);

            if (_depthReader != null) _depthReader.Delete();
            disposed = true;
        }
    }
}