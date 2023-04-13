using GameWindow.Components.Miscellaneous;
using GameWindow.Components.UIElements;
using GameWindow.Systems.Networking;
using System.Numerics;
using System.Windows;

namespace GameWindow.Components.Pages
{
    /// <summary>
    /// A page representing the "Login Register" menu
    /// </summary>
    internal class LoginRegister
    {
        private bool loginRegistFlip = false;
        private CustomTextInput usernameInput;
        private CustomTextInput passwordInput;
        private CustomTextInput emailInput;
        private CustomTextInput register2FAInput;
        private CustomButton loginRegistFlipper;
        private CustomButton loginRegistButton;
        private CustomButton register2FAButton;
        private CustomLabel resultLabel;

        public LoginRegister()
        {
            Application.Current.Dispatcher.Invoke(() =>
            { // UI Objects need to be created in an STA thread
                usernameInput = new CustomTextInput(new Transform(new Vector2(50, 12.5f), new Vector2(MainWindow.referenceSize.X / 2 - 25, MainWindow.referenceSize.Y / 2 - 37.5f)));
                passwordInput = new CustomTextInput(new Transform(new Vector2(50, 12.5f), new Vector2(MainWindow.referenceSize.X / 2 + 25, MainWindow.referenceSize.Y / 2 - 37.5f)));
                emailInput = new CustomTextInput(new Transform(new Vector2(100, 12.5f), new Vector2(MainWindow.referenceSize.X / 2, MainWindow.referenceSize.Y / 2 - 25)));
                register2FAInput = new CustomTextInput(new Transform(new Vector2(50, 50), new Vector2(MainWindow.referenceSize.X / 2 - 25, MainWindow.referenceSize.Y / 2 - 7.5f)));
                loginRegistFlipper = new CustomButton(new Transform(new Vector2(50, 50), new Vector2(MainWindow.referenceSize.X / 2 - 25, MainWindow.referenceSize.Y / 2 - 7.5f)), flipLoginRegist, @"Resources\Images\Pixels\Green.png", "Register?"); ;
                loginRegistButton = new CustomButton(new Transform(new Vector2(50, 50), new Vector2(MainWindow.referenceSize.X / 2 + 25, MainWindow.referenceSize.Y / 2 - 7.5f)), loginRegist, @"Resources\Images\Pixels\Green.png", "Click To Login");
                register2FAButton = new CustomButton(new Transform(new Vector2(50, 50), new Vector2(MainWindow.referenceSize.X / 2 + 25, MainWindow.referenceSize.Y / 2 - 7.5f)), () => { }, @"Resources\Images\Pixels\Green.png", "Submit 2FA code");
                resultLabel = new CustomLabel(new Transform(new Vector2(100, 50), new Vector2(MainWindow.referenceSize.X / 2, MainWindow.referenceSize.Y / 2 - 62.5f)), "", System.Windows.Media.Colors.White);
            });

            // Suppressing the "Null When Leaving a Constructor" warning
            usernameInput!.box.AcceptsReturn = false;
            usernameInput.box.AcceptsTab = false;

            passwordInput!.box.AcceptsReturn = false;
            passwordInput.box.AcceptsTab = false;

            emailInput!.box.AcceptsReturn = false;
            emailInput.box.AcceptsTab = false;
            emailInput.Visible(false);

            register2FAInput!.Visible(false);
            register2FAButton!.Visible(false);

            loginRegistFlipper!.ToString();
            loginRegistButton!.ToString();
            resultLabel!.ToString();
        }

        private void flipLoginRegist()
        {
            loginRegistFlip = !loginRegistFlip;
            if (loginRegistFlip)
            {
                loginRegistFlipper.Text = "Login?";
                loginRegistButton.Text = "Click To Register";
                loginRegistFlipper.transform.Position += new Vector2(0, 12.5f);
                loginRegistButton.transform.Position += new Vector2(0, 12.5f);
                emailInput.Visible(true);
            }
            else
            {
                loginRegistFlipper.Text = "Register?";
                loginRegistButton.Text = "Click To Login";
                loginRegistFlipper.transform.Position -= new Vector2(0, 12.5f);
                loginRegistButton.transform.Position -= new Vector2(0, 12.5f);
                emailInput.Visible(false);
            }
        }

        private void loginRegist()
        {
            string username = usernameInput.box.Text.ToLower();
            string password = passwordInput.box.Text.ToLower();
            string email = emailInput.box.Text.ToLower();

            if (loginRegistFlip)
                AddToSubmitCode(new RegisterValidator(username, password, email, On2FA, resultLabel));
            else
                new LoginValidator(username, password, resultLabel, Dispose);
        }

        #region 2FA handling
        RoutedEventHandler? currentSubmit = null;
        private void AddToSubmitCode(RegisterValidator validator)
        {
            if (currentSubmit != null)
                register2FAButton.button.Click -= currentSubmit;

            currentSubmit = (s, e) =>
            {
                validator.Send2FACode(register2FAInput.Text);
                register2FAInput.Text = "";
                register2FAInput.Visible(false);
                register2FAButton.Visible(false);

                usernameInput.Visible(true);
                passwordInput.Visible(true);
                emailInput.Visible(true);
                loginRegistFlipper.Visible(true);
                loginRegistButton.Visible(true);
            };

            register2FAButton.button.Click += currentSubmit;
        }
        private void On2FA(RegisterValidator validator)
        {
            usernameInput.Visible(false);
            passwordInput.Visible(false);
            emailInput.Visible(false);
            loginRegistFlipper.Visible(false);
            loginRegistButton.Visible(false);

            register2FAInput.Visible(true);
            register2FAButton.Visible(true);
        }
        #endregion

        private void Dispose()
        {
            usernameInput.Dispose();
            passwordInput.Dispose();
            emailInput.Dispose();
            register2FAInput.Dispose();
            loginRegistFlipper.Dispose();
            loginRegistButton.Dispose();
            register2FAButton.Dispose();
            resultLabel.Dispose();
        }
    }
}