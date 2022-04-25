
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
        public Camera()
        {
            Position = new Vector3(0, 0, 0);
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

        float deltaTime = 0f;
        float lastFrame = 0f;
        public unsafe void ProcessInput(Glfw GLFW, WindowHandle* window)
        {
            float currentFrame = (float)GLFW.GetTime();
            deltaTime = currentFrame - lastFrame;
            lastFrame = currentFrame;

            var cameraSpeed = 2.5f * deltaTime;
            if (GLFW.GetKey(window, Keys.W) == (int)InputAction.Press)
            {
                Position += cameraSpeed * Front;
            }
            if (GLFW.GetKey(window, Keys.S) == (int)InputAction.Press)
            {
                Position -= cameraSpeed * Front;
            }
            if (GLFW.GetKey(window, Keys.A) == (int)InputAction.Press)
            {
                Position -= Vector3.Normalize(Vector3.Cross(Front, WorldUp)) * cameraSpeed;
            }
            if (GLFW.GetKey(window, Keys.D) == (int)InputAction.Press)
            {
                Position += Vector3.Normalize(Vector3.Cross(Front, WorldUp)) * cameraSpeed;
            }
            if (GLFW.GetKey(window, Keys.X) == (int)InputAction.Press)
            {
                Position -= Vector3.Normalize(WorldUp) * cameraSpeed;
            }
            if (GLFW.GetKey(window, Keys.C) == (int)InputAction.Press)
            {
                Position += Vector3.Normalize(WorldUp) * cameraSpeed;
            }

            var cameraRotate = 0.5f * deltaTime;
            if (GLFW.GetKey(window, Keys.Q) == (int)InputAction.Press)
            {
                Yaw -= cameraRotate;
                var x = MathF.Cos(Yaw);
                var z = MathF.Sin(Yaw);
                Front = new Vector3(x, 0, z);
            }
            if (GLFW.GetKey(window, Keys.E) == (int)InputAction.Press)
            {
                Yaw += cameraRotate;
                var x = MathF.Cos(Yaw);
                var z = MathF.Sin(Yaw);
                Front = new Vector3(x, 0, z);
            }

            Press?.Invoke(new WindowSender(GLFW, window));
        }

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

        public void ProcessMouseScroll(float yoffset)
        {
            if (Zoom < 0.1f)
            {
                Zoom = 0.1f;
            }
            else if (Zoom > MathF.PI / 4)
            {
                Zoom = MathF.PI / 4;
            }
            else
            {
                Zoom -= yoffset;
            }
        }
    }
}