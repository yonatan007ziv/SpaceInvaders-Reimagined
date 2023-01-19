using SpaceInvaders.Components.GameComponents;
using SpaceInvaders.Components.Miscellaneous;
using SpaceInvaders.Components.PhysicsEngine;
using SpaceInvaders.DataStructures;
using SpaceInvaders.Resources;
using SpaceInvaders.Systems;
using System.Diagnostics;

namespace SpaceInvaders
{
    public partial class GameWindow : Form
    {
        public static GameWindow? Instance { get; private set; }
        public LoginRegist? loginRegist;
        public GameWindow()
        {
            InitializeComponent();
            Instance = this;
            loginRegist = new LoginRegist();
        }

        private void GameInitializer()
        {
            Time.StartDeltaTime();
            new InputHandler();

            new Wall(Vector2.Zero, new Vector2(1000,1));
            new Wall(new Vector2(Size.Width-50,5), new Vector2(1, 500));
            new Wall(new Vector2(50,5), new Vector2(1, 500));

            Player p = new Player(new Vector2(00, 550));
            p.transform.SetScale(new Vector2(65, 40));

            Invader.PlotInvaders(this, 100, 100);

            GameLoop();
            testInvaderLoop();
        }
        private async void testInvaderLoop()
        {
            while (true)
            {
                Invader.MoveInvaders();

                await Task.Delay(1000 / 15);
            }

        }
        private async void GameLoop()
        {
            while (true)
            {
                //Take Input
                InputHandler.instance!.InputUpdate();

                //Move Textures
                foreach (Transform t in Transform.transforms)
                    t.UpdatePosition();


                // Check Collisions
                Wall.CheckWallCollisions();

                //Invader.MoveInvaders();

                await Task.Delay(1000 / 60);
            }
        }
    }
}