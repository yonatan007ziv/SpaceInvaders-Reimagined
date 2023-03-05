using GameWindow.Components.UIElements;

namespace GameWindow.Systems.Networking
{
    class RegistValidator : NetworkClient
    {
        private CustomLabel resultLabel;
        public RegistValidator(string username, string password, CustomLabel resultLabel) : base()
        {
            this.resultLabel = resultLabel;
            if (ConnectToAddress("46.121.140.104", 7777))
            {
                SendMessage($"REGISTER:{username}/{password}");
                BeginSingleRead();
            }
            else
                resultLabel.Text = "Failed! server unreachable.";
        }

        protected override void DecodeMessage(string msg)
        {
            resultLabel.Text = msg;
            if (msg == "SUCCESS")
            {
                resultLabel.Text = "Successfully registered!";
            }
            else if (msg == "USERNAME EXISTS")
            {
                resultLabel.Text = "Username already exists, maybe try to login?";
            }
            else if (msg == "INVALID PASSWORD")
            {
                resultLabel.Text = "Invalid password entered.";
            }
            else if (msg == "FAILED")
            {
                resultLabel.Text = "Failed! please try again.";
            }
            StopClient();
        }
    }
}