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
        public static MainWindow? instance;
        public MainWindow()
        {
            InitializeComponent();
            instance = this;
            new InputHandler();

            new Player(new Vector2(0, 700));
            new Wall(new Vector2(0,0), new Vector2(500, 500));
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