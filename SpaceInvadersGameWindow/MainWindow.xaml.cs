using GameWindow.Components.GameComponents;
using GameWindow.Components.Miscellaneous;
using GameWindow.Components.Pages;
using GameWindow.Systems;
using System;
using System.Numerics;
using System.Windows;
using System.Windows.Controls;

namespace GameWindow
{
    /// <summary>
    /// Main Window
    /// </summary>
    public partial class MainWindow : Window
    {
        /// <summary>
        /// Target FPS for systems like <see cref="InputHandler"/>
        /// Also for smooth-moving objects such as:
        /// <list type="bullet">
        ///     <item> <see cref="Player"/> </item>
        ///     <item> <see cref="Bullet"/> </item>
        ///     <item> <see cref="Invader.InvaderType.UFO"/> </item>
        /// </list>
        /// </summary>
        public const int TARGET_FPS = 60;

        public static MainWindow? instance;
        public static string username = "";

        /// <summary>
        /// A ratio between current screen size and <see cref="referenceSize"/>
        /// </summary>
        public static float ratio;

        /// <summary>
        /// Reference size for standardizing measurements such as <see cref="Wall"/> length
        /// </summary>
        public static readonly Vector2 referenceSize = new Vector2(256, 320);

        private Transform CenteredCanvasTransform;

        /// <summary>
        /// Builds the Main Window
        /// </summary>
        public MainWindow()
        {
            InitializeComponent();
            instance = this;

            Width = referenceSize.X * 3;
            Height = referenceSize.Y * 3;

            // Calculate base ratio
            CalculateRatio();

            // Register all transforms to Window.SizeChanged
            SizeChanged += (s, e) =>
            {
                CalculateRatio();
                foreach (Transform T in Transform.Transforms)
                    T.OnSizeChanged();
            };

            CenteredCanvasTransform = new Transform(referenceSize, new Vector2(referenceSize.X / 2, referenceSize.Y / 2));
            CenteredCanvasTransform.PositionChanged += () =>
            {
                Vector2 position = CenteredCanvasTransform.CenteredPosition;
                CenteredCanvas.SetValue(Canvas.LeftProperty, (double)position.X);
                CenteredCanvas.SetValue(Canvas.TopProperty, (double)position.Y);
            };
            CenteredCanvasTransform.ScaleChanged += () =>
            {
                Vector2 scale = CenteredCanvasTransform.ActualScale;
                CenteredCanvas.Width = scale.X;
                CenteredCanvas.Height = scale.Y;
            };

            // Start taking inputs
            new InputHandler(this);

            // Start "Login Register" page
            new LoginRegister();
        }

        /// <summary>
        /// Calculates minimum (so canvas fits on screen) ratio between current size and <see cref="referenceSize"/>
        /// </summary>
        public void CalculateRatio()
        {
            double ratioX = Width / referenceSize.X;
            double ratioY = Height / referenceSize.Y;
            ratio = (float)Math.Min(ratioX, ratioY);
        }
    }
}