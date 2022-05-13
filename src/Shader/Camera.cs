
using Silk.NET.GLFW;
using System.Numerics;

namespace OpenGL.Extension
{
    public unsafe struct WindowSender
    {
        public Glfw GLFW;
        public WindowHandle* Window;

        public WindowSender(Glfw gLFW, WindowHandle* window)
        {
            GLFW = gLFW;
            Window = window;
        }
    }
    public unsafe class Camera2
    {
        public Camera2()
        {
            var p = new Vector3(0, 0, 8);
            var f = new Vector3(0, 0, -1);
            var up = new Vector3(0, 1, 0);
            _viewMatrix = Matrix4x4.CreateLookAt(p, p + f, up);
            Updata();
        }

        private Matrix4x4 _viewMatrix;
        private Vector3 _position;
        private Vector3 _up;
        private Vector3 _front;
        public Vector3 Position { get => _position; set => _position = value; }
        public Vector3 Front { get => _front; set => _front = value; }
        public Vector3 Up { get => _up; set => _up = value; }

        public float Hight { get; set; } = 600;
        public float Width { get; set; } = 800;

        public float Zoom { get; set; } = MathF.PI / 4;

        public Matrix4x4 ViewMatrix
        {
            get => _viewMatrix;
            set
            {
                _viewMatrix = value;
                Updata();
            }
        }

        void Updata()
        {
            Matrix4x4.Invert(_viewMatrix, out var inv);
            Matrix4x4.Decompose(inv, out _, out var rotation, out _position);
            _front = Vector3.Transform(new Vector3(0, 0, -1), rotation);
            _up = Vector3.Transform(Vector3.UnitY, rotation);
        }

        public void UpdataMatrix()
        {
            _viewMatrix = Matrix4x4.CreateLookAt(_position, _front, _up);
            Updata();
        }

        float lastMouseX;
        float lastMouseY;
        bool mouse_right_press;
        bool mouse_left_press;
        KeyModifiers _mods;
        public void MouseInput(WindowHandle* window, MouseButton button, InputAction action, KeyModifiers mods)
        {
            if (button == MouseButton.Right)
            {
                _mods = mods;
                Glfw GLFW = Glfw.GetApi();
                GLFW.GetCursorPos(window, out var xpos, out var ypos);
                if (action == InputAction.Press)
                {
                    mouse_right_press = true;
                    lastMouseX = (float)xpos;
                    lastMouseY = (float)ypos;
                }
                else
                {
                    mouse_right_press = false;
                }
            }
        }
        public void MouseMove(WindowHandle* window, double newX, double newY)
        {
            if (mouse_right_press)
            {
                var deltaX = (float)newX - lastMouseX;
                var deltaY = (float)newY - lastMouseY;

                lastMouseX = (float)newX;
                lastMouseY = (float)newY;

                if (_mods == KeyModifiers.Shift)
                {
                    Pan(deltaX, deltaY);
                }
                else
                {
                    Orbit(deltaX, deltaY);
                }
            }
        }

        float deltaTime = 0f;
        float lastFrame = 0f;
        public unsafe bool ProcessInput(Glfw GLFW, WindowHandle* window)
        {
            float currentFrame = (float)GLFW.GetTime();
            deltaTime = currentFrame - lastFrame;
            lastFrame = currentFrame;
            var cameraSpeed = 2.5f * deltaTime;
            bool flag = false;
            if (GLFW.GetKey(window, Keys.W) == (int)InputAction.Press)
            {
                Position += cameraSpeed * Front;
                flag = true;
            }
            if (GLFW.GetKey(window, Keys.S) == (int)InputAction.Press)
            {
                Position -= cameraSpeed * Front;
                flag = true;
            }
            if (GLFW.GetKey(window, Keys.A) == (int)InputAction.Press)
            {
                Position -= Vector3.Normalize(Vector3.Cross(Front, Up)) * cameraSpeed;
                flag = true;
            }
            if (GLFW.GetKey(window, Keys.D) == (int)InputAction.Press)
            {
                Position += Vector3.Normalize(Vector3.Cross(Front, Up)) * cameraSpeed;
                flag = true;
            }
            if (GLFW.GetKey(window, Keys.X) == (int)InputAction.Press)
            {
                Position -= Vector3.Normalize(Up) * cameraSpeed;
                flag = true;
            }
            if (GLFW.GetKey(window, Keys.C) == (int)InputAction.Press)
            {
                Position += Vector3.Normalize(Up) * cameraSpeed;
                flag = true;
            }
            if (flag) UpdataMatrix();
            return flag;
        }

