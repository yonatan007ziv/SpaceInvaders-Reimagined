using System.Net.Sockets;
using System.Text;

namespace GameplayServer
{
    class MultiplayerGameClient
    {
        private static readonly char messageSeperator = '+';
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
                if (p != this)
                    this.SendMessage($"{p.nickname}$INITIATE PLAYER:");
        }
        private static void Broadcast(string msg)
        {
            Console.WriteLine($"BROADCASTING:{msg}");
            foreach (MultiplayerGameClient p in players)
                p.SendMessage(msg);
        }
        private void SendMessage(string msg)
        {
            Byte[] toSendBuffer = Encoding.UTF8.GetBytes(msg + messageSeperator);
            client.GetStream().Write(toSendBuffer, 0, toSendBuffer.Length);
        }
        private void ReceiveMessage(IAsyncResult ar)
        {
            try
            {
                int bytesRead;
                lock (client.GetStream())
                    bytesRead = client.GetStream().EndRead(ar);
                string msg = Encoding.UTF8.GetString(buffer, 0, bytesRead);
                DecodeSeperator(msg);

                client.GetStream().BeginRead(buffer, 0, buffer.Length, ReceiveMessage, null);
            }
            catch
            {
                players.Remove(this);
                Broadcast($"{nickname}$LEFT:");
            }
        }
        private void DecodeSeperator(string msg)
        {
            if (msg == "") return;

            if (msg.Contains(messageSeperator))
            {
                DecodeSeperator(msg.Split(messageSeperator)[0]);
                DecodeSeperator(msg.Split(messageSeperator)[1]);
            }
            else
                DecodeMessage(msg);
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