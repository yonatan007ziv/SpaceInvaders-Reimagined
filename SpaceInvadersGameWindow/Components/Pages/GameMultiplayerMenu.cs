using GameWindow.Components.Miscellaneous;
using GameWindow.Components.NetworkedComponents;
using GameWindow.Components.UIElements;
using GameWindow.Factories;
using System.Numerics;

namespace GameWindow.Components.Pages
{
    /// <summary>
    /// A page representing the online menu
    /// </summary>
    internal class GameMultiplayerMenu
    {
        private CustomLabel ipLabel, portLabel, resultLabel;
        private CustomTextBox ipInput, portInput;
        private CustomButton connectButton, backButton;

        /// <summary>
        /// Builds a new Multiplayer Menu page
        /// </summary>
        public GameMultiplayerMenu(string resultLabelText)
        {
            ipLabel = UIElementFactory.CreateLabel(new Transform(new Vector2(50, 50), new Vector2(MainWindow.referenceSize.X / 2 - 100, 50)), "IP:", System.Windows.Media.Colors.White);
            portLabel = UIElementFactory.CreateLabel(new Transform(new Vector2(50, 50), new Vector2(MainWindow.referenceSize.X / 2 - 100, 100)), "PORT:", System.Windows.Media.Colors.White);
            resultLabel = UIElementFactory.CreateLabel(new Transform(new Vector2(150, 75), new Vector2(MainWindow.referenceSize.X / 2, 150)), resultLabelText, System.Windows.Media.Colors.White);

            ipInput = UIElementFactory.CreateTextBox(new Transform(new Vector2(125, 50), new Vector2(MainWindow.referenceSize.X / 2, 50)), "127.0.0.1", DelegatesActions.EmptyAction);
            portInput = UIElementFactory.CreateTextBox(new Transform(new Vector2(125, 50), new Vector2(MainWindow.referenceSize.X / 2, 100)), "7778", DelegatesActions.EmptyAction);

            connectButton = UIElementFactory.CreateButton(new Transform(new Vector2(50, 50), new Vector2(MainWindow.referenceSize.X / 2 + 26, 225)), OnConnect, System.Windows.Media.Color.FromRgb(0, 255, 0), "Connect");
            backButton = UIElementFactory.CreateButton(new Transform(new Vector2(50, 50), new Vector2(MainWindow.referenceSize.X / 2 - 26, 225)), OnBack, System.Windows.Media.Color.FromRgb(0, 255, 0), "Back");
        }

        /// <summary>
        /// On "connect" button click
        /// </summary>
        private void OnConnect()
        {
            if (ipInput.Text == "" && portInput.Text == "")
                return;

            Dispose();
            new NetworkedGameClient(ipInput.Text, int.Parse(portInput.Text));
        }

        /// <summary>
        /// On "back" button click
        /// </summary>
        private void OnBack()
        {
            Dispose();
            new GameMainMenu();
        }

        /// <summary>
        /// Disposes the current online menu
        /// </summary>
        private void Dispose()
        {
            ipLabel.Dispose();
            portLabel.Dispose();
            resultLabel.Dispose();
            ipInput.Dispose();
            portInput.Dispose();
            connectButton.Dispose();
            backButton.Dispose();
        }
    }
}