        public void Pan(float deltaX, float deltaY, Vector3 origin = default)
        {
            var distance = Vector3.Distance(Position, origin);

            var fov = Zoom;
            var h = 2 * distance * MathF.Tan(fov / 2.0f);
            var c = h / Hight;

            var transform = Matrix4x4.CreateTranslation(-Vector3.Normalize(Vector3.Cross(Up, Front)) * c * deltaX);
            transform = Matrix4x4.CreateTranslation(-Vector3.Normalize(Up) * c * deltaY) * transform;


            ViewMatrix = transform * _viewMatrix;
        }

        public void Orbit(float deltaX, float deltaY, Vector3 origin = default)
        {
            var transform = Matrix4x4.CreateFromAxisAngle(-Vector3.Cross(Up, Front), degToRad(deltaY / 4));
            transform = Matrix4x4.CreateRotationY(degToRad(deltaX / 4)) * transform;
            ViewMatrix = transform * _viewMatrix;
        }

        public void ZoomC(float deltaY, Vector3 origin = default)
        {
            var direction = origin - Position;
            var distance = Vector3.Distance(Position, origin);
            var move = deltaY * MathF.Max(meter * 2, distance) / 20;

            var transform = Matrix4x4.CreateTranslation(-Vector3.Normalize(direction) * move);

            ViewMatrix = transform * _viewMatrix;
        }

        float meter = 1;

        public void ProcessMouseScroll(WindowHandle* window, double offsetX, double offsetY)
        {
            ZoomC((float)offsetY);
        }

        static float degToRad(float deg) => deg * MathF.PI / 180.0f;
    }
    public unsafe class Camera
    {
        const float YAW = -90.0f;
        const float PITCH = 0.0f;
        const float SPEED = 2.5f;
        const float SENSITIVITY = 0.1f;
        const float ZOOM = MathF.PI / 4;

        public event Action<WindowSender> Press;

        public Vector3 Position { get; set; }
        public Vector3 Front { get; set; }
        public Vector3 WorldUp { get; set; }

        public Vector3 Right => Vector3.Normalize(Vector3.Cross(Front, WorldUp));
        public Vector3 Up => Vector3.Normalize(Vector3.Cross(Right, Front));

        public Matrix4x4 ViewMatrix => Matrix4x4.CreateLookAt(Position, Position + Front, Up);

        public float Yaw { get; set; }
        public float Pitch { get; set; }
        public float MovementSpeed { get; set; }
        public float Zoom { get; set; }
        public float MouseSensitivity { get; set; }

        public float Hight { get; set; } = 600;
        public float Width { get; set; } = 800;
        public Camera()
        {
            Position = new Vector3(0, 0, 1);
            Front = new Vector3(0, 0, -1);
            WorldUp = new Vector3(0, 1, 0);

            Yaw = YAW;
            Pitch = PITCH;

            MovementSpeed = SPEED;
            MouseSensitivity = SENSITIVITY;
            Zoom = ZOOM;
        }

        public Camera(float posX, float posY, float posZ, float upX, float upY, float upZ, float yaw, float pitch)
        {
            Position = new Vector3(posX, posY, posZ);
            Front = new Vector3(0, 0, -1);
            WorldUp = new Vector3(upX, upY, upZ);

            Yaw = yaw;
            Pitch = pitch;

            MovementSpeed = SPEED;
            MouseSensitivity = SENSITIVITY;
            Zoom = ZOOM;
        }



        bool mouse_right_press;
        bool mouse_left_press;
        KeyModifiers _mods;

        static float degToRad(float deg) => deg * MathF.PI / 180.0f;

        public void MouseInput(WindowHandle* window, MouseButton button, InputAction action, KeyModifiers mods)
        {
            if (button == MouseButton.Right)
            {
                _mods = mods;
                Glfw GLFW = Glfw.GetApi();
                GLFW.GetCursorPos(window, out var xpos, out var ypos);
                if (action == InputAction.Press)
                {
                    mouse_right_press = true;
                    lastMouseX = (float)xpos;
                    lastMouseY = (float)ypos;
                }
                else
                {
                    mouse_right_press = false;
                }
            }
        }

        public void MouseMove(WindowHandle* window, double newX, double newY)
        {
            Glfw GLFW = Glfw.GetApi();
            if (mouse_right_press)
            {
                var deltaX = (float)newX - lastMouseX;
                var deltaY = (float)newY - lastMouseY;

                lastMouseX = (float)newX;
                lastMouseY = (float)newY;

                Yaw = deltaX;
                Pitch = deltaY;

                if (_mods == KeyModifiers.Shift)
                {
                    Pan(deltaX, deltaY);
                }
                else
                {
                    Orbit(deltaX, deltaY);
                }
            }
        }

