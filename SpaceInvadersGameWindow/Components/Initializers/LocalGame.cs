using GameWindow.Components.GameComponents;
using GameWindow.Components.Miscellaneous;
using GameWindow.Components.Pages;
using GameWindow.Components.UIElements;
using GameWindow.Systems;
using System.Numerics;
using System.Windows;
using System.Windows.Input;

namespace GameWindow.Components.Initializers
{
    internal class LocalGame
    {
        public static LocalGame? instance;

        private Player player;
        public bool didQuit;
        public bool isLost;
        private int score = 0;
        public int Score
        {
            get { return score; }
            set { score = value; CreditsLabel.Text = "CREDIT " + score; }
        }
        private int livesLeft = 3;
        public int LivesLeft
        {
            get { return livesLeft; }
            set { livesLeft = value; LivesLabel.Text = "LIVES: " + livesLeft; }
        }

        private CustomLabel CreditsLabel;
        private CustomLabel LivesLabel;
        private CustomLabel? LostLabel;
        private CustomButton? PlayAgainBtn;
        private CustomButton? MainMenuBtn;

        public LocalGame()
        {
            instance = this;

            #region temp overlay
            //Transform ColorOverlayT = new Transform(new Vector2(MainWindow.referenceSize.X, MainWindow.referenceSize.Y), new Vector2(MainWindow.referenceSize.X / 2, MainWindow.referenceSize.Y / 2));
            //Sprite ColorOverlaySprite = new Sprite(ColorOverlayT, @"Resources\Images\Overlay.png");
            #endregion

            StartGame();

            // Suppressing the "Null When Leaving a Constructor" warning
            CreditsLabel!.ToString();
            LivesLabel!.ToString();
            player!.ToString();
        }

        #region Game Preparation
        private async void StartGame()
        {
            isLost = false;
            MakeWall();
            MakeBunkers();

            Application.Current.Dispatcher.Invoke(() =>
            { // UI Objects need to be created in an STA thread
                CreditsLabel = new CustomLabel(new Transform(new Vector2(50, 50), new Vector2(MainWindow.referenceSize.X / 1.25f, MainWindow.referenceSize.Y / 1.15f)), "", System.Windows.Media.Colors.White);
                LivesLabel = new CustomLabel(new Transform(new Vector2(50, 50), new Vector2(25, MainWindow.referenceSize.Y / 1.15f)), "", System.Windows.Media.Colors.White);
            });
            Score = 0;
            LivesLeft = 3;

            InputHandler.AddInputLoop(InputLoop);
            Player.PauseUnpause(true);
            player = new Player(new Vector2(50, MainWindow.referenceSize.Y * 0.735f), this);
            await Invader.PlotInvaders();
            Invader.PauseUnpauseInvaders(false);
            Player.PauseUnpause(false);
            InputHandler.Disabled = false;
        }
        private void MakeWall()
        {
            Wall.Ceiling = new Wall(new Vector2(MainWindow.referenceSize.X, 5), new Vector2(MainWindow.referenceSize.X / 2, 5), @"Resources\Images\Pixels\Red.png");
            Wall.Floor = new Wall(new Vector2(MainWindow.referenceSize.X, 5), new Vector2(MainWindow.referenceSize.X / 2, MainWindow.referenceSize.Y / 1.08f), @"Resources\Images\Pixels\Green.png");
            Wall.LeftWall = new Wall(new Vector2(5, MainWindow.referenceSize.Y), new Vector2(25, MainWindow.referenceSize.Y / 2));
            Wall.RightWall = new Wall(new Vector2(5, MainWindow.referenceSize.Y), new Vector2(MainWindow.referenceSize.X - 25, MainWindow.referenceSize.Y / 2));
        }
        private void MakeBunkers()
        {
            new Bunker(new Vector2(0.4f * (MainWindow.referenceSize.X / 2), 2 * MainWindow.referenceSize.Y / 3));
            new Bunker(new Vector2(0.8f * (MainWindow.referenceSize.X / 2), 2 * MainWindow.referenceSize.Y / 3));
            new Bunker(new Vector2(1.2f * (MainWindow.referenceSize.X / 2), 2 * MainWindow.referenceSize.Y / 3));
            new Bunker(new Vector2(1.6f * (MainWindow.referenceSize.X / 2), 2 * MainWindow.referenceSize.Y / 3));
        }
        #endregion

        #region Pause Menu
        private static bool HeldEscape;
        private static bool paused;
        private static PauseMenu? pauseMenu;
        private void InputLoop()
        {
            if (InputHandler.keysDown.Contains(Key.Escape))
            {
                if (!HeldEscape)
                    PauseUnpause(!paused);
                HeldEscape = true;
            }
            else
                HeldEscape = false;
        }
        public static void PauseUnpause(bool pause)
        {
            if (pause)
            {
                Bullet.PauseUnpauseBullets(true);
                Invader.PauseUnpauseInvaders(true);
                Player.PauseUnpause(true);
                pauseMenu = new PauseMenu();
            }
            else
            {
                Bullet.PauseUnpauseBullets(false);
                Invader.PauseUnpauseInvaders(false);
                Player.PauseUnpause(false);
                pauseMenu?.Dispose();
            }
            paused = pause;
        }
        #endregion

        public void Lost()
        {
            isLost = true;
            PlayerBullet.instance = null;
            Wall.Ceiling!.Dispose();
            Wall.Floor!.Dispose();
            Wall.LeftWall!.Dispose();
            Wall.RightWall!.Dispose();
            player.Dispose();
            CreditsLabel.Dispose();
            LivesLabel.Dispose();
            Invader.DisposeAll();
            Bullet.DisposeAll();
            Bunker.DisposeAll();

            // UI Objects need to be created in an STA thread
            Application.Current.Dispatcher.Invoke(() =>
            {
                LostLabel = new CustomLabel(new Transform(new Vector2(MainWindow.referenceSize.X, MainWindow.referenceSize.Y), new Vector2(MainWindow.referenceSize.X / 2, MainWindow.referenceSize.Y / 2)),
                    $"You Lost\nCredits: {Score}", System.Windows.Media.Colors.White);

                PlayAgainBtn = new CustomButton(new Transform(new Vector2(MainWindow.referenceSize.X / 5, MainWindow.referenceSize.Y / 5), new Vector2(MainWindow.referenceSize.X * 3 / 4, MainWindow.referenceSize.Y * 5 / 6)),
                    () =>
                    {
                        DisposeLostMenu();
                        StartGame();
                    }, "", "Play Again");

                MainMenuBtn = new CustomButton(
                    new Transform(new Vector2(MainWindow.referenceSize.X / 5, MainWindow.referenceSize.Y / 5), new Vector2(MainWindow.referenceSize.X / 4, MainWindow.referenceSize.Y * 5 / 6)),
                    () =>
                    {
                        DisposeLostMenu();
                        GameInitializers.StartGameMenu(GameInitializers.username);
                    }, "", "Main Menu");
                // ...
            });
        }
        private void DisposeLostMenu()
        {
            LostLabel?.Dispose();
            PlayAgainBtn?.Dispose();
            MainMenuBtn?.Dispose();
        }
        public void Dispose()
        {
            Wall.DisposeAll();
            Bunker.DisposeAll();
            Bullet.DisposeAll();
            Invader.DisposeAll();
            player.Dispose();
            LivesLabel.Dispose();
            CreditsLabel.Dispose();
        }
    }
}