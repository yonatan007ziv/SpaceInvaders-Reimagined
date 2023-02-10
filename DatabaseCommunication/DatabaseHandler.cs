/*using System.Data.SqlClient;

namespace DatabaseCommunication
{
    internal class DatabaseHandler : NetworkServer
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
        public void Connected(string username)
        {
            UpdateConnected(username, 1);
        }
        public void Disconnected(string username)
        {
            UpdateConnected(username, 0);
        }
        public bool IsConnected(string username)
        {
            SqlCommand cmd = conn.CreateCommand();
            cmd.CommandText = $"SELECT Count(*) FROM [Users] WHERE Username = '{username}' AND Connected = '{1}'";
            conn.Open();
            int result = (int)cmd.ExecuteScalar();
            conn.Close();
            return result == 1;
        }
        private void UpdateConnected(string username, int bit)
        {
            SqlCommand cmd = conn.CreateCommand();
            cmd.CommandText = $"UPDATE [Users] SET Connected = '{bit}' WHERE Username = '{username}'";
            conn.Open();
            cmd.ExecuteScalar();
            conn.Close();
        }
    }
}*/