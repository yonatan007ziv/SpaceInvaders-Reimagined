using SpaceInvadersGameWindow.Components.Initializers;
using SpaceInvadersGameWindow.Components.UIElements;
using System.Numerics;

namespace SpaceInvadersGameWindow.Components.Pages
{
    internal class GameMainMenuPage
    {
        CustomButton SinglePlayerButton, MultiplayerButton, OptionsButton;
        public GameMainMenuPage()
        {
            SinglePlayerButton = new CustomButton(new Vector2(50, 50), new Vector2(25, 50), OnSingleplayer, "", "Singleplayer");
            MultiplayerButton = new CustomButton(new Vector2(50, 50), new Vector2(100, 50), OnMultiplayer, "", "Multiplayer");
            OptionsButton = new CustomButton(new Vector2(50, 50), new Vector2(175, 50), OnOptions, "", "Options");
        }
        public void Dispose()
        {
            SinglePlayerButton.Dispose();
            MultiplayerButton.Dispose();
            OptionsButton.Dispose();
        }
        private void OnSingleplayer()
        {
            SinglePlayerButton.Dispose();
            Dispose();
            new LocalGameInitializer();
        }
        private void OnMultiplayer()
        {
            MultiplayerButton.Dispose();
            Dispose();
            new GameMultiplayerMenu();
        }
        private void OnOptions()
        {
            OptionsButton.Dispose();
            Dispose();
            throw new System.Exception();
        }
    }
}