using GameWindow.Components.GameComponents;
using GameWindow.Components.Miscellaneous;
using GameWindow.Components.UIElements;
using System.Numerics;
using System.Windows;

namespace GameWindow.Components.Pages
{
    /// <summary>
    /// A page representing the main menu of the game
    /// </summary>
    internal class GameMainMenu
    {
        private CustomButton SingleplayerButton, MultiplayerButton, OptionsButton;

        /// <summary>
        /// Builds a new Main Menu page
        /// </summary>
        public GameMainMenu()
        {
            Application.Current.Dispatcher.Invoke(() =>
            { // UI Objects need to be created in an STA thread
                SingleplayerButton = new CustomButton(new Transform(new Vector2(MainWindow.referenceSize.X / 6, MainWindow.referenceSize.Y / 6), new Vector2(MainWindow.referenceSize.X / 2 - 75, MainWindow.referenceSize.Y / 2)), OnSingleplayer, System.Windows.Media.Color.FromRgb(0, 255, 0), "Singleplayer");
                MultiplayerButton = new CustomButton(new Transform(new Vector2(MainWindow.referenceSize.X / 6, MainWindow.referenceSize.Y / 6), new Vector2(MainWindow.referenceSize.X / 2, MainWindow.referenceSize.Y / 2)), OnMultiplayer, System.Windows.Media.Color.FromRgb(0, 0, 255), "Multiplayer");
                OptionsButton = new CustomButton(new Transform(new Vector2(MainWindow.referenceSize.X / 6, MainWindow.referenceSize.Y / 6), new Vector2(MainWindow.referenceSize.X / 2 + 75, MainWindow.referenceSize.Y / 2)), OnOptions, System.Windows.Media.Color.FromRgb(255, 0, 0), "Options");
            });

            // Suppressing the "Null When Leaving a Constructor" warning
            SingleplayerButton!.ToString();
            MultiplayerButton!.ToString();
            OptionsButton!.ToString();
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
            SingleplayerButton.Dispose();
            MultiplayerButton.Dispose();
            OptionsButton.Dispose();
        }
    }
}