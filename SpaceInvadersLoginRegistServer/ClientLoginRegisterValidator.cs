using System.Net.Sockets;

namespace LoginRegisterServer
{
    /// <summary>
    /// A class responsible for communicating with a Login / Register request
    /// </summary>
    internal class ClientLoginRegisterValidator
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

        private readonly NetworkClientHandler clientHandler;
        private string username = "", password = "", email = "";
        private int generated2FACode;

        /// <summary>
        /// Builds a new <see cref="ClientLoginRegisterValidator"/> responsible for validating the login / register attempt
        /// </summary>
        /// <param name="client"> The Login / Register client </param>
        public ClientLoginRegisterValidator(TcpClient client)
        {
            Console.WriteLine("New Login/Register client accepted");
            clientHandler = new NetworkClientHandler(client, InterpretMessage, () => { });
        }

        /// <summary>
        /// Interprets the message gotten from the stream
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

            clientHandler.SendMessage(loginResult.ToString());
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

            clientHandler.SendMessage(registerResult.ToString());
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

                generated2FACode = new Random().Next(10000, 100000); // 5 digit code
                SMTPHandler.SendEmail(email, "Space Invaders - Reimagined", $"Your 2FA code is: {generated2FACode}");
                clientHandler.BeginRead();
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
            if (int.TryParse(code, out int codeReceived) && codeReceived == generated2FACode)
            {
                Console.WriteLine($"INSERTING USER: ({username}, {password}, {email})");
                DatabaseHandler.InsertUser(username, password, email);
                clientHandler.SendMessage(RegisterResult.Success.ToString());
            }
            else
                clientHandler.SendMessage(RegisterResult.Wrong2FA.ToString());
        }
        #endregion
    }
}