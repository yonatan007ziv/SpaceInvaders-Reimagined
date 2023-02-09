using System.Net;
using System.Net.Sockets;
using System.Runtime.CompilerServices;
using System.Text;

namespace SpaceInvadersGameplayServer
{
    class MultiplayerGameClient
    {
        private static List<MultiplayerGameClient> players = new List<MultiplayerGameClient>();

        private TcpClient client;
        private Byte[] buffer;
        private bool gotNick;
        private string? nickname;

        public MultiplayerGameClient(TcpClient client)
        {
            players.Add(this);
            this.client = client;
            buffer = new byte[this.client.ReceiveBufferSize];

            this.client.GetStream().BeginRead(buffer, 0, buffer.Length, ReceiveMessage, null);

            foreach (MultiplayerGameClient p in players)
            {
                if (p == this) continue;
                Byte[] toSendBuffer = Encoding.UTF8.GetBytes($"{p.nickname}$INITIATE PLAYER:");
                client.GetStream().Write(toSendBuffer, 0, toSendBuffer.Length);
            }

        }
        private void Broadcast(string msg)
        {
            Console.WriteLine($"BROADCASTING:{msg}");
            foreach (MultiplayerGameClient p in players)
            {
                Byte[] toSendBuffer = Encoding.UTF8.GetBytes(msg);
                p.client.GetStream().Write(toSendBuffer, 0, toSendBuffer.Length);
            }
        }
        private void ReceiveMessage(IAsyncResult ar)
        {
            try
            {
                int bytesRead;
                lock (client.GetStream())
                    bytesRead = client.GetStream().EndRead(ar);
                string msg = Encoding.UTF8.GetString(buffer, 0, bytesRead);
                DecodeMessage(msg);
                client.GetStream().BeginRead(buffer, 0, buffer.Length, ReceiveMessage, null);
            }
            catch
            {
                players.Remove(this);
                Broadcast($"{nickname}$LEFT:");
            }
        }
        private void DecodeMessage(string msg)
        {
            if (!gotNick)
            {
                gotNick = true;
                nickname = msg;
                Broadcast($"{nickname}$INITIATE PLAYER:");
                return;
            }
            else
                Broadcast($"{nickname}${msg}");
        }
    }
}