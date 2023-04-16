using System.Net.Sockets;
using System.Security.Cryptography;
using System.Text;

namespace LoginRegistServer
{
    /// <summary>
    /// A class responsible for communicating with a Login / Register request
    /// </summary>
    internal class ClientLoginRegistValidator
    {
        /// <summary>
        /// Represents all the possible replies for a login attempt
        /// </summary>
        private enum LoginResult
        {
            Success,
            NoSuchUsername,
            WrongPassword,
            AlreadyConnected,
            Failed
        }

        /// <summary>
        /// Represents all the possible replies for a register attempt
        /// </summary>
        private enum RegisterResult
        {
            Success,
            Need2FA,
            Wrong2FA,
            UsernameExists,
            EmailExists,
            InvalidUsername,
            InvalidPassword,
            InvalidEmail,
            Failed
        }

        private string username = "", password = "", email = "";
        private int generated2FACode;

        private Byte[] buffer;
        private TcpClient client;

        // Encryption related fields
        private RSA rsa = RSA.Create();
        private Aes aes = Aes.Create();

        /// <summary>
        /// Builds a new <see cref="ClientLoginRegistValidator"/> responsible for validating the login / register attempt
        /// </summary>
        /// <param name="client"> The Login / Register client </param>
        public ClientLoginRegistValidator(TcpClient client)
        {
            Console.WriteLine("New Login/Register client accepted");

            this.client = client;
            buffer = new Byte[client.ReceiveBufferSize];

            this.client.GetStream().BeginRead(buffer, 0, buffer.Length, (ar) => { ReceiveRSA(); WriteEncryptedAes(); BeginRead(); }, null);
        }

        #region Login Logic
        /// <summary>
        /// Starts the login procedure
        /// </summary>
        /// <param name="loginData"> The gotten login data </param>
        private void Login(string loginData)
        {
            username = loginData.Split('/')[0];
            password = loginData.Split('/')[1];
            LoginResult loginResult = ValidateLogin();

            SendMessage(loginResult.ToString());
        }

        /// <summary>
        /// Validate Login in the following steps:
        /// <list type="number">
        ///     <item> Check if username exists in the database </item>
        ///     <item> Check if password is correct </item>
        ///     <item> Check if player is not connected </item>
        /// </list>
        /// If those checks are positive, then the login data is validated
        /// </summary>
        /// <returns> A <see cref="LoginResult"/> representing the result of the check </returns>
        private LoginResult ValidateLogin()
        {
            Console.WriteLine($"VALIDATING LOGIN: ({username}, {password})");
            try
            {
                if (!DatabaseHandler.UsernameExists(username))
                    return LoginResult.NoSuchUsername;
                else if (!DatabaseHandler.PasswordCorrect(username, password))
                    return LoginResult.WrongPassword;
                else if (DatabaseHandler.IsConnected(username))
                    return LoginResult.AlreadyConnected;

                return LoginResult.Success;
            }
            catch
            {
                return LoginResult.Failed;
            }
        }
        #endregion

        #region Register Logic
        /// <summary>
        /// Starts the register procedure
        /// </summary>
        /// <param name="registerData"> The gotten register data </param>
        private void Register(string registerData)
        {
            username = registerData.Split('/')[0];
            password = registerData.Split('/')[1];
            email = registerData.Split('/')[2];
            RegisterResult registerResult = ValidateRegister();

            SendMessage(registerResult.ToString());
        }

        /// <summary>
        /// Validate Login in the following steps:
        /// <list type="number">
        ///     <item> Check if username does not exists in the database </item>
        ///     <item> Check if email does not exists in the database </item>
        ///     <item> Check if username password and email are valid </item>
        /// </list>
        /// If those checks are positive, then the register data is validated and initiates 2FA check
        /// </summary>
        /// <returns> A <see cref="RegisterResult"/> representing the result of the check </returns>
        private RegisterResult ValidateRegister()
        {
            Console.WriteLine($"VALIDATING REGISTER: ({username}, {password}, {email})");
            try
            {
                if (DatabaseHandler.UsernameExists(username))
                    return RegisterResult.UsernameExists;
                else if (DatabaseHandler.EmailExists(email))
                    return RegisterResult.EmailExists;
                else if (username.Length == 0 || username.Contains('/'))
                    return RegisterResult.InvalidUsername;
                else if (password.Length == 0 || password.Contains('/'))
                    return RegisterResult.InvalidPassword;
                else if (email.Length == 0 || email.Contains('/'))
                    return RegisterResult.InvalidEmail;

                generated2FACode = new Random().Next(10000, 100000); // 5 digit number
                SMTPHandler.SendEmail(email, "Space Invaders - Reimagined", $"Your 2FA code is: {generated2FACode}");
                BeginRead();
                return RegisterResult.Need2FA;
            }
            catch (FormatException ex) when (ex.Message.Contains("e-mail"))
            {
                return RegisterResult.InvalidEmail;
            }
            catch
            {
                return RegisterResult.Failed;
            }
        }

