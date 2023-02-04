using SpaceInvadersGameWindow.Components.UIElements;
using System.Numerics;

namespace SpaceInvadersGameWindow.Components.Pages
{
    internal class GameMainMenuPage
    {
        CustomButton SinglePlayerButton, MultiplayerButton, OptionsButton;
        public GameMainMenuPage()
        {
            SinglePlayerButton = new CustomButton(new Vector2(50, 50), new Vector2(25, 50), () => { Dispose(); GameInitializer.instance!.StartGame(); }, "", true, "Singleplayer");
            MultiplayerButton = new CustomButton(new Vector2(50, 50), new Vector2(100, 50), () => { Dispose(); throw new System.NotImplementedException(); }, "", true, "Multiplayer");
            OptionsButton = new CustomButton(new Vector2(50, 50), new Vector2(175, 50), () => { Dispose(); throw new System.NotImplementedException(); }, "", true,"Options");
        }
        public void Dispose()
        {
            SinglePlayerButton.Dispose();
            MultiplayerButton.Dispose();
            OptionsButton.Dispose();
        }
    }
}