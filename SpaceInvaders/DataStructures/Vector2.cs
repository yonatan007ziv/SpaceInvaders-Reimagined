using System.Drawing;
using System.Reflection.Metadata.Ecma335;

namespace SpaceInvaders.DataStructures
{
    internal class Vector2
    {
        private int _x, _y;
        public int x
        {
            get { return _x; }
            private set { _x = value; }
        }
        public int y
        {
            get { return _y; }
            private set { _y = value; }
        }

        public Vector2(Size size)
        {
            _x = size.Width;
            _y = size.Height;
        }
        public Vector2(Point point)
        {
            _x = point.X;
            _y = point.Y;
        }
        public Vector2(Vector2 other)
        {
            _x = other._x;
            _y = other._y;
        }
        public Vector2(int x, int y)
        {
            this._x = x;
            this._y = y;
        }
        public int GetMagnitude()
        {
            return (int)Math.Sqrt(_x * _x + _y * _y);
        }
        public Vector2 Normalize()
        {
            return this / GetMagnitude();
        }

        public Point ToPoint()
        {
            return new Point(_x, _y);
        }
        public Size ToSize()
        {
            return new Size(_x, _y);
        }

        #region operator overloading
        public  static bool operator ==(Vector2 a, Vector2 b)
        {
            return a.x == b.x && a.y == b.y;
        }
        public static bool operator !=(Vector2 a, Vector2 b)
        {
            return a.x != b.x || a.y != b.y;
        }
        public static Vector2 operator +(Vector2 a, Vector2 b)
        {
            return new Vector2(a._x + b._x, a._y + b._y);
        }
        public static Vector2 operator -(Vector2 a, Vector2 b)
        {
            return new Vector2(a._x - b._x, a._y - b._y);
        }
        public static Vector2 operator *(Vector2 a, int mul)
        {
            return new Vector2(a._x * mul, a._y * mul);
        }
        public static Vector2 operator /(Vector2 a, int div)
        {
            return new Vector2(a._x / div, a._y / div);
        }
        #endregion
        public override string ToString()
        {
            return $"({x}, {y})";
        }

        public static Vector2 Zero = new Vector2(0, 0);
    }
}