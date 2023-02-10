using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace GameWindow.Systems.Networking
{
    abstract class NetworkClient
    {
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
            client.GetStream().BeginRead(buffer, 0, buffer.Length, ReceiveMessage, null);
        }
        protected void BeginRead()
        {
            client.GetStream().BeginRead(buffer, 0, buffer.Length, ReceiveMessageLoop, null);
        }
        protected void SendMessage(string msg)
        {
            Byte[] toSendBuffer = Encoding.UTF8.GetBytes(msg);
            client.GetStream().Write(toSendBuffer, 0, toSendBuffer.Length);
        }
        private void ReceiveMessage(IAsyncResult ar)
        {
            int bytesRead;
            lock (client.GetStream())
                bytesRead = client.GetStream().EndRead(ar);
            string msg = Encoding.UTF8.GetString(buffer, 0, bytesRead);
            DecodeMessage(msg);
        }
        private void ReceiveMessageLoop(IAsyncResult ar)
        {
            int bytesRead;
            lock (client.GetStream())
                bytesRead = client.GetStream().EndRead(ar);
            string msg = Encoding.UTF8.GetString(buffer, 0, bytesRead);
            DecodeMessage(msg);
            client.GetStream().BeginRead(buffer, 0, buffer.Length, ReceiveMessageLoop, null);
        }
        protected abstract void DecodeMessage(string msg);
        public void StopClient()
        {
            client.Close();
        }
    }
}
