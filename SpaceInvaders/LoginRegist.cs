using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Sockets;
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

            TcpClient client = new TcpClient();
            client.Connect("localhost", 7777);
            Byte[] buffer = Encoding.UTF8.GetBytes("YonatanZiv/ziv");
            client.GetStream().Write(buffer,0,buffer.Length);
            loginButton = new System.Windows.Forms.Button();
            usernameInput = new System.Windows.Forms.TextBox();
            passwordInput = new System.Windows.Forms.TextBox();

            loginButton.Location = new System.Drawing.Point(22, 71);
            loginButton.Name = "loginButton";
            loginButton.Size = new System.Drawing.Size(75, 23);
            loginButton.TabIndex = 2;
            loginButton.Text = "Login";
            loginButton.UseVisualStyleBackColor = true;

            usernameInput.Location = new System.Drawing.Point(12, 12);
            usernameInput.Name = "usernameInput";
            usernameInput.PlaceholderText = "Username";
            usernameInput.Size = new System.Drawing.Size(100, 23);
            usernameInput.TabIndex = 3;

            passwordInput.Location = new System.Drawing.Point(12, 42);
            passwordInput.Name = "passwordInput";
            passwordInput.PlaceholderText = "Password";
            passwordInput.Size = new System.Drawing.Size(100, 23);
            passwordInput.TabIndex = 4;
            passwordInput.UseSystemPasswordChar = true;

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
