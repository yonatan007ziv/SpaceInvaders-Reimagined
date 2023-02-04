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
            resultLabel.SetText(msg);
            if (msg == "SUCCESS")
            {
                InitializeMenu();
            }
            else if (msg == "NO SUCH USERNAME")
            {
            }
            else if (msg == "WRONG PASSWORD")
            {
            }
            else if (msg == "FAILED")
            {
            }
            StopClient();
        }
    }
}