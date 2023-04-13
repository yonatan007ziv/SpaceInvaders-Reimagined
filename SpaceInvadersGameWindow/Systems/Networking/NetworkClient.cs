using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace GameWindow.Systems.Networking
{
    public abstract class NetworkClient
    {
        // Client related fieles
        private TcpClient client;
        private Byte[] buffer;

        // Encryption related fields
        private TaskCompletionSource<bool> EncryptionReady = new TaskCompletionSource<bool>();
        private RSA rsa = RSA.Create();
        private Aes aes = Aes.Create();

        /// <summary>
        /// Builds the <see cref="client"/> and <see cref="buffer"/> objects
        /// </summary>
        public NetworkClient()
        {
            client = new TcpClient();
            buffer = new Byte[client.ReceiveBufferSize];
        }

        /// <summary>
        /// Abstract method for interpreting the received message
        /// </summary>
        /// <param name="message"> Message to interpret </param>
        protected abstract void InterpretMessage(string message);

        /// <summary>
        /// Establishes and initializes a safe transport to the desired endpoint by connecting and writing the public key
        /// </summary>
        /// <param name="ip"> Endpoint's IP </param>
        /// <param name="port"> Endpoint's port </param>
        /// <returns> <c>true</c> if the connection was established successfully; otherwise, <c>false</c> </returns>
        protected bool Connect(string ip, int port)
        {

            try
            {
                client.Connect(IPAddress.Parse(ip), port);

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

        /// <summary>
        /// Awaits until the encryption is ready, and then starts reading from the stream
        /// </summary>
        /// <param name="loop"> Whether the read is looped or not </param>
        protected async void BeginRead(bool loop)
        {
            await EncryptionReady.Task;
            client.GetStream().BeginRead(buffer, 0, buffer.Length, (result) => ReceiveMessage(result, loop), null);
        }

        /// <summary>
        /// Writes a message to the endpoint in the following steps:
        /// <list type="number">
        ///     <item> Awaits until the encryption is ready </item>
        ///     <item> Encrypts the message </item>
        ///     <item> Attaches 4 bytes representing the messages length to the Encrypted message </item>
        ///     <item> Writes to the recipient 's stream </item>
        /// </list>
        /// </summary>
        /// <remarks>
        /// Remark:
        /// Important to prefix the length of the message because sometimes messages get written too fast and get joined up
        /// </remarks>
        /// <param name="msg"> The message to write </param>
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

        /// <summary>
        /// The first read of the stream: Reads the AES Key and IV and raises <see cref="EncryptionReady"/> to true
        /// </summary>
        /// <param name="ar"> A <see cref="IAsyncResult"/> representing the async state of the read operation </param>
        private void ReceiveAES(IAsyncResult ar)
        {
            int bytesRead;
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

        /// <summary>
        /// Receives encrypted message(s), and separates them
        /// </summary>
        /// <param name="ar"> A <see cref="IAsyncResult"/> representing the async state of the read operation </param>
        /// <param name="loop"> Whether the read is looped or not </param>
        /// <exception cref="Exception"> Thrown if an exception is thrown when getting the network stream or ending read has failed </exception>
        private void ReceiveMessage(IAsyncResult ar, bool loop)
        {
            int bytesRead = 0;

            try
            {
                lock (client.GetStream())
                    bytesRead = client.GetStream().EndRead(ar);
            }
            catch { StopClient(); }

            if (bytesRead == 0) { StopClient(); return; }

            byte[] encrypted = new byte[bytesRead];
            Array.Copy(buffer, encrypted, bytesRead);

            EncryptedSeperator(encrypted);

            if (loop)
                client.GetStream().BeginRead(buffer, 0, buffer.Length, (result) => ReceiveMessage(result, true), null);
        }

        /// <summary>
        /// Separates the encrypted messages (if got more than one) and decrypts and interprets them
        /// </summary>
        /// <param name="encryptedWithPrefix"> A byte array containing the received encrypted message(s), with the first 4 bytes representing the length </param>
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
                DecryptMessage(encryptedMessage);
        }

        /// <summary>
        /// Decrypts the AES bytes and interprets the gotten message
        /// </summary>
        /// <param name="encrypted"> A byte array containing the Encrypted message </param>
        private void DecryptMessage(byte[] encrypted)
        {
            byte[] decrypted = DecryptAES(encrypted);
            InterpretMessage(Encoding.UTF8.GetString(decrypted));
        }

        /// <summary>
        /// Decrypts the gotten AES bytes
        /// </summary>
        /// <param name="encrypted"> A byte array containing the Encrypted message </param>
        /// <returns> A byte array containing the decrypted message </returns>
        private byte[] DecryptAES(byte[] encrypted)
        {
            return aes.CreateDecryptor(aes.Key, aes.IV).TransformFinalBlock(encrypted, 0, encrypted.Length);
        }

        /// <summary>
        /// <list type="bullet">
        ///     <item> Disconnects the current client and releases its resources </item>
        ///     <item> Constructs a new <see cref="TcpClient"/> ready for use </item>
        /// </list>
        /// </summary>
        public void StopClient()
        {
            if (client.Connected)
            {
                client.GetStream().Close();
                client.Close();
            }
            client = new TcpClient();
        }
    }
}