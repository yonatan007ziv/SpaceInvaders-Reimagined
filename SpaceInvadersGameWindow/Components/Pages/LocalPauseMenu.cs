using GameWindow.Components.Initializers;
using GameWindow.Components.Miscellaneous;
using GameWindow.Components.UIElements;
using System.Numerics;
using System.Windows;

namespace GameWindow.Components.Pages
{
    /// <summary>
    /// A page representing the local game pause menu
    /// </summary>
    internal class LocalPauseMenu
    {
        CustomLabel pauseLabel;
        CustomButton newGameButton, mainMenuButton;

        public LocalPauseMenu()
        {
            LocalGame.Paused = true;

            Application.Current.Dispatcher.Invoke(() =>
            { // UI Objects need to be created in an STA thread
                pauseLabel = new CustomLabel(new Transform(new Vector2(50, 50), new Vector2(50, 25)), "Game Paused", System.Windows.Media.Colors.Black);
                newGameButton = new CustomButton(new Transform(new Vector2(50, 50), new Vector2(100, 100)), NewGame, "", "New Game");
                mainMenuButton = new CustomButton(new Transform(new Vector2(50, 50), new Vector2(150, 100)), MainMenu, "", "Main Menu");
            });

            // Suppressing the "Null When Leaving a Constructor" warning
            pauseLabel!.ToString();
            newGameButton!.ToString();
            mainMenuButton!.ToString();
        }

        private void NewGame()
        {
            Dispose();
            LocalGame.instance!.Dispose();
            LocalGame.instance.StartGame();
            LocalGame.Paused = false;
        }

        private void MainMenu()
        {
            Dispose();
            LocalGame.instance!.Dispose();
            GameInitializers.StartGameMenu(GameInitializers.username);
            LocalGame.Paused = false;
        }

        public void Dispose()
        {
            pauseLabel.Dispose();
            newGameButton.Dispose();
            mainMenuButton.Dispose();
        }
    }
}
