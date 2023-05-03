using System.Net;
using System.Net.Sockets;

namespace LoginRegisterServer
{
    /// <summary>
    /// Login-Register Server
    /// </summary>
    internal class LoginRegisterServer
    {
        private const int port = 7777;

        /// <summary>
        /// Entry point for the Login-Register Server
        /// </summary>
        public static void Main()
        {
            TcpListener listener = new TcpListener(IPAddress.Any, port);
            listener.Start(); // Start listening

            while (true)
                new ClientLoginRegisterValidator(listener.AcceptTcpClient());
        }
    }
}