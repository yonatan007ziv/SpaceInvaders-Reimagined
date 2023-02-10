using GameWindow.Components.Miscellaneous;
using GameWindow.Components.Initializers;
using GameWindow.Components.UIElements;
using System.Numerics;
using System.Windows;

namespace GameWindow.Components.Pages
{
    internal class GameMainMenu
    {
        CustomButton SingleplayerButton, MultiplayerButton, OptionsButton;
        public GameMainMenu()
        {
            Application.Current.Dispatcher.Invoke(() =>
            {
                SingleplayerButton = new CustomButton(new Transform(new Vector2(50, 50), new Vector2(25, 50)), OnSingleplayer, "", "Singleplayer");
                MultiplayerButton = new CustomButton(new Transform(new Vector2(50, 50), new Vector2(100, 50)), OnMultiplayer, "", "Multiplayer");
                OptionsButton = new CustomButton(new Transform(new Vector2(50, 50), new Vector2(175, 50)), OnOptions, "", "Options");
            });
        }
        public void Dispose()
        {
            SingleplayerButton.Dispose();
            MultiplayerButton.Dispose();
            OptionsButton.Dispose();
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
            throw new System.Exception();
        }
    }
}