using GameWindow.Components.Miscellaneous;
using GameWindow.Components.Initializers;
using GameWindow.Components.UIElements;
using System.Numerics;
using System.Security.Principal;

namespace GameWindow.Components.Pages
{
    internal class GameMultiplayerMenu
    {
        private CustomLabel ipLabel;
        private CustomLabel portLabel;
        private CustomTextInput ipInput;
        private CustomTextInput portInput;
        private CustomButton connectButton;

        public GameMultiplayerMenu()
        {
            ipLabel = new CustomLabel(new Transform(new Vector2(50, 50), new Vector2(15, 50)), "IP:", System.Windows.Media.Colors.White);
            portLabel = new CustomLabel(new Transform(new Vector2(50, 50), new Vector2(15, 100)), "PORT:", System.Windows.Media.Colors.White);
            ipInput = new CustomTextInput(new Transform(new Vector2(125, 50), new Vector2(100, 50)));
            portInput = new CustomTextInput(new Transform(new Vector2(125, 50), new Vector2(100, 100)));
            connectButton = new CustomButton(new Transform(new Vector2(50, 50), new Vector2(100, 225)), OnConnect, "", "Connect");
        }
        private void Dispose()
        {
            ipLabel.Dispose();
            portLabel.Dispose();
            ipInput.Dispose();
            portInput.Dispose();
            connectButton.Dispose();
        }
        void OnConnect()
        {
            if (ipInput.Text == "" && portInput.Text == "")
                return;
            Dispose();
            new MultiplayerGameInitializer(ipInput.Text, int.Parse(portInput.Text), GameInitializers.username!);
            connectButton.Dispose();
        }
    }
}