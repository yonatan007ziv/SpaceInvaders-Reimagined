using System.Net;
using System.Net.Sockets;

namespace SpaceInvadersServer
{
    internal class LoginRegistServer
    {
        TcpListener listener;
        DatabaseHandler dbHandler;
        public LoginRegistServer()
        {
            listener = new TcpListener(IPAddress.Any, 7777);
            listener.Start();
            dbHandler = new DatabaseHandler();

            while (true)
            {
                new ClientLoginRegistValidator(listener.AcceptTcpClient(), (username, password) => check(username, password));
            }
        }
        bool check(string username, string password)
        {
            return dbHandler.ValidLogin(username, password);
        }
    }
}