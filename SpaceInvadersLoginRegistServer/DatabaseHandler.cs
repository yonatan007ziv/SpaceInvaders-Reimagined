using System.Data.SqlClient;

namespace LoginRegisterServer
{
    /// <summary>
    /// DatabaseHandler class provides methods to interact with a SQL database for user authentication and registration purposes
    /// </summary>
    internal static class DatabaseHandler
    {
        private const string connString = @"Data Source=(LocalDB)\MSSQLLocalDB;AttachDbFilename=""D:\Code\VS Community\SpaceInvaders-Reimagined\SpaceInvadersLoginRegistServer\Users.mdf"";Integrated Security=True";
        private readonly static SqlConnection conn = new SqlConnection(connString);

        /// <summary>
        /// Checks if a username exists in the database
        /// </summary>
        /// <param name="username"> The username to check </param>
        /// <returns> Returns true if the username exists in the database, otherwise false </returns>
        public static bool UsernameExists(string username)
        {
            SqlCommand cmd = conn.CreateCommand();
            cmd.CommandText = $"SELECT Count(Username) FROM [Users] WHERE Username='{username}'";

            conn.Open();
            int result = (int)cmd.ExecuteScalar();
            conn.Close();
            return result > 0;
        }

        /// <summary>
        /// Checks if an email address exists in the database
        /// </summary>
        /// <param name="email"> The email address to check </param>
        /// <returns> Returns true if the email exists in the database, otherwise false </returns>
        public static bool EmailExists(string email)
        {
            SqlCommand cmd = conn.CreateCommand();
            cmd.CommandText = $"SELECT Count(Email) FROM [Users] WHERE Email='{email}'";

            conn.Open();
            int result = (int)cmd.ExecuteScalar();
            conn.Close();
            return result > 0;
        }

        /// <summary>
        /// Checks if the provided password matches the password stored in the database for a given username
        /// </summary>
        /// <param name="username"> The username of the account to check the password for </param>
        /// <param name="password"> The password to check against the database </param>
        /// <returns> Returns true if the password is correct, otherwise false </returns>
        public static bool PasswordCorrect(string username, string password)
        {
            SqlCommand cmd = conn.CreateCommand();
            cmd.CommandText = $"SELECT Count(*) FROM [Users] WHERE Username='{username}' AND Password='{password}'";
            conn.Open();
            int result = (int)cmd.ExecuteScalar();
            conn.Close();
            return result > 0;
        }

        /// <summary>
        /// Inserts a new user record into the database with the provided username, password, and email
        /// </summary>
        /// <param name="username"> The desired username for the new account </param>
        /// <param name="password"> The desired password for the new account </param>
        /// <param name="email"> The email address associated with the new account </param>
        public static void InsertUser(string username, string password, string email)
        {
            SqlCommand cmd = conn.CreateCommand();
            cmd.CommandText = $"INSERT INTO [Users] (Username, Password, Email, Connected) VALUES ('{username}', '{password}', '{email}', '0')";
            conn.Open();
            cmd.ExecuteScalar();
            conn.Close();
        }
    }
}