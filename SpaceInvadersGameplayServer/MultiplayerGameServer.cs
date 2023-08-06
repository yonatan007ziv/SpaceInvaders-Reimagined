using System.Net;
using System.Net.Sockets;

namespace GameplayServer
{
    /// <summary>
    /// Game Server
    /// </summary>
    internal class MultiplayerGameServer
    {
        private const int port = 7779;

        /// <summary>
        /// Entry point for the Game Server
        /// </summary>
        public static void Main()
        {
            TcpListener listener = new TcpListener(IPAddress.Any, port);
            listener.Start(); // Start listening

            while (true)
                new MultiplayerGameClient(listener.AcceptTcpClient());
        }
    }
}