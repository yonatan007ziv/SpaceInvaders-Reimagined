using GameWindow.Components.GameComponents;
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
        private CustomLabel pauseLabel;
        private CustomButton newGameButton, mainMenuButton;

        /// <summary>
        /// Builds the local game pause menu page
        /// </summary>
        public LocalPauseMenu()
        {
            LocalGame.Paused = true;

            Application.Current.Dispatcher.Invoke(() =>
            { // UI Objects need to be created in an STA thread
                pauseLabel = new CustomLabel(new Transform(new Vector2(100, 100), new Vector2(125, 60)), "Game Paused", System.Windows.Media.Colors.Blue);
                newGameButton = new CustomButton(new Transform(new Vector2(50, 50), new Vector2(100, 100)), NewGame, "", "New Game");
                mainMenuButton = new CustomButton(new Transform(new Vector2(50, 50), new Vector2(150, 100)), MainMenu, "", "Main Menu");
            });

            // Suppressing the "Null When Leaving a Constructor" warning
            pauseLabel!.ToString();
            newGameButton!.ToString();
            mainMenuButton!.ToString();
        }

        /// <summary>
        /// Starts a new <see cref="LocalGame"/>
        /// </summary>
        private void NewGame()
        {
            Dispose();
            LocalGame.instance!.Dispose();
            LocalGame.instance.StartGame();
            LocalGame.Paused = false;
        }

        /// <summary>
        /// Goes back to the Main Menu page
        /// </summary>
        private void MainMenu()
        {
            Dispose();
            LocalGame.instance!.Dispose();
            new GameMainMenu();
            LocalGame.Paused = false;
        }

        /// <summary>
        /// Disposes the current <see cref="LocalPauseMenu"/> page
        /// </summary>
        public void Dispose()
        {
            pauseLabel.Dispose();
            newGameButton.Dispose();
            mainMenuButton.Dispose();
        }
    }
}
