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
        CustomButton SingleplayerButton, MultiplayerButton, OptionsButton;
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
        private void OnSingleplayer()
        {
            Dispose();
            GameInitializers.StartSingleplayerGame();
        }
        private void OnMultiplayer()
        {
            Dispose();
            GameInitializers.StartMultiplayerGameMenu();
        }
        private void OnOptions()
        {
            Dispose();
            GameInitializers.StartOptionsMenu();
        }
        public void Dispose()
        {
            SingleplayerButton.Dispose();
            MultiplayerButton.Dispose();
            OptionsButton.Dispose();
        }
    }
}