using GameWindow.Components.GameComponents;
using GameWindow.Components.GameComponents.NetworkedComponents;
using GameWindow.Components.Miscellaneous;
using GameWindow.Systems.Networking;
using System.Numerics;

namespace GameWindow.Components.Initializers
{
    internal class MultiplayerGameClient : NetworkClient
    {
        public MultiplayerGameClient(string ip, int port, string username) : base()
        {
            if (ConnectToAddress(ip, port))
            {
                SendMessage(username);
                BeginRead();

                Wall.Ceiling = new Wall(new Vector2(256, 5), new Vector2(256 / 2, 0), @"Resources\Images\Pixels\Red.png");
                Wall.Floor = new Wall(new Vector2(256, 5), new Vector2(256 / 2, 256), @"Resources\Images\Pixels\Green.png");
                Wall.LeftWall = new Wall(new Vector2(5, 256), new Vector2(0, 256 / 2));
                Wall.RightWall = new Wall(new Vector2(5, 256), new Vector2(256, 256 / 2));
            }
            else
                GameInitializers.StartGameMenu(username);
        }
        protected override void DecodeMessage(string msg)
        {
            string gotNick = msg.Split('$')[0];

            if (msg.Contains("INITIATE PLAYER"))
            {
                if (gotNick == GameInitializers.username) // if local player
                    new NetworkedPlayer(new Vector2(100, 200), gotNick, SendMessage);
                else if (!NetworkedPlayer.currentPlayers.ContainsKey(gotNick))
                    new NetworkedPlayer(new Vector2(100, 50), gotNick);
            }

            if (gotNick == GameInitializers.username) return; // prevent message loopback

            if (msg.Contains("LEFT"))
            {
                NetworkedPlayer.currentPlayers[gotNick].Dispose();
                NetworkedPlayer.currentPlayers.Remove(gotNick);
            }
            else if (msg.Contains("PLAYER POS"))
            {
                int.TryParse(msg.Split(':')[1], out int x);
                Transform t = NetworkedPlayer.currentPlayers[gotNick].transform;
                t.Position = new Vector2(x, t.Position.Y);
            }
            else if (msg.Contains("INITIATE BULLET"))
            {
                new NetworkedBullet(gotNick);
            }
            else if (msg.Contains("BULLET EXPLOSION"))
            {
                NetworkedPlayer.currentPlayers[gotNick].myBullet?.BulletExplosion();
            }
            else if (msg.Contains("BULLET HIT"))
            {
                string hitName = msg.Split(":")[1];
                if (hitName == GameInitializers.username)
                    NetworkedPlayer.currentPlayers[hitName].LocalKill();
                else
                    NetworkedPlayer.currentPlayers[hitName].OnlineKill();
            }
        }
    }
}