using GameWindow.Components.Miscellaneous;
using GameWindow.Components.NetworkedComponents;
using GameWindow.Components.UIElements;
using System.Numerics;
using System.Windows;

namespace GameWindow.Components.Pages
{
    /// <summary>
    /// A page representing the online game "pause" menu
    /// </summary>
    internal class OnlinePauseMenu
    {
        private CustomLabel pauseLabel;
        private CustomButton mainMenuButton;

        /// <summary>
        /// Builds the online "pause" menu page
        /// </summary>
        public OnlinePauseMenu()
        {
            Application.Current.Dispatcher.Invoke(() =>
            { // UI Objects need to be created in an STA thread
                pauseLabel = new CustomLabel(new Transform(new Vector2(50, 50), new Vector2(100, 50)), "Menu", System.Windows.Media.Colors.White);
                mainMenuButton = new CustomButton(new Transform(new Vector2(50, 50), new Vector2(100, 100)), MainMenu, System.Windows.Media.Color.FromRgb(0, 0, 0), "Main Menu");
            });

            // Suppressing the "Null When Leaving a Constructor" warning
            pauseLabel!.ToString();
            mainMenuButton!.ToString();
        }

        /// <summary>
        /// Goes back to the Main Menu
        /// </summary>
        private void MainMenu()
        {
            Dispose();
            NetworkedGameClient.instance?.Dispose();
            new GameMainMenu();
        }

        /// <summary>
        /// Disposes the current <see cref="OnlinePauseMenu"/> page
        /// </summary>
        public void Dispose()
        {
            pauseLabel.Dispose();
            mainMenuButton.Dispose();
        }
    }
}
