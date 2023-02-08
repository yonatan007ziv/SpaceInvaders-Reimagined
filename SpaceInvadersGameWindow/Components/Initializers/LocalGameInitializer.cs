using SpaceInvaders.Components.GameComponents;
using SpaceInvaders.Components.Miscellaneous;
using SpaceInvadersGameWindow.Components.UIElements;
using System.Numerics;

namespace SpaceInvadersGameWindow.Components.Initializers
{
    internal class LocalGameInitializer
    {
        public static LocalGameInitializer? instance;

        private int score = 0;
        public int Score
        {
            get { return score; }
            set { score = value; CreditsLabel.Text = "CREDIT " + score; }
        }

        private CustomLabel CreditsLabel;

        public LocalGameInitializer()
        {
            instance = this;

            #region temp overlay
            //Transform ColorOverlayT = new Transform(new Vector2(256, 256), new Vector2(256 / 2, 256 / 2));
            //Sprite ColorOverlaySprite = new Sprite(ColorOverlayT, @"Resources\RawFiles\Images\Overlay.png");
            #endregion

            Wall.Ceiling = new Wall(new Vector2(256, 5), new Vector2(256 / 2, 2.5f), @"Resources\RawFiles\Images\Pixels\Red.png");
            Wall.Floor = new Wall(new Vector2(256, 5), new Vector2(256 / 2, 224), @"Resources\RawFiles\Images\Pixels\Green.png");
            Wall.LeftWall = new Wall(new Vector2(5, 256), new Vector2(0, 256 / 2));
            Wall.RightWall = new Wall(new Vector2(5, 256), new Vector2(256 - 16, 256 / 2));

            CreditsLabel = new CustomLabel(new Transform(new Vector2(50, 50), new Vector2(200, 200)),"", System.Windows.Media.Colors.White);
            Score = 0;

            new Player(new Vector2(50, 200));

            Invader.PlotInvaders(0, 0);
            Invader.StartInvaders();
        }
    }
}