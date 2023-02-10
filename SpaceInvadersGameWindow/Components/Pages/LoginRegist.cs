using GameWindow.Components.Miscellaneous;
using GameWindow.Components.Initializers;
using GameWindow.Components.UIElements;
using GameWindow.Systems.Networking;
using System;
using System.Diagnostics;
using System.Numerics;

namespace GameWindow.Components.Pages
{
    internal class LoginRegist
    {
        private bool loginRegistFlip = false;
        private CustomTextInput usernameInput;
        private CustomTextInput passwordInput;
        private CustomButton LoginRegistFlipper;
        private CustomButton LoginRegistButton;
        private CustomLabel resultLabel;

        public LoginRegist()
        {
            usernameInput = new CustomTextInput(new Transform(new Vector2(50, 25), new Vector2(256 / 2 - 25, 256 / 2 - 50)));
            passwordInput = new CustomTextInput(new Transform(new Vector2(50, 25), new Vector2(256 / 2 + 25, 256 / 2 - 50)));

            LoginRegistFlipper = new CustomButton(new Transform(new Vector2(50, 50), new Vector2(256 / 2 - 25, 256 / 2)), flipLoginRegist, @"Resources\Images\Pixels\Green.png", "Register?"); ;
            LoginRegistButton = new CustomButton(new Transform(new Vector2(50, 50), new Vector2(256 / 2 + 25, 256 / 2)), loginRegist, @"Resources\Images\Pixels\Green.png", "Click To Login");

            resultLabel = new CustomLabel(new Transform(new Vector2(100, 50), new Vector2(256 / 2, 256 / 2 + 50)), "", System.Windows.Media.Colors.White);
        }
        private void flipLoginRegist()
        {
            loginRegistFlip = !loginRegistFlip;
            LoginRegistFlipper.Text = loginRegistFlip ? "Login?" : "Register?";
            LoginRegistButton.Text = loginRegistFlip ? "Click To Register" : "Click To Login";
        }
        private void loginRegist()
        {
            if (loginRegistFlip)
                new RegistValidator(usernameInput.box.Text, passwordInput.box.Text, resultLabel);
            else
                new LoginValidator(usernameInput.box.Text, passwordInput.box.Text, resultLabel, Dispose);
        }
        private void Dispose()
        {
            usernameInput.Dispose();
            passwordInput.Dispose();
            LoginRegistFlipper.Dispose();
            LoginRegistButton.Dispose();
            resultLabel.Dispose();
        }
    }
}