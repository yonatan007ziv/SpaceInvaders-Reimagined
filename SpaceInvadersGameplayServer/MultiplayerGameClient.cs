using GameplayServer.GameData;
using System.Net.Sockets;

namespace GameplayServer
{
    /// <summary>
    /// A class responsible for communicating with a gameplay player client
    /// </summary>
    class MultiplayerGameClient
    {
        private static readonly Dictionary<string, MultiplayerGameClient> players = new Dictionary<string, MultiplayerGameClient>();
        private static readonly BunkerData[] bunkersData = new BunkerData[8];
        private static int teamAScore = 0;
        private static int teamBScore = 0;
        private static int teamAPlayers = 0;
        private static int teamBPlayers = 0;

        private readonly NetworkClientHandler clientHandler;
        private PlayerData playerData;
        private bool gotUsername = false;
        private bool disconnected = false;

        static MultiplayerGameClient()
        {
            for (int i = 0; i < 8; i++)
                bunkersData[i] = new BunkerData(i);
        }

        /// <summary>
        /// Broadcasts a message to all players
        /// </summary>
        /// <param name="msg"> The message to broadcast </param>
        private static void Broadcast(string msg)
        {
            Console.WriteLine($"BROADCASTING: {msg}");
            foreach (MultiplayerGameClient p in players.Values)
                p.SendMessage(msg);
        }

        /// <summary>
        /// Checks if one or more of the teams deserve a bunker
        /// </summary>
        public static void BunkersChecker()
        {
            if (teamAScore >= 25)
                Broadcast("SERVER$GiveTeamBunker:A");
            else
                Broadcast("SERVER$RevokeTeamBunker:A");
            if (teamBScore >= 25)
                Broadcast("SERVER$GiveTeamBunker:B");
            else
                Broadcast("SERVER$RevokeTeamBunker:B");
        }

        /// <summary>
        /// Broadcasts the team scores to all players
        /// </summary>
        public static void BroadcastScores()
        {
            Broadcast($"SERVER$Score:(A,{teamAScore})");
            Broadcast($"SERVER$Score:(B,{teamBScore})");
        }

        /// <summary>
        /// Gets the next player's team, teams are balanced
        /// </summary>
        /// <returns> 'A' or 'B' depending on the next team </returns>
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

        /// <summary>
        /// Builds a game client object under an open connection <see cref="TcpClient"/>
        /// </summary>
        /// <param name="client"> The client's connection </param>
        public MultiplayerGameClient(TcpClient client)
        {
            clientHandler = new NetworkClientHandler(client, InterpretMessage, OnDisconnect);
            playerData = new PlayerData();
        }

        /// <summary>
        /// Interprets the message gotten from the stream
        /// </summary>
        /// <param name="msg"> The received message </param>
        private void InterpretMessage(string msg)
        {
            if (!gotUsername)
            {
                gotUsername = true;
                playerData.username = msg;

                // If username already connected
                if (players.ContainsKey(msg))
                {
                    clientHandler.SendMessage("AlreadyConnected");
                    clientHandler.EndRead();
                    return;
                }

                playerData.team = NextTeam();

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

                BunkersChecker();
                return;
            }

            if (msg.Contains("PlayerPosition"))
                int.TryParse(msg.Split(':')[1], out playerData.xPos);
            else if (msg.Contains("CreateBunker"))
            {
                int bunkerID = int.Parse(msg.Split(':')[1]);
                for (int i = 0; i < bunkersData[bunkerID].parts.Length; i++)
                    bunkersData[bunkerID].parts[i].stage = 1;

                if (playerData.team == 'A')
                    teamAScore -= 25;
                else
                    teamBScore -= 25;

                BroadcastScores();
                BunkersChecker();
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

                    BunkersChecker();
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

        /// <summary>
        /// Handles the disconnection of the player
        /// </summary>
        private void OnDisconnect()
        {
            if (playerData.username == "" || disconnected) return;

            disconnected = true;
            Console.WriteLine($"Disconnected username: ({playerData.username})");
            foreach (KeyValuePair<string, MultiplayerGameClient> player in players)
                if (player.Value == this)
                    players.Remove(player.Key);

            if (playerData.team == 'A')
                teamAPlayers--;
            else if (playerData.team == 'B')
                teamBPlayers--;

            Broadcast($"{playerData.username}$Left");
        }

        /// <summary>
        /// Forwards a message to send to the client handler
        /// </summary>
        /// <param name="msg"> The message to send </param>
        private void SendMessage(string msg) => clientHandler.SendMessage(msg);
    }
}