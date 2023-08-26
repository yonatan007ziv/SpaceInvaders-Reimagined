using GameWindow.Components.GameComponents;
using GameWindow.Components.Miscellaneous;
using GameWindow.Components.UIElements;
using GameWindow.Factories;
using System.Numerics;

namespace GameWindow.Components.Pages
{
    /// <summary>
    /// A page representing the main menu of the game
    /// </summary>
    internal class GameMainMenu
    {
        private CustomButton singleplayerButton, multiplayerButton, optionsButton;

        /// <summary>
        /// Builds a new Main Menu page
        /// </summary>
        public GameMainMenu()
        {
            singleplayerButton = UIElementFactory.CreateButton(new Transform(new Vector2(MainWindow.referenceSize.X / 6, MainWindow.referenceSize.Y / 6), new Vector2(MainWindow.referenceSize.X / 2 - 75, MainWindow.referenceSize.Y / 2)), OnSingleplayer, System.Windows.Media.Color.FromRgb(0, 255, 0), "Singleplayer");
            multiplayerButton = UIElementFactory.CreateButton(new Transform(new Vector2(MainWindow.referenceSize.X / 6, MainWindow.referenceSize.Y / 6), new Vector2(MainWindow.referenceSize.X / 2, MainWindow.referenceSize.Y / 2)), OnMultiplayer, System.Windows.Media.Color.FromRgb(0, 0, 255), "Multiplayer");
            optionsButton = UIElementFactory.CreateButton(new Transform(new Vector2(MainWindow.referenceSize.X / 6, MainWindow.referenceSize.Y / 6), new Vector2(MainWindow.referenceSize.X / 2 + 75, MainWindow.referenceSize.Y / 2)), OnOptions, System.Windows.Media.Color.FromRgb(255, 0, 0), "Options");
        }

        /// <summary>
        /// Starts a new singleplayer game
        /// </summary>
        private void OnSingleplayer()
        {
            Dispose();
            if (LocalGame.instance == null)
                new LocalGame();
            else
                LocalGame.instance?.StartGame();
        }

        /// <summary>
        /// Starts a new <see cref="GameMultiplayerMenu"/> page
        /// </summary>
        private void OnMultiplayer()
        {
            Dispose();
            new GameMultiplayerMenu("");
        }

        /// <summary>
        /// Starts a new <see cref="OptionsMenu"/> page
        /// </summary>
        private void OnOptions()
        {
            Dispose();
            new OptionsMenu();
        }

        /// <summary>
        /// Disposes the current <see cref="GameMainMenu"/> page
        /// </summary>
        public void Dispose()
        {
            singleplayerButton.Dispose();
            multiplayerButton.Dispose();
            optionsButton.Dispose();
        }
    }
}