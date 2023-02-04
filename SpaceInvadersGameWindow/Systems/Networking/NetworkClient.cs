﻿using System;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace SpaceInvadersGameWindow.Systems.Networking
{
    abstract class NetworkClient
    {
        private TcpClient client;
        private Byte[] buffer;
        private int port = 7777;

        public NetworkClient()
        {
            client = new TcpClient();
            client.Connect(IPEndPoint.Parse($"127.0.0.1:{port}"));
            buffer = new Byte[client.ReceiveBufferSize];
        }
        protected void ConnectToAddress(string ip)
        {
            client.Connect(IPEndPoint.Parse(ip));
        }
        protected void BeginSingleRead()
        {
            client.GetStream().BeginRead(buffer, 0, buffer.Length, ReceiveMessage, null);
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
        protected abstract void DecodeMessage(string msg);
        public void StopClient()
        {
            client.Close();
        }
    }
}