        /// <summary>
        /// Compares the gotten code against the generated code and responds appropriately
        /// </summary>
        /// <param name="code"> The received 2FA code </param>
        private void Check2FA(string code)
        {
            if (int.TryParse(code, out int codeReceived))
                if (codeReceived == generated2FACode)
                {
                    Console.WriteLine($"INSERTING USER: ({username}, {password}, {email})");
                    DatabaseHandler.InsertUser(username, password, email);
                    SendMessage(RegisterResult.Success.ToString());
                    return;
                }
            SendMessage(RegisterResult.Wrong2FA.ToString());
        }
        #endregion

        /// <summary>
        /// Interprets the message gotten from the client
        /// </summary>
        /// <param name="msg"> The received message </param>
        private void InterpretMessage(string msg)
        {
            if (msg.Contains("Login"))
                Login(msg.Split(':')[1]);
            else if (msg.Contains("Register"))
                Register(msg.Split(':')[1]);
            else if (msg.Contains("2FACode"))
                Check2FA(msg.Split(':')[1]);
        }
        private void ReceiveRSA()
        {
            rsa.ImportRSAPublicKey(buffer, out _);
        }
        private void WriteEncryptedAes()
        {
            byte[] aesKey = aes.Key;
            byte[] aesIV = aes.IV;

            byte[] encryptedAesKey = rsa.Encrypt(aesKey, RSAEncryptionPadding.OaepSHA256);
            byte[] encryptedAesIV = rsa.Encrypt(aesIV, RSAEncryptionPadding.OaepSHA256);

            byte[] encryptedAesKeyIV = encryptedAesKey.Concat(encryptedAesIV).ToArray();
            client.GetStream().Write(encryptedAesKeyIV, 0, encryptedAesKeyIV.Length);
        }
        private void BeginRead()
        {
            client.GetStream().BeginRead(buffer, 0, buffer.Length, ReceiveMessage, null);
        }
        private void ReceiveMessage(IAsyncResult aR)
        {
            int bytesRead = 0;
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
            EncryptedSeperator(encrypted);
        }
        private void EncryptedSeperator(byte[] encryptedWithPrefix)
        {
            List<byte[]> encryptedMessages = new List<byte[]>();
            int pos = 0;

            while (pos < encryptedWithPrefix.Length)
            {
                int currentLength = BitConverter.ToInt32(encryptedWithPrefix, pos);
                pos += sizeof(int);
                byte[] encryptedMessage = new byte[currentLength];
                Array.Copy(encryptedWithPrefix, pos, encryptedMessage, 0, currentLength);
                encryptedMessages.Add(encryptedMessage);
                pos += currentLength;
            }

            foreach (byte[] encryptedMessage in encryptedMessages)
                DecodeEncryptedMessage(encryptedMessage);
        }
        private void DecodeEncryptedMessage(byte[] encrypted)
        {
            byte[] decrypted = aes.CreateDecryptor(aes.Key, aes.IV).TransformFinalBlock(encrypted, 0, encrypted.Length);
            string msg = Encoding.UTF8.GetString(decrypted);
            InterpretMessage(msg);
        }
        private void SendMessage(string msg)
        {
            byte[] bytes = Encoding.UTF8.GetBytes(msg);
            byte[] encrypted = aes.CreateEncryptor().TransformFinalBlock(bytes, 0, bytes.Length);

            byte[] encryptedWithPrefix = new byte[encrypted.Length + sizeof(int)];
            byte[] length = BitConverter.GetBytes(encrypted.Length);

            int pos;
            // Add the encrypted message's length as a prefix
            for (pos = 0; pos < length.Length; pos++)
                encryptedWithPrefix[pos] = length[pos];

            Array.Copy(encrypted, 0, encryptedWithPrefix, pos, encrypted.Length);

            client.GetStream().Write(encryptedWithPrefix, 0, encryptedWithPrefix.Length);
        }
    }
}