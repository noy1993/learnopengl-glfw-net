using Silk.NET.GLFW;
using Silk.NET.OpenGLES;
using System.Drawing;
using System.Numerics;

namespace OpenGLTest
{
    public unsafe class Viewer
    {
        public Viewer(GL gl, Camera camera, uint width, uint height)
        {
            Camera = camera;
            Width = width;
            Height = height;
            this.gl = gl;
        }

        private readonly GL gl;
        public Color Background { get; set; } = Color.White;

        public float Width { get; }
        public float Height { get; }

        public RectangleF Rect => new RectangleF(0, 0, Width, Height);
        public Camera Camera { get; }
        public void Draw(Framebuffer? framebuffer = null)
        {
            //gl.ClearColor(Background);
            //gl.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);

            //var width = framebuffer == null ? Width : framebuffer.Width;
            //var height = framebuffer == null ? Height : framebuffer.Height;
            //gl.Viewport(0, 0, width, height);

            //gl.Enable(EnableCap.Blend);
            //gl.BlendFunc(BlendingFactor.SrcAlpha, BlendingFactor.OneMinusSrcAlpha);


            //useprogram?
            //gl.ActiveTexture()

        }

        public void Navigate(NavigationMode type, float deltaX, float deltaY, Vector3 origin)
        {
            var camera = Camera.Position;
            var distance = Vector3.Distance(camera, origin);
            if (type == NavigationMode.LookAround)
            {
                origin = camera;
                deltaX = -deltaX;
                deltaY = -deltaY;
            }
            else if (type == NavigationMode.LookAt)
            {
                origin = camera;
            }

            var mvMatrix = Camera.GetViewMatrix();
            var mvOrigin = Vector3.Transform(origin, mvMatrix);
            var transform = Matrix4x4.CreateTranslation(mvOrigin);

            float degToRad(float deg) => deg * MathF.PI / 180;

            switch (type)
            {
                case NavigationMode.FreeOrbit:
                    transform = Matrix4x4.CreateRotationX(degToRad(deltaY / 4)) * transform;
                    transform = Matrix4x4.CreateRotationY(degToRad(deltaX / 4)) * transform;
                    break;
                case NavigationMode.LockedOrbit:
                case NavigationMode.FixedOrbit:
                case NavigationMode.LookAround:
                case NavigationMode.LookAt:
                case NavigationMode.Orbit:
                    transform = Matrix4x4.CreateRotationX(degToRad(deltaY / 4)) * transform;
                    var mvZ = Vector3.Transform(Vector3.UnitZ, mvMatrix);
                    mvZ = Vector3.Normalize(mvZ);
                    transform = MatrixExtension.Rotation(degToRad(deltaX / 4), mvZ) * transform;
                    break;
                case NavigationMode.Pan:
                    float c;
                    if (this.Camera.Attribute.Type == CameraType.ORTHOGONAL)
                    {
                        c = this.Camera.Attribute.Height / (float)Height;
                    }
                    else
                    {
                        var fov = this.Camera.Attribute.Fov * MathF.PI / 180;
                        var h = 2 * distance * MathF.Tan(fov / 2.0f);
                        c = h / Height;
                    }
                    transform = Matrix4x4.CreateTranslation(new Vector3(c * deltaX, -c * deltaY, 0)) * transform;
                    break;
                case NavigationMode.Zoom:
                    var direction = Vector3.Subtract(origin, camera);
                    var move = deltaY * MathF.Max(distance, this.meter * 2) / 20;
                    break;
                case NavigationMode.Walk:
                    break;
                default:
                    break;
            }

            var translation = Vector3.Negate(mvOrigin);
            transform = Matrix4x4.CreateTranslation(translation) * transform;
            mvMatrix = Matrix4x4.Multiply(transform, mvMatrix);
        }
        private float meter;
        public enum NavigationMode
        {
            None,
            Pan,
            Zoom,
            Orbit,
            FixedOrbit,
            FreeOrbit,
            LookAround,
            Walk,
            LookAt,
            LockedOrbit,
        }
    }

