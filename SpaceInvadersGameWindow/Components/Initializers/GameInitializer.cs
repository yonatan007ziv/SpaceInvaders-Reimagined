using GameWindow.Components.Pages;

namespace GameWindow.Components.Initializers
{
    /// <summary>
    /// Helper class for initializing game parts
    /// </summary>
    internal static class GameInitializers
    {
        public static string username = "";

        /// <summary>
        /// Starts the "Login Register" page
        /// </summary>
        public static void StartLoginRegist()
        {
            new LoginRegister();
        }

        /// <summary>
        /// Starts the "Game Menu" page
        /// </summary>
        /// <param name="username"> Local player's username </param>
        public static void StartGameMenu(string username)
        {
            GameInitializers.username = username;
            new GameMainMenu();
        }

        /// <summary>
        /// Starts A local game
        /// </summary>
        public static void StartSingleplayerGame()
        {
            if (LocalGame.instance == null)
                new LocalGame();
            else
                LocalGame.instance?.StartGame();
        }

        /// <summary>
        /// Starts the "Multiplayer Menu" page
        /// </summary>
        public static void StartMultiplayerGameMenu()
        {
            new GameMultiplayerMenu();
        }

        /// <summary>
        /// Starts the "Game Options" page
        /// </summary>
        public static void StartOptionsMenu()
        {
            new OptionsMenu();
        }
    }
}