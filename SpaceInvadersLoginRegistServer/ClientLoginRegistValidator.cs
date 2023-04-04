using System.Net.Sockets;
using System.Security.Cryptography;
using System.Text;

namespace LoginRegistServer
{
    internal class ClientLoginRegistValidator
    {
        private static readonly char messageSeperator = '+';

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

        private Byte[] buffer;
        private TcpClient client;
        private DatabaseHandler dbHandler;

        // Encryption related fields
        private RSA rsa = RSA.Create();
        private Aes aes = Aes.Create();

        public ClientLoginRegistValidator(TcpClient client, DatabaseHandler dbHandler)
        {
            Console.WriteLine("New client accepted.");

            this.client = client;
            this.dbHandler = dbHandler;
            buffer = new Byte[client.ReceiveBufferSize];

            this.client.GetStream().Read(buffer, 0, buffer.Length);
            ReceiveRSA();
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
            msg = msg.Split(messageSeperator)[0];
            Console.WriteLine("GOT: " + msg);
            if (msg.Contains("LOGIN"))
                Login(msg.Split(':')[1]);
            else if (msg.Contains("REGISTER"))
                Register(msg.Split(':')[1]);
            else
                SendMessage("Invalid Message");
        }
        private void ReceiveRSA()
        {
            rsa.ImportRSAPublicKey(buffer, out _);

            byte[] aesKey = aes.Key;
            byte[] aesIV = aes.IV;

            byte[] encryptedAesKey = rsa.Encrypt(aesKey, RSAEncryptionPadding.OaepSHA256);
            byte[] encryptedAesIV = rsa.Encrypt(aesIV, RSAEncryptionPadding.OaepSHA256);

            byte[] encryptedAesKeyIV = encryptedAesKey.Concat(encryptedAesIV).ToArray();
            client.GetStream().Write(encryptedAesKeyIV, 0, encryptedAesKeyIV.Length);

            client.GetStream().BeginRead(buffer, 0, buffer.Length, ReceiveMessage, null);
        }
        private void ReceiveMessage(IAsyncResult aR)
        {
            int bytesRead = -1;
            try
            {
                lock (client.GetStream())
                    bytesRead = client.GetStream().EndRead(aR);
            }
            catch (Exception ex)
            {
                client.Close();
                Console.WriteLine("Closed conn");
                Console.WriteLine($"Caught Exception: {ex}");
                return;
            }

            byte[] encrypted = new byte[bytesRead];
            Array.Copy(buffer, encrypted, bytesRead);
            byte[] decrypted = aes.CreateDecryptor(aes.Key, aes.IV).TransformFinalBlock(encrypted, 0, encrypted.Length);

            string msg = Encoding.UTF8.GetString(decrypted);
            DecodeMessage(msg);
        }
        private void SendMessage(string msg)
        {
            byte[] bytes = Encoding.UTF8.GetBytes(msg);
            byte[] encrypted = aes.CreateEncryptor().TransformFinalBlock(bytes, 0, bytes.Length);
            client.GetStream().Write(encrypted, 0, encrypted.Length);
        }
    }
}