using GameWindow.Components.Pages;
using System;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Windows.Threading;

namespace GameWindow.Components.Initializers
{
    internal static class GameInitializers
    {
        public static string username = "";
        public static void StartLoginRegist()
        {
            new LoginRegist();
        }

        public static void StartGameMenu(string username)
        {
            GameInitializers.username = username;
            new GameMainMenu();
        }

        public static void StartSingleplayerGame()
        {
            new LocalGame();
        }

        public static void StartMultiplayerGameMenu()
        {
            new GameMultiplayerMenu();
        }
    }
}