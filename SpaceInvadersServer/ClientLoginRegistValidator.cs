using System.Net.Sockets;
using System.Text;

namespace SpaceInvadersServer
{
    internal class ClientLoginRegistValidator
    {
        private enum LoginResult
        {
            Success,
            NoSuchUsername,
            WrongPassword,
            failed
        }
        private enum RegisterResult
        {
            Success,
            UsernameExists,
            InvalidPassword,
            failed
        }

        Byte[] buffer;
        TcpClient client;
        DatabaseHandler dbHandler;
        public ClientLoginRegistValidator(TcpClient client, DatabaseHandler dbHandler)
        {
            Console.WriteLine("New client accepted.");

            this.client = client;
            this.dbHandler = dbHandler;
            buffer = new Byte[client.ReceiveBufferSize];

            client.GetStream().BeginRead(buffer, 0, buffer.Length, ReceiveMessage, null);
        }

        #region Login Logic
        private void Login(string loginData)
        {
            string username = loginData.Split('/')[0];
            string password = loginData.Split('/')[1];
            LoginResult loginResult = ValidateLogin(username, password);

            switch (loginResult)
            {
                case LoginResult.Success:
                    SendMessage("SUCCESS");
                    return;
                case LoginResult.NoSuchUsername:
                    SendMessage("NO SUCH USERNAME");
                    return;
                case LoginResult.WrongPassword:
                    SendMessage("WRONG PASSWORD");
                    return;
                case LoginResult.failed:
                    SendMessage("FAILED");
                    return;
            }
        }
        private LoginResult ValidateLogin(string username, string password)
        {
            try
            {
                if (!dbHandler.UsernameExists(username))
                    return LoginResult.NoSuchUsername;
                if (!dbHandler.PasswordCorrect(username, password))
                    return LoginResult.WrongPassword;
                return LoginResult.Success;
            }
            catch
            {
                return LoginResult.failed;
            }
        }
        #endregion

        #region Register Logic
        private void Register(string registerData)
        {
            string username = registerData.Split('/')[0];
            string password = registerData.Split('/')[1];
            RegisterResult registerResult = ValidateRegister(username, password);

            switch (registerResult)
            {
                case RegisterResult.Success:
                    SendMessage("SUCCESS");
                    return;
                case RegisterResult.UsernameExists:
                    SendMessage("USERNAME EXISTS");
                    return;
                case RegisterResult.InvalidPassword:
                    SendMessage("INVALID PASSWORD");
                    return;
                case RegisterResult.failed:
                    SendMessage("FAILED");
                    return;
            }
        }
        private RegisterResult ValidateRegister(string username, string password)
        {
            try
            {
                if (dbHandler.UsernameExists(username))
                    return RegisterResult.UsernameExists;
                else if (password.Length == 0)
                    return RegisterResult.InvalidPassword;
                Console.WriteLine($"INSERTING {username} {password}");
                dbHandler.InsertUser(username, password);
                return RegisterResult.Success;
            }
            catch
            {
                return RegisterResult.failed;
            }
        }
        #endregion

        private void DecodeMessage(string msg)
        {
            Console.WriteLine("GOT: " + msg);
            if (msg.Contains("LOGIN"))
                Login(msg.Split(':')[1]);
            else if (msg.Contains("REGISTER"))
                Register(msg.Split(':')[1]);
            else
                SendMessage("Invalid Message");
        }
        private void ReceiveMessage(IAsyncResult aR)
        {
            try
            {
                int bytesRead;
                lock (client.GetStream())
                    bytesRead = client.GetStream().EndRead(aR);
                string msg = Encoding.UTF8.GetString(buffer, 0, bytesRead);

                DecodeMessage(msg);

                client.Close();
                Console.WriteLine("Closed conn");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Caught Exception: {ex}");
            }
        }
        private void SendMessage(string msg)
        {
            Byte[] buffer = Encoding.UTF8.GetBytes(msg);
            client.GetStream().Write(buffer, 0, buffer.Length);
        }
    }
}