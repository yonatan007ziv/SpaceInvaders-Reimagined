using GameWindow.Components.GameComponents;
using GameWindow.Components.Miscellaneous;
using GameWindow.Components.UIElements;
using System.Numerics;

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

        private CustomLabel CreditsLabel;

        public LocalGame()
        {
            instance = this;

            #region temp overlay
            //Transform ColorOverlayT = new Transform(new Vector2(256, 256), new Vector2(256 / 2, 256 / 2));
            //Sprite ColorOverlaySprite = new Sprite(ColorOverlayT, @"Resources\Images\Overlay.png");
            #endregion

            Wall.Ceiling = new Wall(new Vector2(MainWindow.referenceSize.X, 5), new Vector2(MainWindow.referenceSize.X / 2, 5), @"Resources\Images\Pixels\Red.png");
            Wall.Floor = new Wall(new Vector2(MainWindow.referenceSize.X, 5), new Vector2(MainWindow.referenceSize.X / 2, MainWindow.referenceSize.Y - 25), @"Resources\Images\Pixels\Green.png");
            Wall.LeftWall = new Wall(new Vector2(5, MainWindow.referenceSize.Y), new Vector2(5, MainWindow.referenceSize.Y / 2));
            Wall.RightWall = new Wall(new Vector2(5, MainWindow.referenceSize.Y), new Vector2(MainWindow.referenceSize.X-5, MainWindow.referenceSize.Y / 2));

            CreditsLabel = new CustomLabel(new Transform(new Vector2(50, 50), new Vector2(200, 200)),"", System.Windows.Media.Colors.White);
            Score = 0;

            player = new Player(new Vector2(50, MainWindow.referenceSize.Y * 0.8f), this);

            Invader.PlotInvaders(0, 25);
            Invader.StartInvaders(this);
        }

        private Player player;

        public void Won()
        {
            player.StopInput();
        }
        public void Lost()
        {

        }
    }
}