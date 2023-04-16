using GameWindow.Components.UIElements;
using System;

namespace GameWindow.Systems.Networking
{
    /// <summary>
    /// Connects to remote Register-Server and gets the appropriate response
    /// </summary>
    public class RegisterValidator : NetworkClient
    {
        private const string REGISTER_SERVER_IP = "46.121.140.69";
        private const int REGISTER_SERVER_PORT = 7777;

        private CustomLabel resultLabel;
        private Action On2FA;

        /// <summary>
        /// Sends Register Data to the remote Register-Server and reads a response
        /// </summary>
        /// <param name="username"> The inputted username </param>
        /// <param name="password"> The inputted password </param>
        /// <param name="email"> The inputted email </param>
        /// <param name="On2FA"> What happens on 2FA needed </param>
        /// <param name="resultLabel"> The result label </param>
        public RegisterValidator(string username, string password, string email, Action On2FA, CustomLabel resultLabel) : base()
        {
            this.resultLabel = resultLabel;
            this.On2FA = On2FA;

            if (username == "")
            {
                resultLabel.Text = "Invalid Username entered.";
                return;
            }
            else if (password == "")
            {
                resultLabel.Text = "Invalid Password entered.";
                return;
            }
            else if (email == "")
            {
                resultLabel.Text = "Invalid Email entered.";
                return;
            }

            if (Connect(REGISTER_SERVER_IP, REGISTER_SERVER_PORT))
            {
                SendMessage($"Register:{username}/{password}/{email}");
                BeginRead(false);
            }
            else
                resultLabel.Text = "Failed! server unreachable.";
        }

        /// <summary>
        /// Sends the 2FA code to the Register-Server
        /// </summary>
        /// <param name="code"> The code </param>
        public void Send2FACode(string code)
        {
            SendMessage($"2FACode:{code}");
            BeginRead(false);
        }

        /// <summary>
        /// Interprets Register-Response from server
        /// </summary>
        /// <param name="msg"> Response from server </param>
        protected override void InterpretMessage(string msg)
        {
            resultLabel.Text = msg;
            if (msg == "Success")
                resultLabel.Text = "Successfully registered!";
            else if (msg == "UsernameExists")
                resultLabel.Text = "Username already exists, maybe try to login?";
            else if (msg == "EmailExists")
                resultLabel.Text = "Email already exists, maybe try to login?";
            else if (msg == "InvalidUsername")
                resultLabel.Text = "Invalid Username entered.";
            else if (msg == "InvalidPassword")
                resultLabel.Text = "Invalid Password entered.";
            else if (msg == "InvalidEmail")
                resultLabel.Text = "Invalid Email entered.";
            else if (msg == "Failed")
                resultLabel.Text = "Failed Register! please try again.";
            else if (msg == "Wrong2FA")
                resultLabel.Text = "Wrong code entered! please try to register again.";
            else if (msg == "Need2FA")
            {
                On2FA();
                return; // Prevents disconnection from server
            }
            StopClient();
        }
    }
}