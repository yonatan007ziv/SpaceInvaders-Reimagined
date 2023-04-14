using System.Net.Sockets;
using System.Security.Cryptography;
using System.Text;
using GameplayServer.GameData;

namespace GameplayServer
{
    class MultiplayerGameClient
    {
        private static readonly Dictionary<string, MultiplayerGameClient> players = new Dictionary<string, MultiplayerGameClient>();

        private readonly TcpClient client;
        private IAsyncResult currentRead;
        private readonly Byte[] buffer;

        private bool gotUsername = false;
        private PlayerData playerData;

        static MultiplayerGameClient()
        {
            for (int i = 0; i < 8; i++)
                bunkersData[i] = new BunkerData(i, true);
        }

        public MultiplayerGameClient(TcpClient client)
        {
            this.client = client;
            buffer = new Byte[client.ReceiveBufferSize];
            currentRead = this.client.GetStream().BeginRead(buffer, 0, buffer.Length, (ar) => { ReceiveRSA(); WriteEncryptedAes(); BeginRead(); }, null);

            playerData = new PlayerData();
        }

        #region Encryption
        // Encryption related fields
        private readonly RSA rsa = RSA.Create();
        private readonly Aes aes = Aes.Create();

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
        #endregion

        #region Game Data and Login
        private static readonly BunkerData[] bunkersData = new BunkerData[8];
        private static int teamAScore = 0;
        private static int teamBScore = 0;
        private static int teamAPlayers = 0;
        private static int teamBPlayers = 0;

        public static void BunkersOpportunity()
        {
            if (teamAScore >= 25)
                Broadcast("SERVER$TeamBunker:A");
            if (teamBScore >= 25)
                Broadcast("SERVER$TeamBunker:B");
        }
        public static void BroadcastScores()
        {
            Broadcast($"SERVER$Score:(A,{teamAScore})");
            Broadcast($"SERVER$Score:(B,{teamBScore})");
        }
        private static char NextTeam()
        {
            if (teamAPlayers > teamBPlayers)
            {
                teamBPlayers++;
                return 'B';
            }
            else
            {
                teamAPlayers++;
                return 'A';
            }
        }
        private void OnDisconnect()
        {
            foreach (KeyValuePair<string, MultiplayerGameClient> player in players)
                if (player.Value == this)
                    players.Remove(player.Key);

            if (playerData.team == 'A')
                teamAPlayers--;
            else
                teamBPlayers--;

            DatabaseHandler.UpdateConnected(playerData.username, false);
            Broadcast($"{playerData.username}$Left");
        }
        #endregion

        private static void Broadcast(string msg)
        {
            Console.WriteLine(msg);
            foreach (MultiplayerGameClient p in players.Values)
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

            try { client.GetStream().Write(encryptedWithPrefix, 0, encryptedWithPrefix.Length); }
            catch { OnDisconnect(); }
        }
        private void BeginRead()
        {
            try { currentRead = client.GetStream().BeginRead(buffer, 0, buffer.Length, ReceiveMessage, null); }
            catch { OnDisconnect(); }
        }
        private void EndRead()
        {
            try { client.GetStream().EndRead(currentRead); }
            catch { OnDisconnect(); }
        }
        private void ReceiveMessage(IAsyncResult aR)
        {
            int bytesRead = 0;

            try
            {
                lock (client.GetStream())
                    bytesRead = client.GetStream().EndRead(aR);
            }
            catch { OnDisconnect(); return; }

            if (bytesRead == 0) { OnDisconnect(); return; }

            byte[] encrypted = new byte[bytesRead];
            Array.Copy(buffer, encrypted, bytesRead);
            EncryptedSeperator(encrypted);

            BeginRead();
        }
        private void DecodeMessage(string msg)
        {
            if (!gotUsername)
            {
                gotUsername = true;
                if (players.ContainsKey(msg))
                {
                    SendMessage("AlreadyConnected");
                    EndRead();
                    return;
                }

                playerData.username = msg;

                playerData.team = NextTeam();
                DatabaseHandler.UpdateConnected(playerData.username, true);

                // Broadcast Player
                SendMessage($"{playerData.username}$InitiatePlayer:({playerData.xPos},{playerData.team})");
                Broadcast($"{playerData.username}$InitiatePlayer:({playerData.xPos},{playerData.team})");
                players.Add(playerData.username, this);

                // Send Scores
                SendMessage($"SERVER$Score:(A,{teamAScore})");
                SendMessage($"SERVER$Score:(B,{teamBScore})");

                // Send Players 
                foreach (MultiplayerGameClient p in players.Values)
                    if (p != this)
                        SendMessage($"{p.playerData.username}$InitiatePlayer:({p.playerData.xPos},{p.playerData.team})");

                // Send Bunker Data
                foreach (BunkerData b in bunkersData)
                    foreach (BunkerPartData p in b.parts)
                        SendMessage($"SERVER$InitiateBunkerPart:({p.BunkerID},{(int)p.partType},{p.stage})");

                BunkersOpportunity();
                return;
            }

            if (msg.Contains("PlayerPosition"))
                int.TryParse(msg.Split(':')[1], out playerData.xPos);
            else if (msg.Contains("CreateBunker"))
            {
                int bunkerID = int.Parse(msg.Split(':')[1]);
                bunkersData[bunkerID] = new BunkerData(bunkerID);

                if (playerData.team == 'A')
                    teamAScore -= 25;
                else
                    teamBScore -= 25;

                BroadcastScores();
                BunkersOpportunity();
            }
            else if (msg.Contains("BulletHit"))
            {
                string HitObject = msg.Split(':')[1].Split("(")[0];
                string HitObjectDetails = msg.Split('(')[1].Split(")")[0];

                if (HitObject.Contains("Player"))
                {
                    if (playerData.team == 'A')
                        teamAScore += 10;
                    else
                        teamBScore += 10;

                    if (players[HitObjectDetails].playerData.team == 'A')
                        teamAScore -= 5;
                    else
                        teamBScore -= 5;

                    BunkersOpportunity();
                    BroadcastScores();
                }
                else if (HitObject.Contains("BunkerPart"))
                {
                    string partDetails = msg.Split("(")[1].Split(')')[0];
                    int BunkerID = int.Parse(partDetails.Split(',')[0]);
                    int part = int.Parse(partDetails.Split(',')[1]);
                    int stage = int.Parse(partDetails.Split(',')[2]);
                    bunkersData[BunkerID].parts[part].stage = stage;
                }
            }

            Broadcast($"{playerData.username}${msg}");
        }
    }
}