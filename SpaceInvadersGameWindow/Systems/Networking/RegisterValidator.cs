using GameWindow.Components.UIElements;
using static GameWindow.Components.Miscellaneous.Delegates;

namespace GameWindow.Systems.Networking
{
    /// <summary>
    /// Connects to remote Register-Server and gets the appropriate response
    /// </summary>
    public class RegisterValidator : NetworkClient
    {
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
            if (Connect("46.121.140.69", 7777))
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
                resultLabel.Text = "Invalid password entered.";
            else if (msg == "InvalidEmail")
                resultLabel.Text = "Invalid email entered.";
            else if (msg == "Failed")
                resultLabel.Text = "Failed Register! please try again.";
            else if (msg == "Wrong2FA")
                resultLabel.Text = "Wrong code entered! please try to register again.";
            else if (msg == "Need2FA")
            {
                On2FA();
                resultLabel.Text = "Please check your email inbox.";
                return; // Prevents disconnection from server
            }
            StopClient();
        }
    }
}