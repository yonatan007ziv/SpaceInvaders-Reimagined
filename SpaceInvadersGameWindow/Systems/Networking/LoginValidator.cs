using System.Diagnostics;
using System.Windows;
using System.Windows.Controls.Primitives;

namespace SpaceInvadersGameWindow.Systems.Networking
{
    class LoginValidator : NetworkClient
    {
        public LoginValidator(string username, string password) : base()
        {
            SendMessage($"LOGIN:{username}/{password}");
            BeginRead();
        }

        protected override void DecodeMessage(string msg)
        {
            Debug.WriteLine(msg);
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
        }
    }
}