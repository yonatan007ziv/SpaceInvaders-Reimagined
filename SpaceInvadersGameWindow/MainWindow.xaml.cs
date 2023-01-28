using SpaceInvaders.Components.GameComponents;
using SpaceInvaders.Components.PhysicsEngine;
using SpaceInvaders.Components.Renderer;
using SpaceInvaders.Systems;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Input.StylusWisp;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace SpaceInvadersGameWindow
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public static MainWindow? instance;
        public static float ratio = 1;
        public void CalculateRatio()
        {
            double ratioX = MainWindow.instance!.Width / 256;
            double ratioY = MainWindow.instance!.Height / 256;
            ratio = (float)Math.Min(ratioX, ratioY);
        }
        public MainWindow()
        {
            InitializeComponent();
            SizeChanged += (s, e) => CalculateRatio();
            instance = this;
            new InputHandler();
            Wall.Ceiling = new Wall(new Vector2((float)Width, 5), new Vector2((float)Width / 2, 0));

            Vector2 VerticalWallScale = new Vector2(5, (float)Height);
            Wall.LeftWall = new Wall(VerticalWallScale, new Vector2(5, (float)Height / 2));
            Wall.RightWall = new Wall(VerticalWallScale, new Vector2((float)Width - 25, (float)Height / 2));

            new Player(new Vector2(50, 200));

            //SpriteRenderer overlay = new SpriteRenderer(@"Resources\RawFiles\Images\Overlay.png", new Vector2(256, 256), new Vector2(0, 0));
            //overlay.Opacity = 0.25;

            Invader.PlotInvaders(0, 0);

            TestGameLoopInvaders();
        }
        private async void TestGameLoopInvaders()
        {
            while (true)
            {
                Invader.MoveInvaders();

                await Task.Delay(1000);
            }
        }
    }
}