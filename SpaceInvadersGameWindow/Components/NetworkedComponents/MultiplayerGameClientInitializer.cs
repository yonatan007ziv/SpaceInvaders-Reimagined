using GameWindow.Components.GameComponents;
using GameWindow.Components.Initializers;
using GameWindow.Components.Miscellaneous;
using GameWindow.Components.Pages;
using GameWindow.Components.UIElements;
using GameWindow.Systems;
using GameWindow.Systems.Networking;
using System;
using System.Numerics;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace GameWindow.Components.NetworkedComponents
{
    internal class MultiplayerGameClientInitializer : NetworkClient
    {
        public static MultiplayerGameClientInitializer? instance;

        public MultiplayerGameClientInitializer(string ip, int port, string username) : base()
        {
            instance = this;
            if (ConnectToAddress(ip, port))
            {
                SendMessage(username);
                BeginRead();

                bunkerButton1.Visible(false);
                bunkerButton2.Visible(false);
                bunkerButton3.Visible(false);
                bunkerButton4.Visible(false);
                bunkerButton5.Visible(false);
                bunkerButton6.Visible(false);
                bunkerButton7.Visible(false);
                bunkerButton8.Visible(false);

                Wall.Ceiling = new Wall(new Vector2(MainWindow.referenceSize.X, 5), new Vector2(MainWindow.referenceSize.X / 2, 5), @"Resources\Images\Pixels\Red.png");
                Wall.Floor = new Wall(new Vector2(MainWindow.referenceSize.X, 5), new Vector2(MainWindow.referenceSize.X / 2, MainWindow.referenceSize.Y * 0.735f + 25), @"Resources\Images\Pixels\Green.png");
                Wall.LeftWall = new Wall(new Vector2(5, MainWindow.referenceSize.Y), new Vector2(25, MainWindow.referenceSize.Y / 2));
                Wall.RightWall = new Wall(new Vector2(5, MainWindow.referenceSize.Y), new Vector2(MainWindow.referenceSize.X - 25, MainWindow.referenceSize.Y / 2));

                InputHandler.AddInputLoop(InputLoop);
            }
            else
                GameInitializers.StartMultiplayerGameMenu();
        }
        private static bool Paused;
        private static bool heldEscape;
        private OnlinePauseMenu pauseMenu;

        private CustomLabel teamALabel;
        private int scoreA = 0;
        private int ScoreA
        {
            get { return scoreA; }
            set { scoreA = value; teamALabel.Text = $"Team A: {value}"; }
        }

        private CustomLabel teamBLabel;
        private int scoreB = 0;
        private int ScoreB
        {
            get { return scoreB; }
            set { scoreB = value; teamBLabel.Text = $"Team B: {value}"; }
        }

        private CustomButton bunkerButton1 = new CustomButton(new Transform(new Vector2(24, 16), new Vector2(0.4f * (MainWindow.referenceSize.X / 2), 2 * MainWindow.referenceSize.Y / 3)), () => CreateBunker("0"), "", "CREATE BUNKER");
        private CustomButton bunkerButton2 = new CustomButton(new Transform(new Vector2(24, 16), new Vector2(0.8f * (MainWindow.referenceSize.X / 2), 2 * MainWindow.referenceSize.Y / 3)), () => CreateBunker("1"), "", "CREATE BUNKER");
        private CustomButton bunkerButton3 = new CustomButton(new Transform(new Vector2(24, 16), new Vector2(1.2f * (MainWindow.referenceSize.X / 2), 2 * MainWindow.referenceSize.Y / 3)), () => CreateBunker("2"), "", "CREATE BUNKER");
        private CustomButton bunkerButton4 = new CustomButton(new Transform(new Vector2(24, 16), new Vector2(1.6f * (MainWindow.referenceSize.X / 2), 2 * MainWindow.referenceSize.Y / 3)), () => CreateBunker("3"), "", "CREATE BUNKER");
        private CustomButton bunkerButton5 = new CustomButton(new Transform(new Vector2(24, 16), new Vector2(0.4f * (MainWindow.referenceSize.X / 2), 2 * MainWindow.referenceSize.Y / 3)), () => CreateBunker("4"), "", "CREATE BUNKER");
        private CustomButton bunkerButton6 = new CustomButton(new Transform(new Vector2(24, 16), new Vector2(0.8f * (MainWindow.referenceSize.X / 2), 2 * MainWindow.referenceSize.Y / 3)), () => CreateBunker("5"), "", "CREATE BUNKER");
        private CustomButton bunkerButton7 = new CustomButton(new Transform(new Vector2(24, 16), new Vector2(1.2f * (MainWindow.referenceSize.X / 2), 2 * MainWindow.referenceSize.Y / 3)), () => CreateBunker("6"), "", "CREATE BUNKER");
        private CustomButton bunkerButton8 = new CustomButton(new Transform(new Vector2(24, 16), new Vector2(1.6f * (MainWindow.referenceSize.X / 2), 2 * MainWindow.referenceSize.Y / 3)), () => CreateBunker("7"), "", "CREATE BUNKER");
        private void BunkerButtons(char team)
        {
            if (team == 'A')
            {
                bunkerButton1.Visible(true);
                bunkerButton2.Visible(true);
                bunkerButton3.Visible(true);
                bunkerButton4.Visible(true);
            }
            else
            {
                bunkerButton5.Visible(true);
                bunkerButton6.Visible(true);
                bunkerButton7.Visible(true);
                bunkerButton8.Visible(true);
            }
        }
        private static void CreateBunker(string bunkerID)
        {
            instance!.bunkerButton1.Visible(false);
            instance!.bunkerButton2.Visible(false);
            instance!.bunkerButton3.Visible(false);
            instance!.bunkerButton4.Visible(false);
            instance!.bunkerButton5.Visible(false);
            instance!.bunkerButton6.Visible(false);
            instance!.bunkerButton7.Visible(false);
            instance!.bunkerButton8.Visible(false);
            instance!.SendMessage("CREATE BUNKER:" + bunkerID);
        }
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
        private void PauseUnpause(bool pause)
        {
            Paused = pause;
            if (pause)
                pauseMenu = new OnlinePauseMenu();
            else
                pauseMenu?.Dispose();
        }
        protected override void DecodeMessage(string msg)
        {
            string senderUsername = msg.Split('$')[0];

            if (msg.Contains("INITIATE PLAYER"))
            {
                string positionsAndTeam = msg.Split('(')[1].Split(')')[0];
                int pos = int.Parse(positionsAndTeam.Split(',')[0]);
                char team = positionsAndTeam.Split(',')[1][0];
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

                if (senderUsername == GameInitializers.username) // if local player
                {
                    Application.Current.Dispatcher.Invoke(() =>
                    { // UI Objects need to be created in an STA thread
                        teamALabel = new CustomLabel(new Transform(new Vector2(50, 25), new Vector2(30, team == 'A' ? MainWindow.referenceSize.Y * 0.735f + 16 : 14)), @"Team A", System.Windows.Media.Colors.White);
                        teamBLabel = new CustomLabel(new Transform(new Vector2(50, 25), new Vector2(30, team == 'B' ? MainWindow.referenceSize.Y * 0.735f + 16 : 14)), @"Team B", System.Windows.Media.Colors.White);
                    });

                    new NetworkedPlayer(new Vector2(pos, yPos), senderUsername, team, SendMessage);
                }
                else if (!NetworkedPlayer.currentPlayers.ContainsKey(senderUsername))
                    new NetworkedPlayer(new Vector2(pos, yPos), senderUsername, team);
            }


            if (msg.Contains("BULLET HIT"))
            {
                string HitDetails = msg.Split('(')[1].Split(")")[0];
                string HitObject = HitDetails.Split(',')[0];

                if (HitObject.Contains("Player"))
                {
                    string playerName = HitDetails.Split(',')[1];
                    if (playerName == GameInitializers.username)
                        NetworkedPlayer.currentPlayers[playerName].LocalKill();
                    else
                        NetworkedPlayer.currentPlayers[playerName].OnlineKill();
                }
                else if (HitObject.Contains("BunkerPart"))
                {
                    int BunkerID = int.Parse(HitObject.Split('%')[1]);
                    int part = int.Parse(HitDetails.Split(',')[1]);
                    NetworkedBunker.Bunkers[BunkerID].parts[part].Hit();
                }
            }

            if (senderUsername == GameInitializers.username) return; // prevent message loopback

            if (msg.Contains("PLAYER POS"))
            {
                int.TryParse(msg.Split(':')[1], out int x);
                Transform t = NetworkedPlayer.currentPlayers[senderUsername].transform;
                t.Position = new Vector2(x, t.Position.Y);
            }
            else if (msg.Contains("LEFT"))
            {
                NetworkedPlayer.currentPlayers[senderUsername].Dispose();
                NetworkedPlayer.currentPlayers.Remove(senderUsername);
            }
            else if (msg.Contains("TEAM BUNKER"))
            {
                char team = msg.Split(':')[1][0];
                if (team == NetworkedPlayer.localPlayer!.team)
                    BunkerButtons(team);
            }
            else if (msg.Contains("SCORE"))
            {
                string scoreDetails = msg.Split('(')[1].Split(')')[0];
                char team = scoreDetails.Split(',')[0][0];
                int score = int.Parse(scoreDetails.Split(',')[1]);
                if (team == 'A')
                    ScoreA = score;
                else
                    ScoreB = score;
            }
            else if (msg.Contains("INITIATE BULLET"))
                new NetworkedBullet(senderUsername);
            else if (msg.Contains("INITIATE BUNKERPART"))
            {
                string partDetails = msg.Split("(")[1].Split(')')[0];
                int BunkerID = int.Parse(partDetails.Split(',')[0]);
                int part = int.Parse(partDetails.Split(',')[1]);
                int stage = int.Parse(partDetails.Split(',')[2]);

                if (NetworkedBunker.Bunkers[BunkerID] == null)
                    NetworkedBunker.Bunkers[BunkerID] = new NetworkedBunker(BunkerID, (0 <= BunkerID && BunkerID <= 3 && NetworkedPlayer.localPlayer!.team == 'B') || (4 <= BunkerID && BunkerID <= 7 && NetworkedPlayer.localPlayer!.team == 'A'));
                for (int i = 0; i < stage; i++)
                    NetworkedBunker.Bunkers[BunkerID].parts[part].Hit();
            }
            else if (msg.Contains("BULLET EXPLOSION"))
                NetworkedPlayer.currentPlayers[senderUsername].myBullet?.BulletExplosion();
        }
        public void Dispose()
        {
            StopClient();
            Wall.DisposeAll();
            NetworkedPlayer.DisposeAll();
            NetworkedBunker.DisposeAll();
            teamALabel.Dispose();
            teamBLabel.Dispose();
            InputHandler.RemoveInputLoop(InputLoop);
        }
    }
}