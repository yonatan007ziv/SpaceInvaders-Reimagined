using System.Net.Sockets;
using System.Security.Cryptography;
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

        // Encryption related fields
        private RSA rsa = RSA.Create();
        private Aes aes = Aes.Create();

        public MultiplayerGameClient(TcpClient client)
        {
            players.Add(this);

            this.client = client;
            buffer = new Byte[client.ReceiveBufferSize];

            this.client.GetStream().Read(buffer, 0, buffer.Length);
            ReceiveRSA();

            foreach (MultiplayerGameClient p in players)
                if (p != this)
                    SendMessage($"{p.nickname}$INITIATE PLAYER:");
        }
        private void ReceiveRSA()
        {
            rsa.ImportRSAPublicKey(buffer, out _);

            byte[] aesKey = aes.Key;
            byte[] aesIV = aes.IV;

            byte[] encryptedAesKey = rsa.Encrypt(aesKey, RSAEncryptionPadding.OaepSHA256);
            byte[] encryptedAesIV = rsa.Encrypt(aesIV, RSAEncryptionPadding.OaepSHA256);

            byte[] encryptedAesKeyIV = encryptedAesKey.Concat(encryptedAesIV).ToArray();
            client.GetStream().Write(encryptedAesKeyIV, 0, encryptedAesKeyIV.Length);

            client.GetStream().BeginRead(buffer, 0, buffer.Length, ReceiveMessage, null);
        }

        private void Broadcast(string msg)
        {
            Console.WriteLine($"BROADCASTING:{msg}");
            foreach (MultiplayerGameClient p in players)
                p.SendMessage(msg);
        }
        private void SendMessage(string msg)
        {
            byte[] bytes = Encoding.UTF8.GetBytes(msg);
            byte[] encrypted = aes.CreateEncryptor().TransformFinalBlock(bytes, 0, bytes.Length);
            client.GetStream().Write(encrypted, 0, encrypted.Length);
        }
        private void ReceiveMessage(IAsyncResult aR)
        {
            int bytesRead = -1;
            try
            {
                lock (client.GetStream())
                    bytesRead = client.GetStream().EndRead(aR);
            }
            catch
            {
                players.Remove(this);
                Broadcast($"{nickname}$LEFT:");
                return;
            }

            byte[] encrypted = new byte[bytesRead];
            Array.Copy(buffer, encrypted, bytesRead);
            byte[] decrypted = aes.CreateDecryptor(aes.Key, aes.IV).TransformFinalBlock(encrypted, 0, encrypted.Length);

            string msg = Encoding.UTF8.GetString(decrypted);
            DecodeSeperator(msg);
            client.GetStream().BeginRead(buffer, 0, buffer.Length, ReceiveMessage, null);
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