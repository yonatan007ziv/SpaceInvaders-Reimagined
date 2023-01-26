using SpaceInvaders.Components.GameComponents;
using SpaceInvaders.Components.PhysicsEngine;
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
        public static int GlobalTempZoom = 3;
        public static MainWindow? instance;
        public MainWindow()
        {
            InitializeComponent();
            instance = this;
            new InputHandler();

            Wall.Ceiling = new Wall(new Vector2((float)Width, 50), new Vector2((float)Width / 2, 0));

            Vector2 VerticalWallScale = new Vector2(50, (float)Height);
            Wall.LeftWall = new Wall(VerticalWallScale, new Vector2(50, (float)Height / 2));
            Wall.RightWall = new Wall(VerticalWallScale, new Vector2((float)Width - 25, (float)Height / 2));

            new Player(new Vector2(200, 450));

            Invader.PlotInvaders(0, 0);

            TestGameLoopInvaders();
        }
        private async void TestGameLoopInvaders()
        {
            while (true)
            {
                Invader.MoveInvaders();

                await Task.Delay(500);
            }
        }
    }
}