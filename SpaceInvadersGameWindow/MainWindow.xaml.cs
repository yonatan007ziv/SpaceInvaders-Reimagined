using SpaceInvaders.Components.GameComponents;
using SpaceInvaders.Components.Miscellaneous;
using SpaceInvaders.Systems;
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
        public static MainWindow? instance;
        public static float ratio;
        Transform CenteredXCanvasTransform;
        public MainWindow()
        {
            InitializeComponent();
            instance = this;
            SizeChanged += (s, e) => CalculateRatio();
            CalculateRatio();
            Width = 1920;
            Height = 1080;

            new InputHandler();

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

            //InitializeLoginRegist();
            InitializeGameMenu();
            //InitializeGame();
        }

        public static void CalculateRatio()
        {
            double ratioX = instance!.Width / 256;
            double ratioY = instance!.Height / 256;
            ratio = (float)Math.Min(ratioX, ratioY);
        }
        public void InitializeLoginRegist()
        {
            new LoginRegistPage();
        }
        public void InitializeGameMenu()
        {
            new GameMainMenuPage();
        }
        public void InitializeGame()
        {
            #region temp overlay
            //Transform ColorOverlayT = new Transform(new Vector2(256, 256), new Vector2(256 / 2, 256 / 2));
            //Sprite ColorOverlaySprite = new Sprite(ColorOverlayT, @"Resources\RawFiles\Images\Overlay.png");
            #endregion

            Wall.Ceiling = new Wall(new Vector2(256, 5), new Vector2(256 / 2, 0));
            Wall.Floor = new Wall(new Vector2(256, 5), new Vector2(256 / 2, 256));
            Wall.LeftWall = new Wall(new Vector2(5, 256), new Vector2(0, 256 / 2));
            Wall.RightWall = new Wall(new Vector2(5, 256), new Vector2(256 - 16, 256 / 2));

            new Player(new Vector2(50, 200));

            Invader.PlotInvaders(0, 0);

            TestGameLoopInvaders();
        }
        private async void TestGameLoopInvaders()
        {
            while (true)
            {
                Invader.MoveInvaders();

                await Task.Delay(Invader.invaders.Count * 50);
            }
        }
    }
}