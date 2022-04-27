using System.Numerics;

namespace OpenGL.Extension
{
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