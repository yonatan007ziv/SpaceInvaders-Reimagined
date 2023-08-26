using GameWindow.Components.GameComponents;
using GameWindow.Components.Miscellaneous;
using GameWindow.Components.Pages;
using GameWindow.Components.UIElements;
using GameWindow.Factories;
using GameWindow.Systems;
using GameWindow.Systems.Networking;
using System.Numerics;
using System.Windows.Input;

namespace GameWindow.Components.NetworkedComponents
{
    /// <summary>
    /// A class for all Gameplay-Server communication
    /// </summary>
    internal class NetworkedGameClient : NetworkClient
    {
        public static NetworkedGameClient? instance;
        private static bool Paused;
        private static bool heldEscape;
        private OnlinePauseMenu? pauseMenu;

        private CustomButton? bunkerButton1;
        private CustomButton? bunkerButton2;
        private CustomButton? bunkerButton3;
        private CustomButton? bunkerButton4;
        private CustomButton? bunkerButton5;
        private CustomButton? bunkerButton6;
        private CustomButton? bunkerButton7;
        private CustomButton? bunkerButton8;

        private CustomLabel? teamALabel;
        private CustomLabel? teamBLabel;
        private void SetScoreA(int value)
        { teamALabel!.Text = $"Team A: {value}"; }
        private void SetScoreB(int value)
        { teamBLabel!.Text = $"Team B: {value}"; }

        /// <summary>
        /// Prepares a new multiplayer game player
        /// </summary>
        /// <param name="IP"> "Game-Server"'s IP Address </param>
        /// <param name="port"> "Game-Server"'s port </param>
        /// <param name="username"> Current username </param>
        public NetworkedGameClient(string IP, int port) : base()
        {
            instance = this;
            if (Connect(IP, port))
            {
                SendMessage(MainWindow.username);
                BeginRead(true);

                Wall.MakeOnlineGameWalls();
                teamALabel = UIElementFactory.CreateLabel(new Transform(new Vector2(50, 25), new Vector2(30, 14)), @"Team A", System.Windows.Media.Colors.White);
                teamBLabel = UIElementFactory.CreateLabel(new Transform(new Vector2(50, 25), new Vector2(30, 14)), @"Team B", System.Windows.Media.Colors.White);

                bunkerButton1 = UIElementFactory.CreateButton(new Transform(new Vector2(24, 16), new Vector2(0.4f * (MainWindow.referenceSize.X / 2), 2 * MainWindow.referenceSize.Y / 3)), () => CreateBunker("0"), System.Windows.Media.Color.FromRgb(0, 255, 0), "CREATE BUNKER");
                bunkerButton2 = UIElementFactory.CreateButton(new Transform(new Vector2(24, 16), new Vector2(0.8f * (MainWindow.referenceSize.X / 2), 2 * MainWindow.referenceSize.Y / 3)), () => CreateBunker("1"), System.Windows.Media.Color.FromRgb(0, 255, 0), "CREATE BUNKER");
                bunkerButton3 = UIElementFactory.CreateButton(new Transform(new Vector2(24, 16), new Vector2(1.2f * (MainWindow.referenceSize.X / 2), 2 * MainWindow.referenceSize.Y / 3)), () => CreateBunker("2"), System.Windows.Media.Color.FromRgb(0, 255, 0), "CREATE BUNKER");
                bunkerButton4 = UIElementFactory.CreateButton(new Transform(new Vector2(24, 16), new Vector2(1.6f * (MainWindow.referenceSize.X / 2), 2 * MainWindow.referenceSize.Y / 3)), () => CreateBunker("3"), System.Windows.Media.Color.FromRgb(0, 255, 0), "CREATE BUNKER");
                bunkerButton5 = UIElementFactory.CreateButton(new Transform(new Vector2(24, 16), new Vector2(0.4f * (MainWindow.referenceSize.X / 2), 2 * MainWindow.referenceSize.Y / 3)), () => CreateBunker("4"), System.Windows.Media.Color.FromRgb(0, 255, 0), "CREATE BUNKER");
                bunkerButton6 = UIElementFactory.CreateButton(new Transform(new Vector2(24, 16), new Vector2(0.8f * (MainWindow.referenceSize.X / 2), 2 * MainWindow.referenceSize.Y / 3)), () => CreateBunker("5"), System.Windows.Media.Color.FromRgb(0, 255, 0), "CREATE BUNKER");
                bunkerButton7 = UIElementFactory.CreateButton(new Transform(new Vector2(24, 16), new Vector2(1.2f * (MainWindow.referenceSize.X / 2), 2 * MainWindow.referenceSize.Y / 3)), () => CreateBunker("6"), System.Windows.Media.Color.FromRgb(0, 255, 0), "CREATE BUNKER");
                bunkerButton8 = UIElementFactory.CreateButton(new Transform(new Vector2(24, 16), new Vector2(1.6f * (MainWindow.referenceSize.X / 2), 2 * MainWindow.referenceSize.Y / 3)), () => CreateBunker("7"), System.Windows.Media.Color.FromRgb(0, 255, 0), "CREATE BUNKER");

                bunkerButton1.Visible(false);
                bunkerButton2.Visible(false);
                bunkerButton3.Visible(false);
                bunkerButton4.Visible(false);
                bunkerButton5.Visible(false);
                bunkerButton6.Visible(false);
                bunkerButton7.Visible(false);
                bunkerButton8.Visible(false);

                InputHandler.AddInputLoop(InputLoop);
            }
        }

