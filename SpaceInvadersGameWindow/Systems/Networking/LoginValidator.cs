using SpaceInvadersGameWindow.Components.UIElements;
using System;
using System.Diagnostics;

namespace SpaceInvadersGameWindow.Systems.Networking
{
    class LoginValidator : NetworkClient
    {
        Action InitializeMenu;
        private CustomLabel resultLabel;

        public LoginValidator(string username, string password, CustomLabel resultLabel, Action InitializeMenu) : base()
        {
            this.resultLabel = resultLabel;
            this.InitializeMenu = InitializeMenu;
            SendMessage($"LOGIN:{username}/{password}");
            BeginSingleRead();
        }

        protected override void DecodeMessage(string msg)
        {
            if (msg == "SUCCESS")
            {
                InitializeMenu();
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