using System;
using System.Collections.Generic;
using System.Numerics;
using System.Windows;

namespace GameWindow.Components.Miscellaneous
{
    public class Transform
    {
        private Action? positionChanged;
        private Action? scaleChanged;

        public static List<Transform> transforms = new List<Transform>();

        public Action? PositionChanged
        {
            get { return positionChanged; }
            set
            {
                positionChanged = value;
                positionChanged!();
            }
        }
        public Action? ScaleChanged
        {
            get { return scaleChanged; }
            set
            {
                scaleChanged = value;
                scaleChanged!();
            }
        }

        #region position logic
        private Vector2 _basePosition;
        public Vector2 Position
        {
            get { return _basePosition; }
            set
            {
                Vector2 TempPosition = _basePosition;
                _basePosition = value;
                if (TempPosition != value)
                    Application.Current.Dispatcher.Invoke(() => PositionChanged?.Invoke());
            }
        }
        public Vector2 ActualPosition
        {
            get { return _basePosition * MainWindow.ratio; }
        }
        public Vector2 CenteredPosition
        {
            get { return ActualPosition - (ActualScale / 2); }
        }
        #endregion

        #region scale logic
        private Vector2 _baseScale;
        public Vector2 Scale
        {
            get { return _baseScale; }
            set
            {
                Vector2 TempScale = _baseScale;
                _baseScale = value;
                if (TempScale != value)
                    ScaleChanged?.Invoke();
            }
        }
        public Vector2 ActualScale
        {
            get { return _baseScale * MainWindow.ratio; }
        }
        #endregion

        public Transform(Vector2 scale, Vector2 position)
        {
            transforms.Add(this);

            this._baseScale = scale;
            this._basePosition = position;
        }
        public void OnSizeChanged()
        {
            PositionChanged?.Invoke();
            ScaleChanged?.Invoke();
        }
        public void Dispose()
        {
            transforms.Remove(this);
        }
    }
}