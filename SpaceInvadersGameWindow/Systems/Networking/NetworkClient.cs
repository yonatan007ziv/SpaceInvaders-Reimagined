using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace GameWindow.Systems.Networking
{
    abstract class NetworkClient
    {
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

            byte[] encryptedWithPrefix = new byte[encrypted.Length + sizeof(int)];
            byte[] length = BitConverter.GetBytes(encrypted.Length);

            int pos;
            // Add the encrypted message's length as a prefix
            for (pos = 0; pos < length.Length; pos++)
                encryptedWithPrefix[pos] = length[pos];

            Array.Copy(encrypted, 0, encryptedWithPrefix, pos, encrypted.Length);

            client.GetStream().Write(encryptedWithPrefix, 0, encryptedWithPrefix.Length);
        }
        private void ReceiveAES(IAsyncResult ar)
        {
            int bytesRead = -1;
            lock (client.GetStream())
                bytesRead = client.GetStream().EndRead(ar);

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
            int bytesRead = 0;

            try
            {
                lock (client.GetStream())
                    bytesRead = client.GetStream().EndRead(ar);
            }
            catch { }

            if (bytesRead == 0) return;

            byte[] encrypted = new byte[bytesRead];
            Array.Copy(buffer, encrypted, bytesRead);

            EncryptedSeperator(encrypted);

            if (loop)
                client.GetStream().BeginRead(buffer, 0, buffer.Length, (result) => ReceiveMessage(result, true), null);
        }
        private void EncryptedSeperator(byte[] encryptedWithPrefix)
        {
            List<byte[]> encryptedMessages = new List<byte[]>();
            int pos = 0;

            while (pos < encryptedWithPrefix.Length)
            {
                int currentLength = BitConverter.ToInt32(encryptedWithPrefix, pos);
                pos += sizeof(int);
                byte[] encryptedMessage = new byte[currentLength];
                Array.Copy(encryptedWithPrefix, pos, encryptedMessage, 0, currentLength);
                encryptedMessages.Add(encryptedMessage);
                pos += currentLength;
            }

            foreach (byte[] encryptedMessage in encryptedMessages)
                DecodeEncryptedMessage(encryptedMessage);
        }
        private void DecodeEncryptedMessage(byte[] encrypted)
        {
            byte[] decrypted = aes.CreateDecryptor(aes.Key, aes.IV).TransformFinalBlock(encrypted, 0, encrypted.Length);
            string msg = Encoding.UTF8.GetString(decrypted);
            DecodeMessage(msg);
        }
        public void StopClient()
        {
            client.GetStream().Close();
            client.Close();
            client = new TcpClient();
        }
    }
}