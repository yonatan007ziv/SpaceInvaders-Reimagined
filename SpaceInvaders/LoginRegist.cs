using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SpaceInvaders
{
    public class LoginRegist
    {
        private Button loginButton;
        private TextBox usernameInput;
        private TextBox passwordInput;
        public LoginRegist()
        {
            this.loginButton = new System.Windows.Forms.Button();
            this.usernameInput = new System.Windows.Forms.TextBox();
            this.passwordInput = new System.Windows.Forms.TextBox();

            this.loginButton.Location = new System.Drawing.Point(22, 71);
            this.loginButton.Name = "loginButton";
            this.loginButton.Size = new System.Drawing.Size(75, 23);
            this.loginButton.TabIndex = 2;
            this.loginButton.Text = "Login";
            this.loginButton.UseVisualStyleBackColor = true;

            this.usernameInput.Location = new System.Drawing.Point(12, 12);
            this.usernameInput.Name = "usernameInput";
            this.usernameInput.PlaceholderText = "Username";
            this.usernameInput.Size = new System.Drawing.Size(100, 23);
            this.usernameInput.TabIndex = 3;

            this.passwordInput.Location = new System.Drawing.Point(12, 42);
            this.passwordInput.Name = "passwordInput";
            this.passwordInput.PlaceholderText = "Password";
            this.passwordInput.Size = new System.Drawing.Size(100, 23);
            this.passwordInput.TabIndex = 4;
            this.passwordInput.UseSystemPasswordChar = true;

            GameWindow.Instance!.Controls.Add(this.passwordInput);
            GameWindow.Instance.Controls.Add(this.usernameInput);
            GameWindow.Instance.Controls.Add(this.loginButton);
        }
        public void Dispose()
        {
            GameWindow.Instance!.Controls.Remove(this.passwordInput);
            GameWindow.Instance.Controls.Remove(this.usernameInput);
            GameWindow.Instance.Controls.Remove(this.loginButton);
        }
    }
}
