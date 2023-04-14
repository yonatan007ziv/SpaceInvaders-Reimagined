using GameWindow.Components.Initializers;
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
                SingleplayerButton = new CustomButton(new Transform(new Vector2(MainWindow.referenceSize.X / 6, MainWindow.referenceSize.Y / 6), new Vector2(MainWindow.referenceSize.X / 2 - 100, MainWindow.referenceSize.Y / 2)), OnSingleplayer, "", "Singleplayer");
                MultiplayerButton = new CustomButton(new Transform(new Vector2(MainWindow.referenceSize.X / 6, MainWindow.referenceSize.Y / 6), new Vector2(MainWindow.referenceSize.X / 2, MainWindow.referenceSize.Y / 2)), OnMultiplayer, "", "Multiplayer");
                OptionsButton = new CustomButton(new Transform(new Vector2(MainWindow.referenceSize.X / 6, MainWindow.referenceSize.Y / 6), new Vector2(MainWindow.referenceSize.X / 2 + 100, MainWindow.referenceSize.Y / 2)), OnOptions, "", "Options");
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
            GameInitializers.StartSingleplayerGame();
        }

        /// <summary>
        /// Starts a new <see cref="GameMultiplayerMenu"/> page
        /// </summary>
        private void OnMultiplayer()
        {
            Dispose();
            GameInitializers.StartMultiplayerGameMenu();
        }

        /// <summary>
        /// Starts a new <see cref="OptionsMenu"/> page
        /// </summary>
        private void OnOptions()
        {
            Dispose();
            GameInitializers.StartOptionsMenu();
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