using System.Diagnostics;
using System.Windows.Controls.Primitives;

namespace SpaceInvadersGameWindow.Systems.Networking
{
    class RegistValidator : NetworkClient
    {
        public RegistValidator(string username, string password) : base()
        {
            SendMessage($"REGISTER:{username}/{password}");
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