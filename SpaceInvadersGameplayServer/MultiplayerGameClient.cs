using System.Net.Sockets;
using System.Security.Cryptography;
using System.Text;

namespace GameplayServer
{
    class MultiplayerGameClient
    {
        private static readonly Dictionary<string, MultiplayerGameClient> players = new Dictionary<string, MultiplayerGameClient>();

        private TcpClient client;
        private Byte[] buffer;

        // Team A: 0-3
        // Team B: 4-7
        private static BunkerData[] BunkersData = new BunkerData[8];

        public bool gotUsername = false;
        private GamePlayerData playerData;
        public MultiplayerGameClient(TcpClient client)
        {
            this.client = client;
            buffer = new Byte[client.ReceiveBufferSize];
            this.client.GetStream().BeginRead(buffer, 0, buffer.Length, (ar) => { ReceiveRSA(); WriteEncryptedAes(); BeginRead(); }, null);

            playerData = new GamePlayerData();
        }

        #region Encryption
        // Encryption related fields
        private RSA rsa = RSA.Create();
        private Aes aes = Aes.Create();

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

        #region Game Logic
        private void UpdateAllScores()
        {
            Broadcast($"SERVER$SCORE:(A,{teamAScore})");
            Broadcast($"SERVER$SCORE:(B,{teamBScore})");
        }
        private void GetScores()
        {
            SendMessage($"SERVER$SCORE:(A,{teamAScore})");
            SendMessage($"SERVER$SCORE:(B,{teamBScore})");
        }
        private void GetPlayers()
        {
            foreach (MultiplayerGameClient p in players.Values)
                if (p != this)
                    SendMessage($"{p.playerData.username}$INITIATE PLAYER:({p.playerData.xPos},{p.playerData.team})");
        }
        private static void CreateBunker(int bIndex)
        {
            BunkersData[bIndex] = new BunkerData(bIndex);
            BroadcastBunker(BunkersData[bIndex]);
        }
        private void GetAllBunkers()
        {
            foreach (BunkerData b in BunkersData)
                if (b != null)
                    foreach (BunkerPartData p in b.parts)
                        if (p != null)
                            SendMessage($"SERVER$INITIATE BUNKERPART:({p.BunkerID},{(int)p.part},{p.stage})");
        }
        private static void BroadcastBunker(BunkerData b)
        {
            foreach (BunkerPartData p in b.parts)
                if (p != null)
                    Broadcast($"SERVER$INITIATE BUNKERPART:({p.BunkerID},{(int)p.part},{p.stage})");
        }

        private static bool gaveABunker;
        private static bool gaveBBunker;
        public static void BunkersOpportunity()
        {
            if (teamAScore >= 25 && !gaveABunker)
            {
                Broadcast("SERVER$TEAM BUNKER:A");
                gaveABunker = true;
            }
            if (teamBScore >= 25 && !gaveBBunker)
            {
                Broadcast("SERVER$TEAM BUNKER:B");
                gaveBBunker = true;
            }
        }
        #endregion
        private void BeginRead()
        {
            try { client.GetStream().BeginRead(buffer, 0, buffer.Length, ReceiveMessage, null); }
            catch { OnDisconnect(); }
        }
        private static void Broadcast(string msg)
        {
            Console.WriteLine($"BROADCASTING:{msg}");
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
        private void OnDisconnect()
        {
            foreach (KeyValuePair<string, MultiplayerGameClient> player in players)
                if (player.Value == this)
                    players.Remove(player.Key);

            if (playerData.team == 'A')
                teamAPlayers--;
            else
                teamBPlayers--;

            Broadcast($"{playerData.username}$LEFT");
        }
        private void DecodeMessage(string msg)
        {
            if (!gotUsername)
            {
                gotUsername = true;
                playerData.username = msg;
                playerData.team = NextTeam();
                players.Add(playerData.username, this);
                Broadcast($"{playerData.username}$INITIATE PLAYER:({playerData.xPos},{playerData.team})");
                GetScores();
                GetPlayers();
                GetAllBunkers();
                return;
            }

            if (msg.Contains("PLAYER POS"))
                int.TryParse(msg.Split(':')[1], out playerData.xPos);
            else if (msg.Contains("CREATE BUNKER"))
            {
                int bunkerNum = int.Parse(msg.Split(':')[1]);
                if (0 <= bunkerNum && bunkerNum <= 3)
                {
                    teamAScore -= 25;
                    gaveABunker = false;
                }
                else
                {
                    teamBScore -= 25;
                    gaveBBunker = false;
                }
                UpdateAllScores();
                CreateBunker(bunkerNum);
            }
            else if (msg.Contains("BULLET HIT"))
            {
                string HitDetails = msg.Split('(')[1].Split(")")[0];
                string HitObject = HitDetails.Split(',')[0];

                if (HitObject.Contains("Player"))
                {
                    string playerName = HitDetails.Split(',')[1];
                    if (playerData.team == 'A')
                        teamAScore += 10;
                    else
                        teamBScore += 10;

                    if (players[playerName].playerData.team == 'A')
                        teamAScore -= 5;
                    else
                        teamBScore -= 5;

                    BunkersOpportunity();

                    Broadcast($"SERVER$SCORE:(A,{teamAScore})");
                    Broadcast($"SERVER$SCORE:(B,{teamBScore})");
                }
                else if (HitObject.Contains("BunkerPart"))
                {
                    int BunkerID = int.Parse(HitObject.Split('%')[1]);
                    int part = int.Parse(HitDetails.Split(',')[1]);

                    BunkersData[BunkerID].parts[part].stage++;
                }
            }

            Broadcast($"{playerData.username}${msg}");
        }
        private static int teamAScore = 0;
        private static int teamBScore = 0;
        private static int teamAPlayers = 0;
        private static int teamBPlayers = 0;
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
    }
}