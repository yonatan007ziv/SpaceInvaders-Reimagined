using GameWindow.Components.Initializers;
using GameWindow.Components.Miscellaneous;
using GameWindow.Systems;
using System;
using System.Numerics;
using System.Windows;
using System.Windows.Controls;

namespace GameWindow
{
    public partial class MainWindow : Window
    {
        public static readonly int TARGET_FPS = 60;
        public static MainWindow? instance;

        public static readonly Vector2 referenceSize = new Vector2(256, 256);//new Vector2(384, 448);
        public static float ratio;

        public InputHandler inputHandler;
        private Transform CenteredCanvasTransform;
        public MainWindow()
        {
            InitializeComponent();
            instance = this;

            Height = referenceSize.Y * 2;
            Width = referenceSize.X * 2;

            SizeChanged += (s, e) => CalculateRatio();
            CalculateRatio();

            SizeChanged += (s, e) =>
            {
                foreach (Transform T in Transform.transforms)
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

            inputHandler = new InputHandler(this);
            GameInitializers.StartGameMenu("YonatanZiv");
        }

        public void CalculateRatio()
        {
            double ratioX = Width / referenceSize.X;
            double ratioY = Height / referenceSize.Y;
            ratio = (float)Math.Min(ratioX, ratioY);
        }
    }
}