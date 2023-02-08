using SpaceInvaders.Components.Miscellaneous;
using SpaceInvadersGameWindow.Components.Initializers;
using SpaceInvadersGameWindow.Components.UIElements;
using SpaceInvadersGameWindow.Systems.Networking;
using System;
using System.Diagnostics;
using System.Numerics;

namespace SpaceInvadersGameWindow.Components.Pages
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
            usernameInput = new CustomTextInput(new Transform(new Vector2(50, 25), new Vector2(25, 50)));
            passwordInput = new CustomTextInput(new Transform(new Vector2(50, 25), new Vector2(75, 50)));

            LoginRegistFlipper = new CustomButton(new Transform(new Vector2(50, 50), new Vector2(25, 100)), () => loginRegistFlip = !loginRegistFlip, @"Resources\RawFiles\Images\UI\temp.png");
            LoginRegistButton = new CustomButton(new Transform(new Vector2(50, 50), new Vector2(75, 100)), loginRegist, @"Resources\RawFiles\Images\UI\temp.png");

            resultLabel = new CustomLabel(new Transform(new Vector2(100, 50), new Vector2(75, 150)), "", System.Windows.Media.Colors.White);
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