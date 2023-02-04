using SpaceInvaders.Components.GameComponents;
using SpaceInvaders.Systems;
using SpaceInvadersGameWindow.Components.Pages;
using System.Numerics;
using System.Windows;

namespace SpaceInvadersGameWindow.Components
{
    internal class GameInitializer
    {
        public static GameInitializer? instance;
        public GameInitializer(Window window)
        {
            instance = this;

            new InputHandler(window);

            StartLoginRegist();
            //StartGameMenu();
            //StartGame();
        }

        public void StartLoginRegist()
        {
            new LoginRegistPage();
        }
        public void StartGameMenu()
        {
            new GameMainMenuPage();
        }
        public void StartGame()
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
            Invader.StartInvaders();
        }

    }
}