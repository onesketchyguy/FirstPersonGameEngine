using System;
using System.Drawing;

namespace JunoEngine.Systems
{
    public struct Vector3
    {
        internal static readonly Vector3 zero = new Vector3(0, 0);
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
            return $"({x}, {y})";
        }

        public static bool operator ==(Vector3 a, Vector3 b)
        {
            return a.x == b.x && a.y == b.y;
        }

        public static Vector3 Random(float xMin, float xMax, float yMin, float yMax)
        {
            var ran = new Random();

            var x = ran.Next((int)xMin, (int)xMax);
            var y = ran.Next((int)yMin, (int)yMax);

            return new Vector3(x, y);
        }

        public static Vector3 operator -(Vector3 v)
        {
            return new Vector3(-v.x, -v.y);
        }

        public static explicit operator Vector3(Point v)
        {
            return new Vector3(v.X, v.Y);
        }

        public static explicit operator Vector3(Vector2 v)
        {
            return new Vector3(v.x, v.y);
        }

        public static explicit operator Vector2(Vector3 v)
        {
            return new Vector2(v.x, v.y);
        }

        public static explicit operator Vector3(PointF v)
        {
            return new Vector3(v.X, v.Y);
        }

        public static implicit operator Point(Vector3 v)
        {
            return new Point((int)v.x, (int)v.y);
        }

        public static implicit operator PointF(Vector3 v)
        {
            return new PointF(v.x, v.y);
        }

        public static implicit operator Size(Vector3 v)
        {
            return new Size((int)v.x, (int)v.y);
        }

        public static explicit operator Vector3(Size v)
        {
            return new Vector3(v.Width, v.Height);
        }

        public static implicit operator SizeF(Vector3 v)
        {
            return new SizeF(v.x, v.y);
        }

        public static bool operator !=(Vector3 a, Vector3 b)
        {
            return a.x != b.x || a.y != b.y;
        }

        public static Vector3 operator *(Vector3 a, float b)
        {
            return new Vector3(a.x *= b, a.y *= b);
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
            return new Vector3(a.x -= b.x, a.y -= b.y);
        }

        public static Vector3 operator +(Vector3 a, Vector3 b)
        {
            return new Vector3(a.x += b.x, a.y += b.y);
        }
    }
}