using SpaceInvadersGameWindow;
using System;
using System.Numerics;

namespace SpaceInvaders.Components.Miscellaneous
{
    internal class Transform
    {
        public Action PositionChanged;
        public Action ScaleChanged;

        private Vector2 BasePosition;
        public Vector2 CenteredPosition;
        private Vector2 _position;
        public Vector2 position
        {
            get { return _position; }
            private set
            {
                Vector2 tempPosition = _position;
                _position = value;
                BasePosition = value / MainWindow.ratio;
                CenteredPosition = value - (scale / 2);
                if (tempPosition != value)
                    UpdatePosition();
            }
        }

        private Vector2 BaseScale;
        private Vector2 _scale;
        public Vector2 scale
        {
            get { return _scale; }
            set
            {
                Vector2 tempScale = _scale;
                _scale = value;
                BaseScale = value / MainWindow.ratio;
                if (tempScale != value)
                    UpdateScale();
            }
        }

        public Transform(Vector2 scale, Vector2 position)
        {
            scale *= MainWindow.ratio;
            MainWindow.instance!.SizeChanged += (s, e) => OnSizeChanged();

            this.scale = scale;
            this.position = position;

            UpdateScale();
            UpdatePosition();
        }
        public void AddPosX(float x)
        {
            position += new Vector2(x, 0);
            UpdatePosition();
        }
        public void AddPosY(float y)
        {
            position += new Vector2(0, y);
            UpdatePosition();
        }
        public void UpdatePosition()
        {
            PositionChanged?.Invoke();
        }
        public void UpdateScale()
        {
            ScaleChanged?.Invoke();
        }

        private void OnSizeChanged()
        {
            position = BasePosition * MainWindow.ratio;
            scale = BaseScale * MainWindow.ratio;
        }
        public void Dispose()
        {
            MainWindow.instance!.SizeChanged -= (s, e) => OnSizeChanged();
        }
    }
}