using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net.Sockets;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace GameWindow.Systems.Networking
{
    /// <summary>
    /// Base abstract class for all network clients
    /// </summary>
    public abstract class NetworkClient
    {
        // Client related fields
        private TcpClient client = new TcpClient();
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
                client.Connect(ip, port);

                // After successful connection, Write the RSA public key
                WriteRSA();

                //After writing the RSA public key, read received AES Key and IV
                client.GetStream().BeginRead(buffer, 0, buffer.Length, ReceiveAES, null);

                return true;
            }
            catch(SocketException ex) when (ex.SocketErrorCode == SocketError.ConnectionRefused)
            {
                Debug.WriteLine($"Connection Refused: {ip}:{port}");
            }
            return false;
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
            
            // Get message bytes
            byte[] bytes = Encoding.UTF8.GetBytes(msg);

            // Get encrypted message bytes
            byte[] encrypted = EncryptAES(bytes);

            // Get encrypted message bytes with 4 first bytes representing the length of the message
            byte[] encryptedWithPrefix = Prefix4BytesLength(encrypted);

            client.GetStream().Write(encryptedWithPrefix, 0, encryptedWithPrefix.Length);
        }

        /// <summary>
        /// Encrypts an array of bytes using <see cref="aes"/>
        /// </summary>
        /// <param name="bytes"> The bytes to encrypt </param>
        /// <returns> A byte array containing the encrypted bytes </returns>
        private byte[] EncryptAES(byte[] bytes) { return aes.CreateEncryptor().TransformFinalBlock(bytes, 0, bytes.Length); }
        
        /// <summary>
        /// Takes an array of bytes and prefixes its length at the first 4 bytes
        /// </summary>
        /// <param name="arr"></param>
        /// <returns> A byte array cointaining the original message with the first 4 bytes being length-bytes </returns>
        private byte[] Prefix4BytesLength(byte[] arr)
        {
            byte[] arrWithPrefix = new byte[arr.Length + sizeof(int)];
            byte[] length = BitConverter.GetBytes(arr.Length);

            // Prefix 4 length-bytes to the start of the array
            int pos;
            for (pos = 0; pos < length.Length; pos++)
                arrWithPrefix[pos] = length[pos];

            // Copy the original message after length-bytes
            Array.Copy(arr, 0, arrWithPrefix, pos, arr.Length);

            // Return the original message with 4 length-bytes at the start
            return arrWithPrefix;
        }

        /// <summary>
        /// Writes the RSA public key to the remote stream
        /// </summary>
        private void WriteRSA()
        {
            byte[] publicKey = rsa.ExportRSAPublicKey();
            client.GetStream().Write(publicKey, 0, publicKey.Length);
        }

        /// <summary>
        /// The first read of the stream: Reads the AES Key and IV and raises <see cref="EncryptionReady"/>
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
            catch { StopClient(); return; }

            if (bytesRead == 0) { StopClient(); return; }

            byte[] encrypted = new byte[bytesRead];
            Array.Copy(buffer, encrypted, bytesRead);

            EncryptedSeperator(encrypted);

            if (loop)
                try { client.GetStream().BeginRead(buffer, 0, buffer.Length, (result) => ReceiveMessage(result, true), null); }
                catch { StopClient(); return; }
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