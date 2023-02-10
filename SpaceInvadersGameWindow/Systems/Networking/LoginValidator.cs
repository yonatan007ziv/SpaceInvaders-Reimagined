using GameWindow.Components.Initializers;
using GameWindow.Components.UIElements;
using System;
using System.Diagnostics;

namespace GameWindow.Systems.Networking
{
    class LoginValidator : NetworkClient
    {
        Action DiposeRegistLoginMenu;
        private CustomLabel resultLabel;

        private string username;

        public LoginValidator(string username, string password, CustomLabel resultLabel, Action InitializeMenu) : base()
        {
            this.username = username;

            this.resultLabel = resultLabel;
            this.DiposeRegistLoginMenu = InitializeMenu;
            if (ConnectToAddress("127.0.0.1", 7777))
            {
                SendMessage($"LOGIN:{username}/{password}");
                BeginSingleRead();
            }
            else
                resultLabel.Text = "Failed! server unreachable.";
        }

        protected override void DecodeMessage(string msg)
        {
            if (msg == "SUCCESS")
            {
                resultLabel.Text = "Successfully logged in!";
                DiposeRegistLoginMenu();
                GameInitializers.StartGameMenu(username);
            }
            else if (msg == "NO SUCH USERNAME")
            {
                resultLabel.Text = "No such username exists, maybe try to register?";
            }
            else if (msg == "WRONG PASSWORD")
            {
                resultLabel.Text = "Wrong password entered.";
            }
            else if (msg == "FAILED")
            {
                resultLabel.Text = "Failed! please try again.";
            }
            StopClient();
        }
    }
}