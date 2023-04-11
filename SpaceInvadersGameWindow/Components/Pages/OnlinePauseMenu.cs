using GameWindow.Components.Initializers;
using GameWindow.Components.Miscellaneous;
using GameWindow.Components.NetworkedComponents;
using GameWindow.Components.UIElements;
using System.Numerics;
using System.Windows;

namespace GameWindow.Components.Pages
{
    internal class OnlinePauseMenu
    {
        CustomLabel pauseLabel;
        CustomButton mainMenuButton;
        public OnlinePauseMenu()
        {
            LocalGame.Paused = true;

            Application.Current.Dispatcher.Invoke(() =>
            { // UI Objects need to be created in an STA thread
                pauseLabel = new CustomLabel(new Transform(new Vector2(50, 50), new Vector2(50, 25)), "Menu", System.Windows.Media.Colors.Black);
                mainMenuButton = new CustomButton(new Transform(new Vector2(50, 50), new Vector2(150, 100)), MainMenu, "", "Main Menu");
            });

            // Suppressing the "Null When Leaving a Constructor" warning
            pauseLabel!.ToString();
            mainMenuButton!.ToString();
        }
        private void MainMenu()
        {
            Dispose();
            MultiplayerGameClientInitializer.instance!.Dispose();
            GameInitializers.StartGameMenu(GameInitializers.username);
        }
        public void Dispose()
        {
            pauseLabel.Dispose();
            mainMenuButton.Dispose();
        }
    }
}
