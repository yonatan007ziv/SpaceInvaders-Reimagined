using GameWindow.Components.GameComponents;
using GameWindow.Components.GameComponents.Bunker;
using GameWindow.Components.Miscellaneous;
using GameWindow.Components.UIElements;
using System.Numerics;
using System.Threading.Tasks;
using System.Windows;

namespace GameWindow.Components.Initializers
{
    internal class LocalGame
    {
        public static LocalGame? instance;

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

        private Player player;

        private async void StartGame()
        {
            Wall.Ceiling = new Wall(new Vector2(MainWindow.referenceSize.X, 5), new Vector2(MainWindow.referenceSize.X / 2, 5), @"Resources\Images\Pixels\Red.png");
            Wall.Floor = new Wall(new Vector2(MainWindow.referenceSize.X, 5), new Vector2(MainWindow.referenceSize.X / 2, MainWindow.referenceSize.Y / 1.08f), @"Resources\Images\Pixels\Green.png");
            Wall.LeftWall = new Wall(new Vector2(5, MainWindow.referenceSize.Y), new Vector2(25, MainWindow.referenceSize.Y / 2));
            Wall.RightWall = new Wall(new Vector2(5, MainWindow.referenceSize.Y), new Vector2(MainWindow.referenceSize.X - 25, MainWindow.referenceSize.Y / 2));


            Application.Current.Dispatcher.Invoke(() =>
            { // UI Objects need to be created in an STA thread

                CreditsLabel = new CustomLabel(new Transform(new Vector2(50, 50), new Vector2(MainWindow.referenceSize.X / 1.25f, MainWindow.referenceSize.Y / 1.15f)), "", System.Windows.Media.Colors.White);
                LivesLabel = new CustomLabel(new Transform(new Vector2(50, 50), new Vector2(25, MainWindow.referenceSize.Y / 1.15f)), "", System.Windows.Media.Colors.White);
            });
            Score = 0;
            LivesLeft = 3;

            new Bunker(new Vector2(0.4f * (MainWindow.referenceSize.X / 2), 2 * MainWindow.referenceSize.Y / 3));
            new Bunker(new Vector2(0.8f * (MainWindow.referenceSize.X / 2), 2 * MainWindow.referenceSize.Y / 3));
            new Bunker(new Vector2(1.2f * (MainWindow.referenceSize.X / 2), 2 * MainWindow.referenceSize.Y / 3));
            new Bunker(new Vector2(1.6f * (MainWindow.referenceSize.X / 2), 2 * MainWindow.referenceSize.Y / 3));

            player = new Player(new Vector2(50, MainWindow.referenceSize.Y * 0.8f), this);
            await Invader.PlotInvaders();
            Invader.StartInvaders(this, player);
            player.EnableInput();
        }

        public void Won()
        {
            //player.();
        }
        public void Lost()
        {
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
                    "You Lost", System.Windows.Media.Colors.White);

                PlayAgainBtn = new CustomButton(new Transform(new Vector2(MainWindow.referenceSize.X / 5, MainWindow.referenceSize.Y / 5), new Vector2(MainWindow.referenceSize.X * 3 / 4, MainWindow.referenceSize.Y * 5 / 6)),
                    () =>
                    {
                        LostLabel.Dispose();
                        PlayAgainBtn.Dispose();
                        MainMenuBtn.Dispose();
                        StartGame();
                    }, "", "Play Again");

                MainMenuBtn = new CustomButton(
                    new Transform(new Vector2(MainWindow.referenceSize.X / 5, MainWindow.referenceSize.Y / 5), new Vector2(MainWindow.referenceSize.X / 4, MainWindow.referenceSize.Y * 5 / 6)),
                    () =>
                    {
                        LostLabel.Dispose();
                        PlayAgainBtn.Dispose();
                        MainMenuBtn.Dispose();
                        GameInitializers.StartGameMenu(GameInitializers.username);
                    }, "", "Main Menu");
                // ...
            });
        }
    }
}