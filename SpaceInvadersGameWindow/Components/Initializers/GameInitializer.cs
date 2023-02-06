using SpaceInvadersGameWindow.Components.Pages;

namespace SpaceInvadersGameWindow.Components.Initializers
{
    internal class GameInitializer
    {
        public static GameInitializer? instance;

        public GameInitializer()
        {
            instance = this;

            //StartLoginRegist();
            StartGameMenu();
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
    }
}