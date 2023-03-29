using System.Net;
using System.Net.Sockets;

namespace LoginRegistServer
{
    internal class LoginRegistServer
    {
        private static TcpListener listener = new TcpListener(IPAddress.Any, 7777);
        private static DatabaseHandler dbHandler= new DatabaseHandler();
        public static void Main()
        {
            listener.Start();
            while (true)
                new ClientLoginRegistValidator(listener.AcceptTcpClient(), dbHandler);
        }
    }
}