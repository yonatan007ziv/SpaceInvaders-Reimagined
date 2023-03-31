using System;
using System.Net;
using System.Net.Sockets;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace GameWindow.Systems.Networking
{
    abstract class NetworkClient
    {
        private static readonly char messageSeperator = '+';

        private TcpClient client;
        private Byte[] buffer;

        // Encryption related fields
        private TaskCompletionSource<bool> EncryptionReady = new TaskCompletionSource<bool>();
        private RSA rsa = RSA.Create();
        private Aes aes = Aes.Create();

        public NetworkClient()
        {
            client = new TcpClient();
            buffer = new Byte[client.ReceiveBufferSize];
        }

        protected abstract void DecodeMessage(string msg);
        protected bool ConnectToAddress(string ip, int port)
        {
            try
            {
                client.Connect(IPEndPoint.Parse($"{ip}:{port}"));

                byte[] publicKey = rsa.ExportRSAPublicKey();
                client.GetStream().Write(publicKey, 0, publicKey.Length);
                client.GetStream().BeginRead(buffer, 0, buffer.Length, ReceiveAES, null);

                return true;
            }
            catch
            {
                return false;
            }
        }
        protected async void BeginSingleRead()
        {
            await EncryptionReady.Task;
            client.GetStream().BeginRead(buffer, 0, buffer.Length, (result) => ReceiveMessage(result, false), null);
        }
        protected async void BeginRead()
        {
            await EncryptionReady.Task;
            client.GetStream().BeginRead(buffer, 0, buffer.Length, (result) => ReceiveMessage(result, true), null);
        }
        protected async void SendMessage(string msg)
        {
            await EncryptionReady.Task;
            byte[] bytes = Encoding.UTF8.GetBytes(msg);
            byte[] encrypted = aes.CreateEncryptor().TransformFinalBlock(bytes, 0, bytes.Length);
            client.GetStream().Write(encrypted, 0, encrypted.Length);
        }
        private void ReceiveAES(IAsyncResult ar)
        {
            byte[] encryptedAesKey = new byte[256];
            byte[] encryptedAesIV = new byte[256];

            Array.Copy(buffer, 0, encryptedAesKey, 0, 256);
            Array.Copy(buffer, 256, encryptedAesIV, 0, 256);

            byte[] decryptedAesKey = rsa.Decrypt(encryptedAesKey, RSAEncryptionPadding.OaepSHA256);
            byte[] decryptedAesIV = rsa.Decrypt(encryptedAesIV, RSAEncryptionPadding.OaepSHA256);

            aes.Key = decryptedAesKey;
            aes.IV = decryptedAesIV;

            EncryptionReady.SetResult(true);
        }
        private void ReceiveMessage(IAsyncResult ar, bool loop)
        {
            int bytesRead = -1;
            lock (client.GetStream())
                bytesRead = client.GetStream().EndRead(ar);

            byte[] encrypted = new byte[bytesRead];
            Array.Copy(buffer, encrypted, bytesRead);
            byte[] decrypted = aes.CreateDecryptor().TransformFinalBlock(encrypted, 0, encrypted.Length);

            string msg = Encoding.UTF8.GetString(decrypted);
            MessageSeperator(msg);
            if (loop)
                client.GetStream().BeginRead(buffer, 0, buffer.Length, (result) => ReceiveMessage(result, true), null);
        }
        private void MessageSeperator(string msg)
        {
            if (msg == "") return;

            if (msg.Contains(messageSeperator))
            {
                MessageSeperator(msg.Split(messageSeperator)[0]);
                MessageSeperator(msg.Split(messageSeperator)[1]);
            }
            else
                DecodeMessage(msg);
        }
        public void StopClient()
        {
            client.Close();
        }
    }
}