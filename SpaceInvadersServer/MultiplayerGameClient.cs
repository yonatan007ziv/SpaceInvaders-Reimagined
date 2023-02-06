using System.Net;
using System.Net.Sockets;
using System.Runtime.CompilerServices;
using System.Text;

namespace SpaceInvadersServer
{
    class MultiplayerGameClient
    {
        private static List<TcpClient> clients = new List<TcpClient>();

        private TcpClient client;
        private Byte[] buffer;

        public MultiplayerGameClient(TcpClient client)
        {
            clients.Add(client);
            this.client = client;
            buffer = new byte[this.client.ReceiveBufferSize];

            this.client.GetStream().BeginRead(buffer, 0, buffer.Length, ReceiveMessage, null);
        }
        private void Broadcast(string msg)
        {
            foreach (TcpClient client in clients)
            {
                Byte[] toSendBuffer = Encoding.UTF8.GetBytes(msg);
                client.GetStream().Write(toSendBuffer, 0, toSendBuffer.Length);
            }
        }
        private void ReceiveMessage(IAsyncResult ar)
        {
            int bytesRead;
            lock (client.GetStream())
                bytesRead = client.GetStream().EndRead(ar);
            string msg = Encoding.UTF8.GetString(buffer, 0, bytesRead);
            DecodeMessage(msg);
            client.GetStream().BeginRead(buffer, 0, buffer.Length, ReceiveMessage, null);
        }

        private bool gotNick;
        private string nickname;
        private void DecodeMessage(string msg)
        {
            if (!gotNick)
            {
                gotNick = true;
                nickname = msg;
                return;
            }

            if (msg.Contains("MOVE:"))
                Broadcast(msg);
            else if (msg.Contains("SHOOT:"))
                Broadcast(msg);
            else if (msg.Contains("DEATH:"))
                Broadcast(msg);
        }
    }
}