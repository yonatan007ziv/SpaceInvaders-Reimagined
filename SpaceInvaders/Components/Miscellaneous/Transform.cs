using SpaceInvaders.DataStructures;
using System.Diagnostics;

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

            DesiredPos = position;
            _position = position;
            _scale = scale;
            FixCenterPivot();
        }
        public void AddPosX(int x)
        {
            _position += new Vector2(x, 0);
            DesiredPos += new Vector2(x, 0);
        }
        public void AddPosY(int y)
        {
            _position += new Vector2(0, y);
            DesiredPos += new Vector2(0, y);
        }
        public void SetPosition(Vector2 newPos)
        {
            if (_position != newPos)
            {
                DesiredPos = newPos + _scale / 2;
                _position = newPos;
            }
        }
        public void SetScale(Vector2 newSize)
        {
            if (_scale != newSize)
            {
                _scale = newSize;
                FixCenterPivot();
                if (SetScaleDel != null)
                    SetScaleDel(_scale);
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
            del(_position);
        }
        public void AddScaleDel(ScalePositionDel del)
        {
            SetScaleDel += del;
            del(_scale);
        }

        public Vector2 DesiredPos;
        private void FixCenterPivot()
        {
            SetPosition(DesiredPos - this._scale / 2);
        }
    }
}