using System.Numerics;

namespace OpenGL.Extension
{
    public static class MatrixExtension
    {
        public static Matrix4x4 Rotation(float radian, Vector3 axis)
        {
            return new Matrix4x4(
                MathF.Cos(radian) + axis.X * axis.X * (1 - MathF.Cos(radian)),              axis.X * axis.Y * (1 - MathF.Cos(radian) - axis.Z * MathF.Sin(radian)),     axis.X * axis.Z * (1 - MathF.Cos(radian)) + axis.Y * MathF.Sin(radian),     0,
                axis.Y * axis.X * (1 - MathF.Cos(radian)) + axis.Z * MathF.Sin(radian),     MathF.Cos(radian) + axis.Y * axis.Y * (1 - MathF.Cos(radian)),              axis.Y * axis.Z * (1 - MathF.Cos(radian)) - axis.X * MathF.Sin(radian),     0,
                axis.Z * axis.X * (1 - MathF.Cos(radian)) - axis.Y * MathF.Sin(radian),     axis.Z * axis.Y * (1 - MathF.Cos(radian)) + axis.X * MathF.Sin(radian),     MathF.Cos(radian) + axis.Z * axis.Z * (1 - MathF.Cos(radian)),              0,
                0,                                                                          0,                                                                          0,                                                                          1);
        }
    }
}