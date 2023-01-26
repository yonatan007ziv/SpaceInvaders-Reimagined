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

        private Vector2 _positionBeforeCenter;
        private Vector2 centeredPosition;
        public Vector2 position
        {
            get { return _positionBeforeCenter; }
            private set 
            {
                _positionBeforeCenter = value;
                centeredPosition.X = _positionBeforeCenter.X - scale.X / 2; 
                centeredPosition.Y = _positionBeforeCenter.Y - scale.Y / 2; 
            }
        }

        private Vector2 _scale;
        public Vector2 scale
        {
            get { return _scale; }
            private set { _scale = value; }
        }

        public Transform(Vector2 scale, Vector2 position)
        {
            transforms.Add(this);

            this.scale = scale;
            this.position = position;

            MainWindow.instance!.SizeChanged += (sender, e) => OnSizeChanged();

            UpdateScale();
            UpdatePosition();
        }
        public void AddPosX(int x)
        {
            position += new Vector2(x, 0);
            UpdatePosition();
        }
        public void AddPosY(int y)
        {
            position += new Vector2(0, y);
            UpdatePosition();
        }
        public void SetPosition(Vector2 newPos)
        {
            if (position != newPos)
            {
                position = newPos;
                UpdatePosition();
            }
        }
        public void SetScale(Vector2 newSize)
        {
            if (scale != newSize)
            {
                scale = newSize;
                UpdateScale();
            }
        }
        public void UpdatePosition()
        {
            if (SetPositionDel != null)
                SetPositionDel(centeredPosition);
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
            //Debug.WriteLine(MainWindow.instance.ActualWidth);
            //Debug.WriteLine(MainWindow.instance.ActualHeight);
            //_scale *= new Vector2((float)(MainWindow.instance.ActualWidth /MainWindow.instance.), (float)MainWindow.instance.ActualHeight / 250);
            //UpdateScale();
        }
    }
}