    public class MouseEvent : EventArgs
    {
        public MouseEvent(float clientX, float clientY, MouseButton button, InputAction action, KeyModifiers modes)
        {
            ClientX = clientX;
            ClientY = clientY;
            Button = button;
            Action = action;
            Modes = modes;
        }

        public float ClientX { get; }
        public float ClientY { get; }
        public MouseButton Button { get; }
        public InputAction Action { get; }
        public KeyModifiers Modes { get; }
    }

    public class MouseNavigation
    {
        public unsafe static void InitMouseEvents(Viewer viewer, WindowHandle* window)
        {
            //void handleMouseDown(MouseEvent @event)
            //{
            //    mouseDown = true;
            //    lastMouseX = @event.ClientX;
            //    lastMouseY = @event.ClientY;
            //    startX = @event.ClientX;
            //    startY = @event.ClientY;

            //    var r = viewer.Rect;
            //    var viewX = startX - r.Left;
            //    var viewY = viewer.Height - (startY - r.Top);

            //    //picking
            //    //var data = viewer.

            //};



            //Glfw glfw = Glfw.GetApi();
            //glfw.SetCursorPosCallback(window, MoveCallBack);
        }

        static bool mouseDown = false;
        static float lastMouseX = 0;
        static float lastMouseY = 0;
        static float startX = 0;
        static float startY = 0;
        static Vector3 origin = Vector3.Zero;

        static Viewer viewer;
        static void handleMouseMove(MouseEvent @event)
        {
            var newX = @event.ClientX;
            var newY = @event.ClientY;

            var deltaX = newX - lastMouseX;
            var deltaY = newY - lastMouseY;

            lastMouseX = newX;
            lastMouseY = newY;

            if (@event.Button == MouseButton.Left)
            {
                if (@event.Modes == KeyModifiers.Shift)
                {
                    viewer.Navigate(Viewer.NavigationMode.Pan, deltaX, deltaY, origin);
                }
            }
        }
        private static unsafe void MoveCallBack(WindowHandle* window, double x, double y)
        {
            Glfw glfw = Glfw.GetApi();
            if (glfw.GetMouseButton(window, (int)MouseButton.Left) == (int)InputAction.Press)
            {
                handleMouseMove(new MouseEvent((float)x, (float)y, MouseButton.Left, InputAction.Press, default));
            }
        }
    }

    public static class MatrixExtension
    {
        public static Matrix4x4 Rotation(float radian, Vector3 axis)
        {
            return Rotation(radian, Vector3.Zero, axis);
        }

        public static Vector3 Unit(this in Vector3 dir)
        {
            return dir / dir.Length();
        }

        public static Matrix4x4 Rotation(float radian, Vector3 p, Vector3 axis)
        {
            axis = axis.Unit();
            var u = axis.X;
            var v = axis.Y;
            var w = axis.Z;

            var a = p.X;
            var b = p.Y;
            var c = p.Z;

            var Cos = MathF.Cos(radian);
            var Sin = MathF.Sin(radian);

            var M = Matrix4x4.Identity;

            M.M11 = u * u + (v * v + w * w) * Cos;
            M.M12 = u * v * (1 - Cos) - w * Sin;
            M.M13 = u * w * (1 - Cos) + v * Sin;
            M.M14 = (a * (v * v + w * w) - u * (b * v + c * w)) * (1 - Cos) + (b * w - c * v) * Sin;
            M.M21 = u * v * (1 - Cos) + w * Sin;
            M.M22 = v * v + (u * u + w * w) * Cos;
            M.M23 = v * w * (1 - Cos) - u * Sin;
            M.M24 = (b * (u * u + w * w) - v * (a * u + c * w)) * (1 - Cos) + (c * u - a * w) * Sin;
            M.M31 = u * w * (1 - Cos) - v * Sin;
            M.M32 = v * w * (1 - Cos) + u * Sin;
            M.M33 = w * w + (u * u + v * v) * Cos;
            M.M34 = (c * (u * u + v * v) - w * (a * u + b * v)) * (1 - Cos) + (a * v - b * u) * Sin;
            return M;
        }
    }
}