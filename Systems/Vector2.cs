using System;
using System.Drawing;

namespace JunoEngine.Systems
{
    public struct Vector2
    {
        internal static readonly Vector2 zero = new Vector2(0, 0);
        internal static readonly Vector2 one = new Vector2(1, 1);
        public float x, y;

        internal float Abs
        {
            get
            {
                var X = x;
                var Y = y;

                if (X < 0) X = -x;
                if (Y < 0) Y = -y;

                return X + Y;
            }
        }

        public Vector2(float x = 0, float y = 0)
        {
            this.x = x;
            this.y = y;
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

        public static bool operator ==(Vector2 a, Vector2 b)
        {
            return a.x == b.x && a.y == b.y;
        }

        public static Vector2 Random(float xMin, float xMax, float yMin, float yMax)
        {
            var ran = new Random();

            var x = ran.Next((int)xMin, (int)xMax);
            var y = ran.Next((int)yMin, (int)yMax);

            return new Vector2(x, y);
        }

        public static Vector2 operator -(Vector2 v)
        {
            return new Vector2(-v.x, -v.y);
        }

        public static explicit operator Vector2(Point v)
        {
            return new Vector2(v.X, v.Y);
        }

        public static explicit operator Vector2(PointF v)
        {
            return new Vector2(v.X, v.Y);
        }

        public static implicit operator Point(Vector2 v)
        {
            return new Point((int)v.x, (int)v.y);
        }

        public static implicit operator PointF(Vector2 v)
        {
            return new PointF(v.x, v.y);
        }

        public static implicit operator Size(Vector2 v)
        {
            return new Size((int)v.x, (int)v.y);
        }

        public static explicit operator Vector2(Size v)
        {
            return new Vector2(v.Width, v.Height);
        }

        public static implicit operator SizeF(Vector2 v)
        {
            return new SizeF(v.x, v.y);
        }

        public static bool operator !=(Vector2 a, Vector2 b)
        {
            return a.x != b.x || a.y != b.y;
        }

        public static Vector2 operator *(Vector2 a, float b)
        {
            return new Vector2(a.x *= b, a.y *= b);
        }

        public static float Distance(Vector2 pos_a, Vector2 pos_b)
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

        public static Vector2 operator -(Vector2 a, Vector2 b)
        {
            return new Vector2(a.x -= b.x, a.y -= b.y);
        }

        public static Vector2 operator +(Vector2 a, Vector2 b)
        {
            return new Vector2(a.x += b.x, a.y += b.y);
        }
    }
}