using GameWindow.Components.Miscellaneous;
using GameWindow.Systems;
using GameWindow.Components.Initializers;
using System;
using System.Numerics;
using System.Windows;
using System.Windows.Controls;
using GameWindow.Components.GameComponents;
using GameWindow.Components.UIElements;

namespace GameWindow
{
    public partial class MainWindow : Window
    {
        public static MainWindow? instance;
        public static int TargetFPS = 60;

        public readonly Vector2 referenceSize = new Vector2(512, 512);
        public static float ratio;

        public InputHandler inputHandler;
        private Transform CenteredXCanvasTransform;
        public MainWindow()
        {
            InitializeComponent();
            instance = this;

            SizeChanged += (s, e) => CalculateRatio();
            CalculateRatio();

            SizeChanged += (s, e) =>
            {
                foreach (Transform T in Transform.transforms)
                    T.OnSizeChanged();
            };

            CenteredXCanvasTransform = new Transform(new Vector2(512, 512), new Vector2(256, 256));
            CenteredXCanvasTransform.PositionChanged += () =>
            {
                Vector2 position = CenteredXCanvasTransform.CenteredPosition;
                CenteredCanvas.SetValue(Canvas.LeftProperty, (double)position.X);
                CenteredCanvas.SetValue(Canvas.TopProperty, (double)position.Y);
            };
            CenteredXCanvasTransform.ScaleChanged += () =>
            {
                Vector2 scale = CenteredXCanvasTransform.ActualScale;
                CenteredCanvas.Width = scale.X;
                CenteredCanvas.Height = scale.Y;
            };

            inputHandler = new InputHandler(this);
            GameInitializers.StartLoginRegist();
        }

        public void CalculateRatio()
        {
            double ratioX = Width / referenceSize.X;
            double ratioY = Height / referenceSize.Y;
            ratio = (float)Math.Min(ratioX, ratioY);
        }
    }
}