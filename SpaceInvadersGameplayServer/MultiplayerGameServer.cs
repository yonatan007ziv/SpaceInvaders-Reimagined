using System.Net;
using System.Net.Sockets;

namespace SpaceInvadersGameplayServer
{
    internal class MultiplayerGameServer
    {
        public MultiplayerGameServer()
        {
            TcpListener listener = new TcpListener(IPAddress.Any, 7778);
            listener.Start();

            while (true)
                new MultiplayerGameClient(listener.AcceptTcpClient());
        }
    }
}
