using SpaceInvaders.Components.GameComponents;
using SpaceInvaders.Components.Miscellaneous;
using SpaceInvaders.Systems;
using SpaceInvadersGameWindow.Components;
using SpaceInvadersGameWindow.Components.Pages;
using SpaceInvadersGameWindow.Components.UIElements;
using System;
using System.Numerics;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace SpaceInvadersGameWindow
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public static MainWindow instance;
        public static int TargetFPS = 60;
        public static float ratio;
        public void CalculateRatio()
        {
            double ratioX = Width / 256;
            double ratioY = Height / 256;
            ratio = (float)Math.Min(ratioX, ratioY);
        }

        Transform CenteredXCanvasTransform;
        public MainWindow()
        {
            InitializeComponent();
            instance = this;

            SizeChanged += (s, e) => CalculateRatio();
            CalculateRatio();

            Width = 1920;
            Height = 1080;

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

            new GameInitializer(this);
        }
    }
}