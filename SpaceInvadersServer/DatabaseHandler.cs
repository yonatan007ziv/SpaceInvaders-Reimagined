using Microsoft.Win32;
using System.Data.SqlClient;
using System.Text.RegularExpressions;

namespace SpaceInvadersServer
{
    internal class DatabaseHandler
    {
        private SqlConnection conn;
        public DatabaseHandler()
        {
            conn = new SqlConnection("Data Source=(LocalDB)\\MSSQLLocalDB;AttachDbFilename=\"D:\\Code\\VS Community\\SpaceInvaders-Reimagined\\SpaceInvadersServer\\Database.mdf\";Integrated Security=True");
        }
        public bool ValidLogin(string username, string password)
        {
            if (!UsernameExists(username))
                return false;
            SqlCommand cmd = conn.CreateCommand();
            cmd.CommandText = $"SELECT Password FROM [UsersDatabase] WHERE Username='{username}'";
            conn.Open();
            string result = (string)cmd.ExecuteScalar();
            conn.Close();
            return result == password;
        }
        public bool UsernameExists(string username)
        {
            SqlCommand cmd = conn.CreateCommand();
            cmd.CommandText = $"SELECT Count(Username) FROM [UsersDatabase] WHERE Username='{username}'";
            conn.Open();
            int result = (int)cmd.ExecuteScalar();
            conn.Close();
            return result > 0;
        }
        public void InsertUser(string username, string password)
        {
            SqlCommand cmd = conn.CreateCommand();
            cmd.CommandText = $"INSERT INTO [UsersDatabase] (Username, Password) VALUES ('{username}', '{password}')";
            conn.Open();
            cmd.ExecuteScalar();
            conn.Close();
        }
    }
}