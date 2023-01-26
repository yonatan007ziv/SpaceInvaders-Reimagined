using SpaceInvadersGameWindow;
using System.Collections.Generic;
using System.Diagnostics;
using System.Numerics;
using System.Windows;

namespace SpaceInvaders.Components.Miscellaneous
{
    internal class Transform
    {
        public static List<Transform> transforms = new List<Transform>();

        public delegate void ScalePositionDel(Vector2 vec);
        private ScalePositionDel? SetPositionDel;
        private ScalePositionDel? SetScaleDel;

        private Vector2 _position;
        public Vector2 position
        {
            get { return _position; }
            private set { }
        }

        private Vector2 _scale;
        public Vector2 scale
        {
            get { return _scale; }
            private set { }
        }

        public Transform(Vector2 position, Vector2 scale)
        {
            transforms.Add(this);
            _position = position;
            _scale = scale;

            MainWindow.instance.SizeChanged += (sender, e) => OnSizeChanged();

            UpdatePosition();
            UpdateScale();
        }
        public void AddPosX(int x)
        {
            _position += new Vector2(x, 0);
            UpdatePosition();
        }
        public void AddPosY(int y)
        {
            _position += new Vector2(0, y);
            UpdatePosition();
        }
        public void SetPosition(Vector2 newPos)
        {
            if (_position != newPos)
                _position = newPos;
            UpdatePosition();
        }
        public void SetScale(Vector2 newSize)
        {
            if (_scale != newSize)
            {
                _scale = newSize;
                UpdateScale();
            }
        }
        public void UpdatePosition()
        {
            if (SetPositionDel != null)
                SetPositionDel(_position);
        }
        public void UpdateScale()
        {
            if (SetScaleDel != null)
                SetScaleDel(_scale);
        }

        public void AddPositionDel(ScalePositionDel del)
        {
            SetPositionDel += del;
            UpdatePosition();
        }
        public void AddScaleDel(ScalePositionDel del)
        {
            SetScaleDel += del;
            UpdateScale();
        }
        private void OnSizeChanged()
        {
            Debug.WriteLine(MainWindow.instance.ActualWidth);
            Debug.WriteLine(MainWindow.instance.ActualHeight);
            _scale *= new Vector2((float)(MainWindow.instance.ActualWidth /MainWindow.instance.), (float)MainWindow.instance.ActualHeight / 250);
            UpdateScale();
        }
    }
}