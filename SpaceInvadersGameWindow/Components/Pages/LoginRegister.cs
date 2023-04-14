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
        private bool loginRegisterFlip = false;
        private CustomTextInput usernameInput;
        private CustomTextInput passwordInput;
        private CustomTextInput emailInput;
        private CustomTextInput register2FAInput;
        private CustomButton loginRegistFlipper;
        private CustomButton loginRegistButton;
        private CustomButton register2FAButton;
        private CustomLabel resultLabel;

        /// <summary>
        /// Builds the Login Register page
        /// </summary>
        public LoginRegister()
        {
            Application.Current.Dispatcher.Invoke(() =>
            { // UI Objects need to be created in an STA thread
                usernameInput = new CustomTextInput(new Transform(new Vector2(50, 12.5f), new Vector2(MainWindow.referenceSize.X / 2 - 25, MainWindow.referenceSize.Y / 2 - 37.5f)));
                passwordInput = new CustomTextInput(new Transform(new Vector2(50, 12.5f), new Vector2(MainWindow.referenceSize.X / 2 + 25, MainWindow.referenceSize.Y / 2 - 37.5f)));
                emailInput = new CustomTextInput(new Transform(new Vector2(100, 12.5f), new Vector2(MainWindow.referenceSize.X / 2, MainWindow.referenceSize.Y / 2 - 25)));
                register2FAInput = new CustomTextInput(new Transform(new Vector2(50, 50), new Vector2(MainWindow.referenceSize.X / 2 - 25, MainWindow.referenceSize.Y / 2 - 7.5f)));
                loginRegistFlipper = new CustomButton(new Transform(new Vector2(50, 50), new Vector2(MainWindow.referenceSize.X / 2 - 25, MainWindow.referenceSize.Y / 2 - 7.5f)), FlipLoginRegist, @"Resources\Images\Pixels\Green.png", "Register?"); ;
                loginRegistButton = new CustomButton(new Transform(new Vector2(50, 50), new Vector2(MainWindow.referenceSize.X / 2 + 25, MainWindow.referenceSize.Y / 2 - 7.5f)), LoginRegisterClick, @"Resources\Images\Pixels\Green.png", "Click To Login");
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

        /// <summary>
        /// Ping pongs between the states of "Login" and "Register"
        /// </summary>
        private void FlipLoginRegist()
        {
            loginRegisterFlip = !loginRegisterFlip;
            if (loginRegisterFlip)
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

        /// <summary>
        /// LoginRegister button click
        /// </summary>
        private void LoginRegisterClick()
        {
            string username = usernameInput.box.Text.ToLower();
            string password = passwordInput.box.Text.ToLower();
            string email = emailInput.box.Text.ToLower();

            if (loginRegisterFlip)
                AddToSubmitCode(new RegisterValidator(username, password, email, On2FANeeded, resultLabel));
            else
                new LoginValidator(username, password, resultLabel, Dispose);
        }

        #region 2FA handling
        RoutedEventHandler? currentSubmit = null;

        /// <summary>
        /// Adds the current <see cref="RegisterValidator"/> to the 2FA submit button
        /// </summary>
        /// <param name="validator"> The current <see cref="RegisterValidator"/> connection handler </param>
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
                FlipLoginRegist();
            };

            register2FAButton.button.Click += currentSubmit;
        }

        /// <summary>
        /// Called when 2FA needed and appropriately prepares the page
        /// </summary>
        private void On2FANeeded()
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

        /// <summary>
        /// Disposes the current <see cref="LoginRegister"/> page
        /// </summary>
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