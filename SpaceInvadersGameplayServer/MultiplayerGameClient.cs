using System.Linq;
using System.Net.Sockets;
using System.Security.Cryptography;
using System.Text;

namespace GameplayServer
{
    class MultiplayerGameClient
    {
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

            this.client.GetStream().BeginRead(buffer, 0, buffer.Length, (ar) => { ReceiveRSA(); WriteEncryptedAes(); GetPlayers();BeginRead(); }, null);
        }
        private void ReceiveRSA()
        {
            rsa.ImportRSAPublicKey(buffer, out _);
        }
        private void WriteEncryptedAes()
        {
            byte[] aesKey = aes.Key;
            byte[] aesIV = aes.IV;

            byte[] encryptedAesKey = rsa.Encrypt(aesKey, RSAEncryptionPadding.OaepSHA256);
            byte[] encryptedAesIV = rsa.Encrypt(aesIV, RSAEncryptionPadding.OaepSHA256);

            byte[] encryptedAesKeyIV = encryptedAesKey.Concat(encryptedAesIV).ToArray();
            client.GetStream().Write(encryptedAesKeyIV, 0, encryptedAesKeyIV.Length);
        }
        private void GetPlayers()
        {
            foreach (MultiplayerGameClient p in players)
                if (p != this)
                    SendMessage($"{p.nickname}$INITIATE PLAYER:");
        }
        private void BeginRead()
        {
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

            byte[] encryptedWithPrefix = new byte[encrypted.Length + sizeof(int)];
            byte[] length = BitConverter.GetBytes(encrypted.Length);

            int pos;
            // Add the encrypted message's length as a prefix
            for (pos = 0; pos < length.Length; pos++)
                encryptedWithPrefix[pos] = length[pos];

            Array.Copy(encrypted, 0, encryptedWithPrefix, pos, encrypted.Length);

            client.GetStream().Write(encryptedWithPrefix, 0, encryptedWithPrefix.Length);
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
            EncryptedSeperator(encrypted);

            BeginRead();
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