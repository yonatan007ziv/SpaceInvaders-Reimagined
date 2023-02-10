using System.Net;
using System.Net.Sockets;

namespace LoginRegistServer
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
                new ClientLoginRegistValidator(listener.AcceptTcpClient(), dbHandler);
        }
    }
}