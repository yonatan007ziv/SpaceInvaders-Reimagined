using System.Net;
using System.Net.Sockets;

namespace GameplayServer
{
    /// <summary>
    /// Game Server
    /// </summary>
    internal class MultiplayerGameServer
    {
        /// <summary>
        /// Entry point for the Game Server
        /// </summary>
        public static void Main()
        {
            TcpListener listener = new TcpListener(IPAddress.Any, 7778);
            listener.Start(); // Start listening

            while (true)
                new MultiplayerGameClient(listener.AcceptTcpClient());
        }
    }
}