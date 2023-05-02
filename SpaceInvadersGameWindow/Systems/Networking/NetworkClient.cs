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
        private TcpClient client;
        private Byte[] buffer;
        private IAsyncResult? currentRead;

        // Encryption related fields
        private TaskCompletionSource<bool> EncryptionReady = new TaskCompletionSource<bool>();
        private RSA rsa = RSA.Create();
        private Aes aes = Aes.Create();

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
        /// Abstract method for when an error occurs
        /// </summary>
        protected abstract void OnError();

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
            catch (SocketException ex) when (ex.SocketErrorCode == SocketError.ConnectionRefused)
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

            try
            {
                currentRead = client.GetStream().BeginRead(buffer, 0, buffer.Length,
                (result) => ReceiveMessage(result, loop), null);
            }
            catch { StopClient(); return; }
        }

        /// <summary>
        /// Awaits until the encryption is ready, and then stops the current reading from the stream
        /// </summary>
        protected async void EndRead()
        {
            await EncryptionReady.Task;

            if (currentRead != null)
                client.GetStream().EndRead(currentRead);
        }

        /// <summary>
        /// Receives encrypted message(s), and separates them
        /// </summary>
        /// <param name="aR"> A <see cref="IAsyncResult"/> representing the async state of the read operation </param>
        /// <param name="loop"> Whether the read is looped or not </param>
        /// <exception cref="Exception"> Thrown if an exception is thrown when getting the network stream or ending read has failed </exception>
        private void ReceiveMessage(IAsyncResult aR, bool loop)
        {
            int bytesRead = 0;
            try
            {
                lock (client.GetStream())
                    bytesRead = client.GetStream().EndRead(aR);
            }
            catch (Exception ex)
            {
                OnError();
                Console.WriteLine($"Closed conn, Caught Exception: {ex}");
                return;
            }

            if (bytesRead == 0) return;

            byte[] encrypted = new byte[bytesRead];
            Array.Copy(buffer, encrypted, bytesRead);

            List<byte[]> encryptedMessages = SeparateEncrypted(encrypted);
            foreach (byte[] encryptedMessage in encryptedMessages)
            {
                byte[] decrypted = DecryptBuffer(encryptedMessage);
                string msg = Encoding.UTF8.GetString(decrypted);
                InterpretMessage(msg);
            }

            if (loop)
                BeginRead(true);
        }

        /// <summary>
        /// Sends a message in the following steps:
        /// <list type="number">
        ///     <item> Awaits until the encryption is ready </item>
        ///     <item> Extracts the bytes from the message </item>
        ///     <item> encrypts the extracted bytes </item>
        ///     <item> Prefixes 4 bytes representing the message's length </item>
        ///     <item> Writes the prefixed message to the recipient </item>
        /// </list>
        /// </summary>
        /// <remarks>
        /// Remark: It's important to prefix the length of the message because sometimes messages get written too fast and get joined up
        /// </remarks>
        /// <param name="msg"> The message to write </param>
        protected async void SendMessage(string msg)
        {
            await EncryptionReady.Task;

            byte[] messageBytes = Encoding.UTF8.GetBytes(msg);
            byte[] encryptedBytes = EncryptBuffer(messageBytes);
            byte[] prefixedEncryptedBytes = AddLengthPrefix(encryptedBytes);

            client.GetStream().Write(prefixedEncryptedBytes, 0, prefixedEncryptedBytes.Length);
        }

        #region Encryption Decryption
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
        /// <list type="bullet">
        ///     <item> Disconnects the current client and releases its resources </item>
        ///     <item> Constructs a new <see cref="TcpClient"/> ready for use </item>
        /// </list>
        /// </summary>
        public void StopClient()
        {
            client.Close();
            client = new TcpClient();
        }
    }
}