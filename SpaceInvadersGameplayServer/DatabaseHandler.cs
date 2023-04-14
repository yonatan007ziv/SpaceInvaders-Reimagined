using System.Data.SqlClient;

namespace GameplayServer
{
    /// <summary>
    /// DatabaseHandler class provides methods to interact with a SQL database for user authentication and registration purposes
    /// </summary>
    internal static class DatabaseHandler
    {
        private static SqlConnection conn = new SqlConnection(@"Data Source=(LocalDB)\MSSQLLocalDB;AttachDbFilename=""D:\Code\VS Community\SpaceInvaders-Reimagined\SpaceInvadersLoginRegistServer\Users.mdf"";Integrated Security=True");
        /// <summary>
        /// Updates if a user is currently connected to the game
        /// </summary>
        /// <param name="username"> The username of the account to update </param>
        /// <param name="connected"> The "Connected" new value </param>
        /// <returns> Returns true if the user is connected, otherwise false </returns>
        public static void UpdateConnected(string username, bool connected)
        {
            SqlCommand cmd = conn.CreateCommand();
            cmd.CommandText = $"UPDATE [Users] SET Connected='{connected}' WHERE Username='{username}'";
            conn.Open();
            cmd.ExecuteScalar();
            conn.Close();
        }
    }
}