        /// <summary>
        /// Disables or enables the bunker buttons
        /// </summary>
        /// <param name="team"> Team to enable / disable the buttons for </param>
        /// <param name="show"> Whether to enable or disable the buttons </param>
        private void BunkerButtons(char team, bool show)
        {
            if (team == 'A')
            {
                if (show)
                {
                    if (!NetworkedBunker.Bunkers[0].BunkerExists())
                        bunkerButton1?.Visible(true);
                    if (!NetworkedBunker.Bunkers[1].BunkerExists())
                        bunkerButton2?.Visible(true);
                    if (!NetworkedBunker.Bunkers[2].BunkerExists())
                        bunkerButton3?.Visible(true);
                    if (!NetworkedBunker.Bunkers[3].BunkerExists())
                        bunkerButton4?.Visible(true);
                }
                else
                {
                    bunkerButton1?.Visible(false);
                    bunkerButton2?.Visible(false);
                    bunkerButton3?.Visible(false);
                    bunkerButton4?.Visible(false);
                }
            }
            else
            {
                if (show)
                {
                    if (!NetworkedBunker.Bunkers[4].BunkerExists())
                        bunkerButton5?.Visible(true);
                    if (!NetworkedBunker.Bunkers[5].BunkerExists())
                        bunkerButton6?.Visible(true);
                    if (!NetworkedBunker.Bunkers[6].BunkerExists())
                        bunkerButton7?.Visible(true);
                    if (!NetworkedBunker.Bunkers[7].BunkerExists())
                        bunkerButton8?.Visible(true);
                }
                else
                {
                    bunkerButton5?.Visible(false);
                    bunkerButton6?.Visible(false);
                    bunkerButton7?.Visible(false);
                    bunkerButton8?.Visible(false);
                }
            }
        }

        /// <summary>
        /// Disables bunker buttons for the current client and sends a CreateBunker message to the server
        /// </summary>
        /// <param name="bunkerID"> The bunker to create </param>
        private void CreateBunker(string bunkerID)
        {
            BunkerButtons(NetworkedPlayer.localPlayer!.team, false);
            SendMessage("CreateBunker:" + bunkerID);
        }

        /// <summary>
        /// Input Loop for the multiplayer game
        /// </summary>
        private void InputLoop()
        {
            if (InputHandler.keysDown.Contains(Key.Escape))
            {
                if (!heldEscape)
                    PauseUnpause(!Paused);
                heldEscape = true;
            }
            else
                heldEscape = false;
        }

        /// <summary>
        /// Opens / Closes the pause menu
        /// </summary>
        /// <param name="pause"> Whether to pause or unpause </param>
        private void PauseUnpause(bool pause)
        {
            Paused = pause;
            if (pause)
                pauseMenu = new OnlinePauseMenu();
            else
                pauseMenu?.Dispose();
        }

