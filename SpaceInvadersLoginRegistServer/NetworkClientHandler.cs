using System.Net.Sockets;
using System.Security.Cryptography;
using System.Text;

namespace LoginRegisterServer
{
    internal class NetworkClientHandler
    {
        public delegate void ActionString(string str);

        private readonly Action onDisconnect;
        private readonly ActionString interpretMessage;
        private readonly Byte[] buffer;
        private readonly TcpClient client;
        private IAsyncResult currentRead;

        // Encryption related fields
        private readonly RSA rsa = RSA.Create();
        private readonly Aes aes = Aes.Create();

        /// <summary>
        /// Takes a buffer and returns a new buffer with the added first 4 bytes representing the original buffer's length
        /// </summary>
        /// <param name="buffer"> The original buffer </param>
        /// <returns> The new buffer with the first 4 bytes representing length </returns>
        private static byte[] AddLengthPrefix(byte[] buffer)
        {
            byte[] prefixedBuffer = new byte[buffer.Length + sizeof(int)];
            byte[] length = BitConverter.GetBytes(buffer.Length);

            int pos;
            // Add the encrypted message's length as a prefix
            for (pos = 0; pos < length.Length; pos++)
                prefixedBuffer[pos] = length[pos];

            // Copy the rest of the buffer to the new buffer
            Array.Copy(buffer, 0, prefixedBuffer, pos, buffer.Length);

            return prefixedBuffer;
        }

        /// <summary>
        /// Separates the conjoined encrypyted messages and returns a list with the buffers 
        /// </summary>
        /// <param name="encryptedWithPrefix"> The received message's buffer with the first 4 bytes representing the length of the message </param>
        /// <returns> A list containing all of the separates messages </returns>
        private static List<byte[]> SeparateEncrypted(byte[] encryptedWithPrefix)
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

            return encryptedMessages;
        }

        /// <summary>
        /// Builds a new client handler
        /// </summary>
        /// <param name="client"> The underlying connection </param>
        /// <param name="interpretMessage"> A reference to the interpret message method </param>
        public NetworkClientHandler(TcpClient client, ActionString interpretMessage, Action onDisconnect)
        {
            this.client = client;
            buffer = new Byte[client.ReceiveBufferSize];
            this.onDisconnect = onDisconnect;
            this.interpretMessage = interpretMessage;

            currentRead = this.client.GetStream().BeginRead(buffer, 0, buffer.Length, (ar) => { ReceiveRSA(); WriteEncryptedAES(); BeginRead(); }, null);
        }

        /// <summary>
        /// Starts a read from the stream
        /// </summary>
        public void BeginRead()
        {
            try { currentRead = client.GetStream().BeginRead(buffer, 0, buffer.Length, (aR) => ReceiveMessage(aR, interpretMessage), null); }
            catch { onDisconnect(); }
        }

        /// <summary>
        /// Ends the current read operation <see cref="currentRead"/>
        /// </summary>
        public void EndRead()
        {
            try { client.GetStream().EndRead(currentRead); }
            catch { onDisconnect(); }
        }

        /// <summary>
        /// Handles the MessageReceived logic in the following steps:
        /// <list type="number">
        ///     <item> Checks if the client disconnected </item>
        ///     <item> Separates conjoined messages (if there are any) </item>
        ///     <item> Interprets the messages </item>
        /// </list>
        /// </summary>
        /// <param name="aR"> A <see cref="IAsyncResult"/> representing the async state of the read operation </param>
        private void ReceiveMessage(IAsyncResult aR, ActionString interpretMessage)
        {
            int bytesRead = 0;
            try
            {
                lock (client.GetStream())
                    bytesRead = client.GetStream().EndRead(aR);
            }
            catch
            {
                onDisconnect();
                Console.WriteLine($"Connection Closed Forcibly");
                return;
            }

            if (bytesRead == 0)
            {
                onDisconnect();
                return;
            }

            byte[] encrypted = new byte[bytesRead];
            Array.Copy(buffer, encrypted, bytesRead);

            List<byte[]> encryptedMessages = SeparateEncrypted(encrypted);
            foreach (byte[] encryptedMessage in encryptedMessages)
            {
                byte[] decrypted = DecryptBuffer(encryptedMessage);
                string msg = Encoding.UTF8.GetString(decrypted);
                interpretMessage(msg);
            }
        }

        /// <summary>
        /// Sends a message in the following steps:
        /// <list type="number">
        ///     <item> Extracts the bytes from the message </item>
        ///     <item> encrypts the extracted bytes </item>
        ///     <item> Prefixes 4 bytes representing the message's length </item>
        ///     <item> Writes the prefixed message to the recipient </item>
        /// </list>
        /// </summary>
        /// <param name="msg"> The message to send </param>
        public void SendMessage(string msg)
        {
            byte[] messageBytes = Encoding.UTF8.GetBytes(msg);
            byte[] encryptedBytes = EncryptBuffer(messageBytes);
            byte[] prefixedEncryptedBytes = AddLengthPrefix(encryptedBytes);

            client.GetStream().Write(prefixedEncryptedBytes, 0, prefixedEncryptedBytes.Length);
        }

        #region Encryption Decryption
        /// <summary>
        /// Handles the import the received RSA credentials
        /// </summary>
        private void ReceiveRSA()
        {
            rsa.ImportRSAPublicKey(buffer, out _);
        }

        /// <summary>
        /// Writes the AES credentials using the gotten <see cref="rsa"/> encryption <br/>
        /// and then disposes the <see cref="rsa"/> object
        /// </summary>
        private void WriteEncryptedAES()
        {
            byte[] aesKey = aes.Key;
            byte[] aesIV = aes.IV;

            byte[] encryptedAesKey = rsa.Encrypt(aesKey, RSAEncryptionPadding.OaepSHA256);
            byte[] encryptedAesIV = rsa.Encrypt(aesIV, RSAEncryptionPadding.OaepSHA256);
            rsa.Dispose();

            byte[] encryptedAesKeyIV = encryptedAesKey.Concat(encryptedAesIV).ToArray();
            client.GetStream().Write(encryptedAesKeyIV, 0, encryptedAesKeyIV.Length);
        }

        /// <summary>
        /// Takes a buffer and decrypts it
        /// </summary>
        /// <param name="encrypted"> A buffer containing the encrypted message </param>
        /// <returns> The decrypted buffer </returns>
        private byte[] EncryptBuffer(byte[] toEncrypt)
        {
            return aes.CreateEncryptor().TransformFinalBlock(toEncrypt, 0, toEncrypt.Length);
        }

        /// <summary>
        /// Takes a buffer and decrypts it
        /// </summary>
        /// <param name="encrypted"> A buffer containing the encrypted message </param>
        /// <returns> The decrypted buffer </returns>
        private byte[] DecryptBuffer(byte[] encrypted)
        {
            return aes.CreateDecryptor(aes.Key, aes.IV).TransformFinalBlock(encrypted, 0, encrypted.Length);
        }
        #endregion

        /// <summary>
        /// Disposes and current <see cref="NetworkClientHandler"/>
        /// </summary>
        public void Dispose()
        {
            client.GetStream().Close();
            client.Close();
        }
    }
}