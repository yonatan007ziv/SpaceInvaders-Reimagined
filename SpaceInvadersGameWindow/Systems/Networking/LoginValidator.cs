using GameWindow.Components.Pages;
using GameWindow.Components.UIElements;
using System;

namespace GameWindow.Systems.Networking
{
    /// <summary>
    /// Connects to remote Login-Server and gets the appropriate response
    /// </summary>
    class LoginValidator : NetworkClient
    {
        private string username;
        private CustomLabel resultLabel;
        private Action DiposeLoginRegisterMenu;

        /// <summary>
        /// Sends Login Data to the remote Login-Server and reads a response
        /// </summary>
        /// <param name="username"> The inputted username </param>
        /// <param name="password"> The inputted password </param>
        /// <param name="resultLabel"> The result label </param>
        /// <param name="DiposeLoginRegisterMenu"> Action to dispose <see cref="LoginRegister"/> menu </param>
        public LoginValidator(string username, string password, CustomLabel resultLabel, Action DiposeLoginRegisterMenu) : base()
        {
            this.username = username;

            this.resultLabel = resultLabel;
            this.DiposeLoginRegisterMenu = DiposeLoginRegisterMenu;

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

            if (Connect(IPConstants.LOGIN_SERVER_IP, IPConstants.LOGIN_SERVER_PORT))
            {
                SendMessage($"Login:{username}/{password}");
                BeginRead(loop: false);
            }
            else
                resultLabel.Text = "Failed! server unreachable.";
        }

        /// <summary>
        /// Interprets Login-Response from server
        /// </summary>
        /// <param name="msg"> Response from server </param>
        protected override void InterpretMessage(string msg)
        {
            if (msg == "Success")
            {
                resultLabel.Text = "Successfully logged in!";
                DiposeLoginRegisterMenu();

                MainWindow.username = username;
                new GameMainMenu();
            }
            else if (msg == "NoSuchUsername")
                resultLabel.Text = "No such username exists, maybe try to register?";
            else if (msg == "WrongPassword")
                resultLabel.Text = "Wrong password entered.";
            else if (msg == "AlreadyConnected")
                resultLabel.Text = "Already connected to the game.";
            else if (msg == "Failed")
                resultLabel.Text = "Failed Login! please try again.";
            StopClient();
        }

        protected override void OnError()
        {
            resultLabel.Text = "Failed Login! please try again.";
        }
    }
}