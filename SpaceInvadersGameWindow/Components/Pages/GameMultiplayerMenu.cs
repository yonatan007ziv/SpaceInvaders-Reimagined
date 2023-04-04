using GameWindow.Components.Miscellaneous;
using GameWindow.Components.Initializers;
using GameWindow.Components.UIElements;
using System.Numerics;
using System.Security.Principal;
using System.Windows;

namespace GameWindow.Components.Pages
{
    internal class GameMultiplayerMenu
    {
        private CustomLabel ipLabel, portLabel;
        private CustomTextInput ipInput, portInput;
        private CustomButton connectButton, backButton;

        public GameMultiplayerMenu()
        {
            Application.Current.Dispatcher.Invoke(() =>
            { // UI Objects need to be created in an STA thread
                ipLabel = new CustomLabel(new Transform(new Vector2(50, 50), new Vector2(15, 50)), "IP:", System.Windows.Media.Colors.White);
                portLabel = new CustomLabel(new Transform(new Vector2(50, 50), new Vector2(15, 100)), "PORT:", System.Windows.Media.Colors.White);
                ipInput = new CustomTextInput(new Transform(new Vector2(125, 50), new Vector2(100, 50)));
                ipInput.Text = "127.0.0.1"; // temp
                portInput = new CustomTextInput(new Transform(new Vector2(125, 50), new Vector2(100, 100)));
                portInput.Text = "7778"; // temp
                connectButton = new CustomButton(new Transform(new Vector2(50, 50), new Vector2(125, 225)), OnConnect, "", "Connect");
                backButton = new CustomButton(new Transform(new Vector2(50, 50), new Vector2(75, 225)), OnBack, "", "Back");
            });

            // Suppressing the "Null When Leaving a Constructor" warning
            ipLabel!.ToString();
            portLabel!.ToString();
            ipInput!.ToString();
            portInput!.ToString();
            connectButton!.ToString();
        }
        private void Dispose()
        {
            ipLabel.Dispose();
            portLabel.Dispose();
            ipInput.Dispose();
            portInput.Dispose();
            connectButton.Dispose();
            backButton.Dispose();
        }
        void OnConnect()
        {
            if (ipInput.Text == "" && portInput.Text == "")
                return;
            Dispose();
            new MultiplayerGameClient(ipInput.Text, int.Parse(portInput.Text), GameInitializers.username!);
        }
        void OnBack()
        {
            Dispose();
            GameInitializers.StartGameMenu(GameInitializers.username);
        }
    }
}