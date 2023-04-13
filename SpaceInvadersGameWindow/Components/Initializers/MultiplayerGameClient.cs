using GameWindow.Components.GameComponents;
using GameWindow.Components.Miscellaneous;
using GameWindow.Components.NetworkedComponents;
using GameWindow.Components.Pages;
using GameWindow.Components.UIElements;
using GameWindow.Systems;
using GameWindow.Systems.Networking;
using System.Numerics;
using System.Windows;
using System.Windows.Input;

namespace GameWindow.Components.Initializers
{
    internal class MultiplayerGameClient : NetworkClient
    {
        public static MultiplayerGameClient? instance;

        public MultiplayerGameClient(string ip, int port, string username) : base()
        {
            instance = this;
            if (Connect(ip, port))
            {
                SendMessage(username);
                BeginRead(true);


                Wall.Ceiling = new Wall(new Vector2(MainWindow.referenceSize.X, 5), new Vector2(MainWindow.referenceSize.X / 2, 5), @"Resources\Images\Pixels\Red.png");
                Wall.Floor = new Wall(new Vector2(MainWindow.referenceSize.X, 5), new Vector2(MainWindow.referenceSize.X / 2, MainWindow.referenceSize.Y * 0.735f + 25), @"Resources\Images\Pixels\Green.png");
                Wall.LeftWall = new Wall(new Vector2(5, MainWindow.referenceSize.Y), new Vector2(25, MainWindow.referenceSize.Y / 2));
                Wall.RightWall = new Wall(new Vector2(5, MainWindow.referenceSize.Y), new Vector2(MainWindow.referenceSize.X - 25, MainWindow.referenceSize.Y / 2));

                Application.Current.Dispatcher.Invoke(() =>
                { // UI Objects need to be created in an STA thread
                    teamALabel = new CustomLabel(new Transform(new Vector2(50, 25), new Vector2(30, 14)), @"Team A", System.Windows.Media.Colors.White);
                    teamBLabel = new CustomLabel(new Transform(new Vector2(50, 25), new Vector2(30, 14)), @"Team B", System.Windows.Media.Colors.White);

                    bunkerButton1 = new CustomButton(new Transform(new Vector2(24, 16), new Vector2(0.4f * (MainWindow.referenceSize.X / 2), 2 * MainWindow.referenceSize.Y / 3)), () => CreateBunker("0"), "", "CREATE BUNKER");
                    bunkerButton2 = new CustomButton(new Transform(new Vector2(24, 16), new Vector2(0.8f * (MainWindow.referenceSize.X / 2), 2 * MainWindow.referenceSize.Y / 3)), () => CreateBunker("1"), "", "CREATE BUNKER");
                    bunkerButton3 = new CustomButton(new Transform(new Vector2(24, 16), new Vector2(1.2f * (MainWindow.referenceSize.X / 2), 2 * MainWindow.referenceSize.Y / 3)), () => CreateBunker("2"), "", "CREATE BUNKER");
                    bunkerButton4 = new CustomButton(new Transform(new Vector2(24, 16), new Vector2(1.6f * (MainWindow.referenceSize.X / 2), 2 * MainWindow.referenceSize.Y / 3)), () => CreateBunker("3"), "", "CREATE BUNKER");
                    bunkerButton5 = new CustomButton(new Transform(new Vector2(24, 16), new Vector2(0.4f * (MainWindow.referenceSize.X / 2), 2 * MainWindow.referenceSize.Y / 3)), () => CreateBunker("4"), "", "CREATE BUNKER");
                    bunkerButton6 = new CustomButton(new Transform(new Vector2(24, 16), new Vector2(0.8f * (MainWindow.referenceSize.X / 2), 2 * MainWindow.referenceSize.Y / 3)), () => CreateBunker("5"), "", "CREATE BUNKER");
                    bunkerButton7 = new CustomButton(new Transform(new Vector2(24, 16), new Vector2(1.2f * (MainWindow.referenceSize.X / 2), 2 * MainWindow.referenceSize.Y / 3)), () => CreateBunker("6"), "", "CREATE BUNKER");
                    bunkerButton8 = new CustomButton(new Transform(new Vector2(24, 16), new Vector2(1.6f * (MainWindow.referenceSize.X / 2), 2 * MainWindow.referenceSize.Y / 3)), () => CreateBunker("7"), "", "CREATE BUNKER");

                    bunkerButton1.Visible(false);
                    bunkerButton2.Visible(false);
                    bunkerButton3.Visible(false);
                    bunkerButton4.Visible(false);
                    bunkerButton5.Visible(false);
                    bunkerButton6.Visible(false);
                    bunkerButton7.Visible(false);
                    bunkerButton8.Visible(false);
                });

                InputHandler.AddInputLoop(InputLoop);
            }
            else
            {
                GameInitializers.StartMultiplayerGameMenu();
                Dispose();
            }
        }
        private static bool Paused;
        private static bool heldEscape;
        private OnlinePauseMenu? pauseMenu;

