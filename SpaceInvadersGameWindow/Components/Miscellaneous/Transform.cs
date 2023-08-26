using System;
using System.Collections.Generic;
using System.Numerics;

namespace GameWindow.Components.Miscellaneous
{
    /// <summary>
    /// A class implementing a transform
    /// </summary>
    public class Transform
    {
        public static readonly List<Transform> Transforms = new List<Transform>();

        private Action? _positionChanged;
        private Action? _scaleChanged;

        /// <summary>
        /// PositionChanged adder & alerter
        /// </summary>
        public event Action PositionChanged
        {
            add
            {
                _positionChanged += value;
                _positionChanged?.Invoke();
            }
            remove
            {
                _positionChanged -= value;
                _positionChanged?.Invoke();
            }
        }

        /// <summary>
        /// ScaleChanged adder & alerter
        /// </summary>
        public event Action ScaleChanged
        {
            add
            {
                _scaleChanged += value;
                _scaleChanged?.Invoke();
            }
            remove
            {
                _scaleChanged -= value;
                _scaleChanged?.Invoke();
            }
        }

        #region position logic
        private Vector2 _basePosition;

        /// <summary>
        /// Base position of the transform
        /// <Getter> returns <see cref="_basePosition"/> </Getter>
        /// <Setter> If position changed, calls <see cref="OnSizeChanged"/> </Setter>
        /// </summary>
        public Vector2 Position
        {
            get { return _basePosition; }
            set
            {
                Vector2 TempPosition = _basePosition;
                _basePosition = value;
                if (TempPosition != value)
                    OnSizeChanged();
            }
        }

        /// <summary>
        /// Actual position on the screen
        /// </summary>
        public Vector2 ActualPosition
        {
            get { return _basePosition * MainWindow.ratio; }
        }

        /// <summary>
        /// Centered position after taking scale into account
        /// </summary>
        public Vector2 CenteredPosition
        {
            get { return ActualPosition - (ActualScale / 2); }
        }
        #endregion

        #region scale logic
        private Vector2 _baseScale;

        /// <summary>
        /// Base scale of the transform
        /// <Getter> returns <see cref="_baseScale"/> </Getter>
        /// <Setter> If scale changed, calls <see cref="OnSizeChanged"/> </Setter>
        /// </summary>
        public Vector2 Scale
        {
            get { return _baseScale; }
            set
            {
                Vector2 TempScale = _baseScale;
                _baseScale = value;
                if (TempScale != value)
                    OnSizeChanged();
            }
        }

        /// <summary>
        /// Actual scale on the screen
        /// </summary>
        public Vector2 ActualScale
        {
            get { return _baseScale * MainWindow.ratio; }
        }
        #endregion

        /// <summary>
        /// Builds a transform
        /// </summary>
        /// <param name="scale"> The scale of the transform </param>
        /// <param name="position"> The position of the transform </param>
        public Transform(Vector2 scale, Vector2 position)
        {
            Transforms.Add(this);

            _baseScale = scale;
            _basePosition = position;
        }

        /// <summary>
        /// Called when <see cref="MainWindow"/>'s size has changed, alerts the relevant registered components
        /// <seealso cref="Sprite"/> <seealso cref="Collider"/> <seealso cref="CustomButton"/> <seealso cref="CustomLabel"/> <seealso cref="CustomTextBox"/> 
        /// </summary>
        public void OnSizeChanged()
        {
            _positionChanged?.Invoke();
            _scaleChanged?.Invoke();
        }

        /// <summary>
        /// Dispose the transform by removing it from <see cref="Transforms"/>
        /// </summary>
        public void Dispose()
        {
            Transforms.Remove(this);
        }
    }
}