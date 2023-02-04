using SpaceInvadersGameWindow.Components.UIElements;
using System.Diagnostics;

namespace SpaceInvadersGameWindow.Systems.Networking
{
    class RegistValidator : NetworkClient
    {
        private CustomLabel resultLabel;
        public RegistValidator(string username, string password, CustomLabel resultLabel) : base()
        {
            this.resultLabel = resultLabel;
            SendMessage($"REGISTER:{username}/{password}");
            BeginSingleRead();
        }

        protected override void DecodeMessage(string msg)
        {
            resultLabel.SetText(msg);
            if (msg == "SUCCESS")
            {

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