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
        private CustomLabel usernameLabel;
        private CustomLabel passwordLabel;
        private CustomLabel emailLabel;
        private CustomTextBox usernameInput;
        private CustomTextBox passwordInput;
        private CustomTextBox emailInput;
        private CustomTextBox twoFAInput;
        private CustomButton flipLoginRegisterButton;
        private CustomButton loginRegistButton;
        private CustomButton submit2FAButton;
        private CustomLabel resultLabel;

        /// <summary>
        /// Builds the Login Register page
        /// </summary>
        public LoginRegister()
        {
            Application.Current.Dispatcher.Invoke(() =>
            { // UI Objects need to be created in an STA thread
                usernameLabel = new CustomLabel(new Transform(new Vector2(50, 12.5f), new Vector2(MainWindow.referenceSize.X / 2 - 25.5f, MainWindow.referenceSize.Y / 2 - 50)), "Username:", System.Windows.Media.Colors.White);
                passwordLabel = new CustomLabel(new Transform(new Vector2(50, 12.5f), new Vector2(MainWindow.referenceSize.X / 2 + 25.5f, MainWindow.referenceSize.Y / 2 - 50)), "Password:", System.Windows.Media.Colors.White);
                emailLabel = new CustomLabel(new Transform(new Vector2(50, 12.5f), new Vector2(MainWindow.referenceSize.X / 2 - 76, MainWindow.referenceSize.Y / 2 - 26)), "Email:", System.Windows.Media.Colors.White);
                resultLabel = new CustomLabel(new Transform(new Vector2(100, 50), new Vector2(MainWindow.referenceSize.X / 2, MainWindow.referenceSize.Y / 2 - 62.5f)), "", System.Windows.Media.Colors.White);

                usernameInput = new CustomTextBox(new Transform(new Vector2(51, 12.5f), new Vector2(MainWindow.referenceSize.X / 2 - 25.5f, MainWindow.referenceSize.Y / 2 - 38.5f)), "", DelegatesActions.EmptyAction);
                passwordInput = new CustomTextBox(new Transform(new Vector2(51, 12.5f), new Vector2(MainWindow.referenceSize.X / 2 + 25.5f, MainWindow.referenceSize.Y / 2 - 38.5f)), "", DelegatesActions.EmptyAction);
                emailInput = new CustomTextBox(new Transform(new Vector2(102, 12.5f), new Vector2(MainWindow.referenceSize.X / 2, MainWindow.referenceSize.Y / 2 - 26)), "", DelegatesActions.EmptyAction);
                twoFAInput = new CustomTextBox(new Transform(new Vector2(50, 50), new Vector2(MainWindow.referenceSize.X / 2 - 25, MainWindow.referenceSize.Y / 2 - 7.5f)), "", DelegatesActions.EmptyAction);

                flipLoginRegisterButton = new CustomButton(new Transform(new Vector2(50, 50), new Vector2(MainWindow.referenceSize.X / 2 - 26, MainWindow.referenceSize.Y / 2 - 7.5f)), FlipLoginRegist, Image.Green, "Register?"); ;
                loginRegistButton = new CustomButton(new Transform(new Vector2(50, 50), new Vector2(MainWindow.referenceSize.X / 2 + 26, MainWindow.referenceSize.Y / 2 - 7.5f)), LoginRegisterClick, Image.Green, "Click To Login");
                submit2FAButton = new CustomButton(new Transform(new Vector2(50, 50), new Vector2(MainWindow.referenceSize.X / 2 + 26, MainWindow.referenceSize.Y / 2 - 7.5f)), () => { }, Image.Green, "Submit 2FA code");
            });

            // Suppressing the "Null When Leaving a Constructor" warning
            usernameInput!.AcceptsReturn = false;
            usernameInput.AcceptsTab = false;

            passwordInput!.AcceptsReturn = false;
            passwordInput.AcceptsTab = false;
            passwordInput.Censor = true;

            emailInput!.AcceptsReturn = false;
            emailInput.AcceptsTab = false;
            emailInput.Visible(false);

            emailLabel!.Visible(false);
            twoFAInput!.Visible(false);
            submit2FAButton!.Visible(false);

            usernameLabel!.ToString();
            passwordLabel!.ToString();
            flipLoginRegisterButton!.ToString();
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
                flipLoginRegisterButton.Text = "Login?";
                loginRegistButton.Text = "Click To Register";
                flipLoginRegisterButton.transform.Position += new Vector2(0, 12.5f);
                loginRegistButton.transform.Position += new Vector2(0, 12.5f);
                emailInput.Visible(true);
                emailLabel!.Visible(true);
            }
            else
            {
                flipLoginRegisterButton.Text = "Register?";
                loginRegistButton.Text = "Click To Login";
                flipLoginRegisterButton.transform.Position -= new Vector2(0, 12.5f);
                loginRegistButton.transform.Position -= new Vector2(0, 12.5f);
                emailInput.Visible(false);
                emailLabel!.Visible(false);
            }
        }

        /// <summary>
        /// LoginRegister button click
        /// </summary>
        private void LoginRegisterClick()
        {
            string username = usernameInput.Text.ToLower();
            string password = passwordInput.Text.ToLower();
            string email = emailInput.Text.ToLower();

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
                submit2FAButton.Click -= currentSubmit;

            currentSubmit = (s, e) =>
            {
                validator.Send2FACode(twoFAInput.Text);
                twoFAInput.Text = "";
                twoFAInput.Visible(false);
                submit2FAButton.Visible(false);

                usernameInput.Visible(true);
                passwordInput.Visible(true);
                emailInput.Visible(true);
                flipLoginRegisterButton.Visible(true);
                loginRegistButton.Visible(true);
                FlipLoginRegist();
            };

            submit2FAButton.Click += currentSubmit;
        }

        /// <summary>
        /// Called when 2FA needed and appropriately prepares the page
        /// </summary>
        private void On2FANeeded()
        {
            resultLabel.Text = "Please check your email inbox.";

            usernameLabel.Visible(false);
            passwordLabel.Visible(false);
            emailLabel.Visible(false);

            usernameInput.Visible(false);
            passwordInput.Visible(false);
            emailInput.Visible(false);
            twoFAInput.Visible(true);

            flipLoginRegisterButton.Visible(false);
            loginRegistButton.Visible(false);
            submit2FAButton.Visible(true);
        }
        #endregion

        /// <summary>
        /// Disposes the current <see cref="LoginRegister"/> page
        /// </summary>
        private void Dispose()
        {
            usernameLabel.Dispose();
            passwordLabel.Dispose();
            emailLabel.Dispose();
            usernameInput.Dispose();
            passwordInput.Dispose();
            emailInput.Dispose();
            twoFAInput.Dispose();
            flipLoginRegisterButton.Dispose();
            loginRegistButton.Dispose();
            submit2FAButton.Dispose();
            resultLabel.Dispose();
        }
    }
}