        float deltaTime = 0f;
        float lastFrame = 0f;
        public unsafe bool ProcessInput(Glfw GLFW, WindowHandle* window)
        {
            float currentFrame = (float)GLFW.GetTime();
            deltaTime = currentFrame - lastFrame;
            lastFrame = currentFrame;
            Press?.Invoke(new WindowSender(GLFW, window));
            var cameraSpeed = 2.5f * deltaTime;
            bool flag = false;
            if (GLFW.GetKey(window, Keys.W) == (int)InputAction.Press)
            {
                Position += cameraSpeed * Front;
                flag = true;
            }
            if (GLFW.GetKey(window, Keys.S) == (int)InputAction.Press)
            {
                Position -= cameraSpeed * Front;
                flag = true;
            }
            if (GLFW.GetKey(window, Keys.A) == (int)InputAction.Press)
            {
                Position -= Vector3.Normalize(Vector3.Cross(Front, WorldUp)) * cameraSpeed;
                flag = true;
            }
            if (GLFW.GetKey(window, Keys.D) == (int)InputAction.Press)
            {
                Position += Vector3.Normalize(Vector3.Cross(Front, WorldUp)) * cameraSpeed;
                flag = true;
            }
            if (GLFW.GetKey(window, Keys.X) == (int)InputAction.Press)
            {
                Position -= Vector3.Normalize(WorldUp) * cameraSpeed;
                flag = true;
            }
            if (GLFW.GetKey(window, Keys.C) == (int)InputAction.Press)
            {
                Position += Vector3.Normalize(WorldUp) * cameraSpeed;
                flag = true;
            }

            var cameraRotate = 0.5f * deltaTime;
            if (GLFW.GetKey(window, Keys.Q) == (int)InputAction.Press)
            {
                Yaw -= cameraRotate;
                var x = MathF.Cos(Yaw);
                var z = MathF.Sin(Yaw);
                Front = new Vector3(x, 0, z);
                flag = true;
            }
            if (GLFW.GetKey(window, Keys.E) == (int)InputAction.Press)
            {
                Yaw += cameraRotate;
                var x = MathF.Cos(Yaw);
                var z = MathF.Sin(Yaw);
                Front = new Vector3(x, 0, z);
                flag = true;
            }
            return flag;
        }

        float lastMouseX;
        float lastMouseY;

        public void Pan(float deltaX, float deltaY, Vector3 origin = default)
        {
            var distance = Vector3.Distance(Position, origin);
            var fov = this.Zoom;
            var h = 2 * distance * MathF.Tan(fov / 2.0f);
            var c = h / Hight;
            Position -= new Vector3(c * deltaX, -c * deltaY, 0);
        }

        public void Orbit(float deltaX, float deltaY, Vector3 origin = default)
        {
            var roy = Vector3.Dot(Front, Vector3.UnitZ) > 0 ? deltaY : -deltaY;

            //Front

            var ro = Matrix4x4.CreateRotationX(degToRad(roy / 4)) * Matrix4x4.CreateRotationY(degToRad(-deltaX / 4));
            var mvZ = Vector3.Transform(Position, ro);

            Position = mvZ;
            //Yaw = -deltaX / 4;
            //Pitch = deltaY / 4;

            var f = origin - Position;
            Front = Vector3.Normalize(f);
        }
        public void ZoomC(float deltaY, Vector3 origin = default)
        {
            //var direction = origin - Position;
            //var distance = Vector3.Distance(Position, origin);
            //var move = deltaY * MathF.Max(meter * 2, distance) / 20;
            //Position = Position + direction * move;

            //Console.WriteLine("zoom");
            Position += deltaY * Front;
        }

        float meter = 1;

        public void ProcessKeyboard(float xoffset, float yoffset, bool constrainPitch = true)
        {
            xoffset *= MouseSensitivity;
            yoffset *= MouseSensitivity;

            Yaw += xoffset;
            Pitch += yoffset;

            if (constrainPitch)
            {
                if (Pitch > 89.0f) Pitch = 89.0f;
                else if (Pitch < -89.0f) Pitch = -89.0f;
            }
        }

        public void ProcessMouseScroll(WindowHandle* window, double offsetX, double offsetY)
        {
            ZoomC((float)offsetY);
        }
    }
}