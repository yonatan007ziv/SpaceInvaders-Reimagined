using SpaceInvadersGameWindow.Components.Initializers;
using SpaceInvadersGameWindow.Components.UIElements;
using System.Numerics;

namespace SpaceInvadersGameWindow.Components.Pages
{
    internal class GameMultiplayerMenu
    {
        private CustomLabel ipLabel;
        private CustomLabel portLabel;
        private CustomLabel usernameLabel;
        private CustomTextInput ipInput;
        private CustomTextInput portInput;
        private CustomTextInput usernameInput;
        private CustomButton connectButton;
        public GameMultiplayerMenu()
        {
            ipLabel = new CustomLabel(new Vector2(50, 50), new Vector2(25, 50), "IP:", System.Windows.Media.Colors.White);
            portLabel = new CustomLabel(new Vector2(50, 50), new Vector2(25, 100), "PORT:", System.Windows.Media.Colors.White);
            usernameLabel = new CustomLabel(new Vector2(50, 50), new Vector2(25, 150), "USERNAME:", System.Windows.Media.Colors.White);
            ipInput = new CustomTextInput(new Vector2(125, 50), new Vector2(100, 50));
            portInput = new CustomTextInput(new Vector2(125, 50), new Vector2(100, 100));
            usernameInput = new CustomTextInput(new Vector2(125, 50), new Vector2(100, 150));
            connectButton = new CustomButton(new Vector2(50, 50), new Vector2(100, 225), OnConnect, "", "Connect");
        }
        private void Dispose()
        {
            ipLabel.Dispose();
            portLabel.Dispose();
            usernameLabel.Dispose();
            ipInput.Dispose();
            portInput.Dispose();
            usernameInput.Dispose();
            connectButton.Dispose();
        }
        void OnConnect()
        {
            if (portInput.Text == "")
                return;
            Dispose();
            new MultiplayerGameInitializer(ipInput.Text, int.Parse(portInput.Text), usernameInput.Text);
            connectButton.Dispose();
        }
    }
}