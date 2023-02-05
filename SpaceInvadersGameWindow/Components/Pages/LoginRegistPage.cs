using SpaceInvadersGameWindow.Components.UIElements;
using SpaceInvadersGameWindow.Systems.Networking;
using System;
using System.Diagnostics;
using System.Numerics;
using System.Windows.Media;

namespace SpaceInvadersGameWindow.Components.Pages
{
    internal class LoginRegistPage
    {
        private bool loginRegistFlip = false;
        private CustomTextInput usernameInput;
        private CustomTextInput passwordInput;
        private CustomButton LoginRegistFlipper;
        private CustomButton LoginRegistButton;
        private CustomLabel resultLabel;
        public LoginRegistPage()
        {
            usernameInput = new CustomTextInput(new Vector2(50, 25), new Vector2(25, 50));
            passwordInput = new CustomTextInput(new Vector2(50, 25), new Vector2(75, 50));

            LoginRegistFlipper = new CustomButton(new Vector2(50, 50), new Vector2(25, 100), () => loginRegistFlip = !loginRegistFlip, @"Resources\RawFiles\Images\UI\temp.png", false);
            LoginRegistButton = new CustomButton(new Vector2(50, 50), new Vector2(75, 100), loginRegist, @"Resources\RawFiles\Images\UI\temp.png", false);

            resultLabel = new CustomLabel(new Vector2(100, 50), new Vector2(75, 150), Colors.White);
        }
        private void loginRegist()
        {
            Debug.WriteLine(usernameInput.box.Text + " " + passwordInput.box.Text);
            if (loginRegistFlip)
                new RegistValidator(usernameInput.box.Text, passwordInput.box.Text, resultLabel);
            else
                new LoginValidator(usernameInput.box.Text, passwordInput.box.Text, resultLabel, () => { Dispose(); GameInitializer.instance!.StartGameMenu(); });
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