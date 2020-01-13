using System;
using System.Drawing;

namespace JunoEngine.Systems
{
    public struct Vector3
    {
        internal static readonly Vector3 zero = new Vector3();
        internal static readonly Vector3 one = new Vector3(1, 1, 1);
        public float x, y, z;

        internal float Abs
        {
            get
            {
                var X = x;
                var Y = y;
                var Z = z;

                if (X < 0) X = -x;
                if (Y < 0) Y = -y;
                if (Z < 0) Z = -z;

                return X + Y + Z;
            }
        }

        public Vector3(float x = 0, float y = 0, float z = 0)
        {
            this.x = x;
            this.y = y;
            this.z = z;
        }

        public override bool Equals(object obj)
        {
            return base.Equals(obj);
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        public override string ToString()
        {
            return $"({x}, {y}, {z})";
        }

        public static bool operator ==(Vector3 a, Vector3 b)
        {
            return a.x == b.x && a.y == b.y && a.z == b.z;
        }

        public static Vector3 Random(float xMin, float xMax, float yMin, float yMax)
        {
            var ran = new Random();

            var x = ran.Next((int)xMin, (int)xMax);
            var y = ran.Next((int)yMin, (int)yMax);
            var z = ran.Next((int)yMin, (int)yMax);

            return new Vector3(x, y, z);
        }

        public static Vector3 operator -(Vector3 v)
        {
            return new Vector3(-v.x, -v.y, -v.z);
        }

        public static implicit operator Vector3(Vector2 v)
        {
            return new Vector3(v.x, v.y);
        }

        public static implicit operator Vector2(Vector3 v)
        {
            return new Vector2(v.x, v.y);
        }

        public static bool operator !=(Vector3 a, Vector3 b)
        {
            return a.x != b.x || a.y != b.y || a.z != b.z;
        }

        public static Vector3 operator *(Vector3 a, float b)
        {
            return new Vector3(a.x *= b, a.y *= b, a.z *= b);
        }

        public static float Distance(Vector3 pos_a, Vector3 pos_b)
        {
            var a = pos_a.Abs;
            var b = pos_b.Abs;

            if (a > b)
            {
                return a - b;
            }
            else
            {
                return b - a;
            }
        }

        public static Vector3 operator -(Vector3 a, Vector3 b)
        {
            return new Vector3(a.x -= b.x, a.y -= b.y, a.z -= b.z);
        }

        public static Vector3 operator +(Vector3 a, Vector3 b)
        {
            return new Vector3(a.x += b.x, a.y += b.y, a.z += b.z);
        }
    }
}