        private CustomLabel? teamALabel;
        private CustomLabel? teamBLabel;
        private void SetScoreA(int value)
        { teamALabel!.Text = $"Team A: {value}"; }
        private void SetScoreB(int value)
        { teamBLabel!.Text = $"Team B: {value}"; }

        private CustomButton? bunkerButton1;
        private CustomButton? bunkerButton2;
        private CustomButton? bunkerButton3;
        private CustomButton? bunkerButton4;
        private CustomButton? bunkerButton5;
        private CustomButton? bunkerButton6;
        private CustomButton? bunkerButton7;
        private CustomButton? bunkerButton8;
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
                if (NetworkedBunker.Bunkers[4]?.BunkerExists() ?? show)
                    bunkerButton5?.Visible(show);
                if (NetworkedBunker.Bunkers[5]?.BunkerExists() ?? show)
                    bunkerButton6?.Visible(show);
                if (NetworkedBunker.Bunkers[6]?.BunkerExists() ?? show)
                    bunkerButton7?.Visible(show);
                if (NetworkedBunker.Bunkers[7]?.BunkerExists() ?? show)
                    bunkerButton8?.Visible(show);
            }
        }
        private void CreateBunker(string bunkerID)
        {
            BunkerButtons(NetworkedPlayer.localPlayer.team, false);
            SendMessage("CreateBunker:" + bunkerID);
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

                if (senderUsername == GameInitializers.username) // if local player
                {
                    teamALabel!.transform.Position = new Vector2(30, team == 'A' ? MainWindow.referenceSize.Y * 0.735f + 16 : 14);
                    teamBLabel!.transform.Position = new Vector2(30, team == 'B' ? MainWindow.referenceSize.Y * 0.735f + 16 : 14);

                    new NetworkedPlayer(new Vector2(pos, yPos), senderUsername, team, SendMessage);
                }
                else if (!NetworkedPlayer.currentPlayers.ContainsKey(senderUsername))
                    new NetworkedPlayer(new Vector2(pos, yPos), senderUsername, team);
            }


            if (msg.Contains("BulletHit"))
            {
                string HitObject = msg.Split(':')[1].Split('(')[0];
                string HitObjectDetails = msg.Split('(')[1].Split(')')[0];

                if (HitObject.Contains("Player"))
                {
                    if (HitObjectDetails == GameInitializers.username)
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
                NetworkedBunker.Bunkers[BunkerID] = new NetworkedBunker(BunkerID,
                    0 <= BunkerID && BunkerID <= 3 && NetworkedPlayer.localPlayer!.team == 'B'
                    || 4 <= BunkerID && BunkerID <= 7 && NetworkedPlayer.localPlayer!.team == 'A');
            }

            if (senderUsername == GameInitializers.username) return; // prevent message loopback

            if (msg.Contains("PlayerPosition"))
            {
                int.TryParse(msg.Split(':')[1], out int x);
                Transform t = NetworkedPlayer.currentPlayers[senderUsername].transform;
                t.Position = new Vector2(x, t.Position.Y);
            }
            else if (msg.Contains("Left"))
            {
                NetworkedPlayer.currentPlayers[senderUsername].Dispose();
                NetworkedPlayer.currentPlayers.Remove(senderUsername);
            }
            else if (msg.Contains("TeamBunker"))
            {
                char team = msg.Split(':')[1][0];
                if (team == NetworkedPlayer.localPlayer!.team)
                    BunkerButtons(team, true);
            }
            else if (msg.Contains("DisableBunkerButtons"))
            {
                char team = msg.Split(':')[1][0];
                if (team == NetworkedPlayer.localPlayer!.team)
                    BunkerButtons(team, true);
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
            else if (msg.Contains("UpdateBunkerPart"))
            {
                string partDetails = msg.Split("(")[1].Split(')')[0];
                int BunkerID = int.Parse(partDetails.Split(',')[0]);

                int part = int.Parse(partDetails.Split(',')[1]);
                int stage = int.Parse(partDetails.Split(',')[2]);

                if (NetworkedBunker.Bunkers[BunkerID] == null)
                    NetworkedBunker.Bunkers[BunkerID] = new NetworkedBunker(BunkerID, 0 <= BunkerID && BunkerID <= 3 && NetworkedPlayer.localPlayer!.team == 'B' || 4 <= BunkerID && BunkerID <= 7 && NetworkedPlayer.localPlayer!.team == 'A');
                for (int i = 0; i < stage; i++)
                    NetworkedBunker.Bunkers[BunkerID].parts[part].Hit();
            }
            else if (msg.Contains("BulletExplosion"))
            {
                NetworkedPlayer.currentPlayers[senderUsername].myBullet?.BulletExplosion();
                NetworkedPlayer.currentPlayers[senderUsername].myBullet = null;
            }
            else if (msg.Contains("InitiateBullet"))
                new NetworkedBullet(senderUsername);
        }
        public void Dispose()
        {
            StopClient();
            InputHandler.RemoveInputLoop(InputLoop);
            Wall.DisposeAll();
            NetworkedPlayer.DisposeAll();
            NetworkedBunker.DisposeAll();
            teamALabel?.Dispose();
            teamBLabel?.Dispose();
        }
    }
}