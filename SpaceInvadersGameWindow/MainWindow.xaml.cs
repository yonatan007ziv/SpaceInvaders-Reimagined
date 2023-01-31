using SpaceInvaders.Components.GameComponents;
using SpaceInvaders.Systems;
using SpaceInvadersGameWindow.Components.UIElements;
using SpaceInvadersGameWindow.Components.UIForms;
using SpaceInvadersGameWindow.Systems.Networking;
using System;
using System.Net;
using System.Net.Sockets;
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
        public MainWindow()
        {
            InitializeComponent();
            instance = this;
            SizeChanged += (s, e) => CalculateRatio();
            CalculateRatio();

            new InputHandler();

            InitializeLoginRegist();
            //InitializeGameMenu();
            //InitializeGame();
        }

        public static void CalculateRatio()
        {
            double ratioX = instance!.Width / 256;
            double ratioY = instance!.Height / 256;
            ratio = (float)Math.Min(ratioX, ratioY);
        }
        private void InitializeLoginRegist()
        {
            new RegistValidator("YonatanZiv", "Shifus");
        }
        private void InitializeGameMenu()
        {
            NavigableButton playButton = new NavigableButton(new Vector2(50, 50), new Vector2(100, 100), () => InitializeGame(), true);
        }
        private void InitializeGame()
        {
            /*
            #region temp overlay
            Transform backgroundT = new Transform(new Vector2(256, 256), new Vector2(256 / 2, 256 / 2));
            SpriteRenderer backgroundSR = new SpriteRenderer(backgroundT, @"Resources\RawFiles\Images\Background.png");

            //Transform overlayT = new Transform(new Vector2(256, 256), new Vector2(256 / 2, 256 / 2));
            //SpriteRenderer overlaySR = new SpriteRenderer(overlayT, @"Resources\RawFiles\Images\Overlay.png");
            #endregion
            */

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