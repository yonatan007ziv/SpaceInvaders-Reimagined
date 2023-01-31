using SpaceInvadersGameWindow.Components.UIElements;
using SpaceInvadersGameWindow.Systems.Networking;
using System.Numerics;
using System.Windows.Controls;

namespace SpaceInvadersGameWindow.Components.UIForms
{
    public partial class LoginRegistPage : Page
    {
        private bool loginRegistFlip;
        private TextBox usernameInput;
        private TextBox passwordInput;
        public LoginRegistPage()
        {
            InitializeComponent();
            usernameInput = new TextBox();
            passwordInput = new TextBox();

            //MainWindow.instance!.

            NavigableButton LoginRegistFlipper = new NavigableButton(new Vector2(50, 50), new Vector2(25, 125), () => loginRegistFlip = !loginRegistFlip, false);
            NavigableButton LoginRegistButton = new NavigableButton(new Vector2(50, 50), new Vector2(100, 100), () => loginRegist(), true);
        }
        private void loginRegist()
        {
            if (loginRegistFlip)
                new RegistValidator(usernameInput.Text, passwordInput.Text);
            else
                new LoginValidator(usernameInput.Text, passwordInput.Text);
        }
    }
}