        /// <summary>
        /// Interprets incoming messages from the "Game-Server"
        /// </summary>
        /// <param name="msg"> Message to interpret </param>
        protected override void InterpretMessage(string msg)
        {
            string senderUsername = msg.Split('$')[0];

            if (msg.Contains("InitiatePlayer"))
            {
                string positionAndTeam = msg.Split('(')[1].Split(')')[0];
                int pos = int.Parse(positionAndTeam.Split(',')[0]);
                char team = positionAndTeam.Split(',')[1][0];
                float yPos;
                if (NetworkedPlayer.localPlayer != null)
                {
                    if (NetworkedPlayer.localPlayer.team == 'A')
                        yPos = team == 'A' ? MainWindow.referenceSize.Y * 0.735f : 30;
                    else
                        yPos = team == 'A' ? 30 : MainWindow.referenceSize.Y * 0.735f;
                }
                else
                    yPos = MainWindow.referenceSize.Y * 0.735f;

                if (senderUsername == MainWindow.username) // if local player
                {
                    teamALabel!.transform.Position = new Vector2(30, team == 'A' ? MainWindow.referenceSize.Y * 0.735f + 16 : 14);
                    teamBLabel!.transform.Position = new Vector2(30, team == 'B' ? MainWindow.referenceSize.Y * 0.735f + 16 : 14);

                    new NetworkedPlayer(new Vector2(pos, yPos), senderUsername, team, SendMessage);
                    NetworkedBunker.InitiateEmptyBunkers();
                }
                else if (!NetworkedPlayer.currentPlayers.ContainsKey(senderUsername))
                    new NetworkedPlayer(new Vector2(pos, yPos), senderUsername, team);
            }


            if (msg.Contains("AlreadyConnected"))
            {
                Dispose();
                new GameMultiplayerMenu("Username already connected!");
            }
            else if (msg.Contains("BulletHit"))
            {
                string HitObject = msg.Split(':')[1].Split('(')[0];
                string HitObjectDetails = msg.Split('(')[1].Split(')')[0];

                if (HitObject.Contains("Player"))
                {
                    if (HitObjectDetails == MainWindow.username)
                        NetworkedPlayer.currentPlayers[HitObjectDetails].LocalKill();
                    else
                        NetworkedPlayer.currentPlayers[HitObjectDetails].OnlineKill();
                }
                else if (HitObject.Contains("BunkerPart"))
                {
                    int BunkerID = int.Parse(HitObjectDetails.Split(',')[0]);
                    int part = int.Parse(HitObjectDetails.Split(',')[1]);
                    NetworkedBunker.Bunkers[BunkerID].parts[part].Hit();
                }
            }
            else if (msg.Contains("CreateBunker"))
            {
                int BunkerID = int.Parse(msg.Split(':')[1]);
                NetworkedBunker.Bunkers[BunkerID].Reset();
                foreach (NetworkedBunkerPart p in NetworkedBunker.Bunkers[BunkerID].parts)
                    p.Hit();

                BunkerButtons(0 <= BunkerID && BunkerID <= 3 ? 'A' : 'B', false);
            }

            if (senderUsername == MainWindow.username) return; // prevent message loopback

            if (msg.Contains("PlayerPosition"))
            {
                int.TryParse(msg.Split(':')[1], out int x);
                Transform t = NetworkedPlayer.currentPlayers[senderUsername].transform;
                t.Position = new Vector2(x, t.Position.Y);
            }
            else if (msg.Contains("GiveTeamBunker"))
            {
                char team = msg.Split(':')[1][0];
                if (team == NetworkedPlayer.localPlayer!.team)
                    BunkerButtons(team, true);
            }
            else if (msg.Contains("RevokeTeamBunker"))
            {
                char team = msg.Split(':')[1][0];
                if (team == NetworkedPlayer.localPlayer!.team)
                    BunkerButtons(team, false);
            }
            else if (msg.Contains("Score"))
            {
                string scoreDetails = msg.Split('(')[1].Split(')')[0];
                char team = scoreDetails.Split(',')[0][0];
                int score = int.Parse(scoreDetails.Split(',')[1]);
                if (team == 'A')
                    SetScoreA(score);
                else
                    SetScoreB(score);
            }
            else if (msg.Contains("InitiateBunkerPart"))
            {
                string partDetails = msg.Split("(")[1].Split(')')[0];
                int BunkerID = int.Parse(partDetails.Split(',')[0]);
                int part = int.Parse(partDetails.Split(',')[1]);
                int stage = int.Parse(partDetails.Split(',')[2]);
                if (stage != 0)
                {
                    NetworkedBunker.Bunkers[BunkerID].parts[part].ResetPart();
                    for (int i = 0; i < stage; i++)
                        NetworkedBunker.Bunkers[BunkerID].parts[part].Hit();
                }
            }
            else if (msg.Contains("BulletExplosion"))
            {
                NetworkedPlayer.currentPlayers[senderUsername].myBullet?.BulletExplosion();
                NetworkedPlayer.currentPlayers[senderUsername].myBullet = null;
            }
            else if (msg.Contains("Left"))
                NetworkedPlayer.currentPlayers[senderUsername].Dispose();
            else if (msg.Contains("InitiateBullet"))
                new NetworkedBullet(senderUsername);
        }

        /// <summary>
        /// Occurs when the connection was not shut down gracefully
        /// </summary>
        protected override void OnError()
        {
            Dispose();
            new GameMultiplayerMenu("Connection lost");
        }

        /// <summary>
        /// Disposes the current <see cref="NetworkedGameClient"/>
        /// </summary>
        public void Dispose()
        {
            StopClient();
            Paused = false;
            InputHandler.RemoveInputLoop(InputLoop);
            Wall.DisposeAll();
            Bullet.DisposeAll();
            NetworkedPlayer.localPlayer = null;
            NetworkedPlayer.DisposeAll();
            NetworkedBunker.DisposeAll();
            teamALabel?.Dispose();
            teamBLabel?.Dispose();
            bunkerButton1?.Dispose();
            bunkerButton2?.Dispose();
            bunkerButton3?.Dispose();
            bunkerButton4?.Dispose();
            bunkerButton5?.Dispose();
            bunkerButton6?.Dispose();
            bunkerButton7?.Dispose();
            bunkerButton8?.Dispose();
        }
    }
}