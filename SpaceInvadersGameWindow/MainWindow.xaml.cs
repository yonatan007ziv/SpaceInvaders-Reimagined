using SpaceInvaders.Components.Miscellaneous;
using SpaceInvaders.Systems;
using SpaceInvadersGameWindow.Components;
using SpaceInvadersGameWindow.Components.Initializers;
using System;
using System.Numerics;
using System.Windows;
using System.Windows.Controls;

namespace SpaceInvadersGameWindow
{
    public partial class MainWindow : Window
    {
        public static MainWindow? instance;
        public static int TargetFPS = 60;
        public static float ratio;

        Transform CenteredXCanvasTransform;
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

            Width = 1024;
            Height = 1024;

            CenteredXCanvasTransform = new Transform(new Vector2(256, 256), new Vector2(0, 0));
            CenteredXCanvasTransform.PositionChanged += () =>
            {
                CenteredCanvas.SetValue(Canvas.LeftProperty, (double)CenteredXCanvasTransform.CenteredPosition.X);
                CenteredCanvas.SetValue(Canvas.TopProperty, (double)CenteredXCanvasTransform.CenteredPosition.Y);
            };
            CenteredXCanvasTransform.ScaleChanged += () =>
            {
                CenteredCanvas.Width = CenteredXCanvasTransform.ActualScale.X;
                CenteredCanvas.Height = CenteredXCanvasTransform.ActualScale.Y;
            };

            new InputHandler(this);
            GameInitializers.StartLoginRegist();
        }

        public void CalculateRatio()
        {
            double ratioX = Width / 256;
            double ratioY = Height / 256;
            ratio = (float)Math.Min(ratioX, ratioY);
        }
    }
}