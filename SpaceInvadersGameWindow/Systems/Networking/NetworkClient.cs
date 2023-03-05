using System;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace GameWindow.Systems.Networking
{
    abstract class NetworkClient
    {
        private static readonly char messageSeperator = '+';
        protected TcpClient client;
        private Byte[] buffer;

        public NetworkClient()
        {
            client = new TcpClient();
            buffer = new Byte[client.ReceiveBufferSize];
        }
        protected bool ConnectToAddress(string ip, int port)
        {
            try
            {
                client.Connect(IPEndPoint.Parse($"{ip}:{port}"));
                return true;
            }
            catch
            {
                return false;
            }
        }
        protected void BeginSingleRead()
        {
            client.GetStream().BeginRead(buffer, 0, buffer.Length, (result) => ReceiveMessage(result, false), null);
        }
        protected void BeginRead()
        {
            client.GetStream().BeginRead(buffer, 0, buffer.Length, (result) => ReceiveMessage(result, true), null);
        }
        protected void SendMessage(string msg)
        {
            Byte[] toSendBuffer = Encoding.UTF8.GetBytes(msg + messageSeperator);
            client.GetStream().Write(toSendBuffer, 0, toSendBuffer.Length);
        }
        private void ReceiveMessage(IAsyncResult ar,bool loop)
        {
            int bytesRead;
            lock (client.GetStream())
                bytesRead = client.GetStream().EndRead(ar);
            string msg = Encoding.UTF8.GetString(buffer, 0, bytesRead);
            DecodeMessage(msg);

            if (loop)
                client.GetStream().BeginRead(buffer, 0, buffer.Length, (result) => ReceiveMessage(result, true), null);
        }
        protected abstract void DecodeMessage(string msg);
        public void StopClient()
        {
            client.Close();
        }
    }
}
