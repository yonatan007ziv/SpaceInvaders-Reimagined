using System.Data.SqlClient;

namespace SpaceInvadersLoginRegistServer
{
    internal class DatabaseHandler
    {
        private SqlConnection conn;
        public DatabaseHandler()
        {
            conn = new SqlConnection(@"Data Source=(LocalDB)\MSSQLLocalDB;AttachDbFilename=""D:\Code\VS Community\SpaceInvaders-Reimagined\SpaceInvadersLoginRegistServer\Users.mdf"";Integrated Security=True");
        }
        public bool UsernameExists(string username)
        {
            SqlCommand cmd = conn.CreateCommand();
            cmd.CommandText = $"SELECT Count(Username) FROM [Users] WHERE Username='{username}'";
            conn.Open();
            int result = (int)cmd.ExecuteScalar();
            conn.Close();
            return result > 0;
        }
        public bool PasswordCorrect(string username, string password)
        {
            SqlCommand cmd = conn.CreateCommand();
            cmd.CommandText = $"SELECT Count(*) FROM [Users] WHERE Username='{username}' AND Password='{password}'";
            conn.Open();
            int result = (int)cmd.ExecuteScalar();
            conn.Close();
            return result > 0;
        }
        public void InsertUser(string username, string password)
        {
            SqlCommand cmd = conn.CreateCommand();
            cmd.CommandText = $"INSERT INTO [Users] (Username, Password) VALUES ('{username}', '{password}')";
            conn.Open();
            cmd.ExecuteScalar();
            conn.Close();
        }
    }
}