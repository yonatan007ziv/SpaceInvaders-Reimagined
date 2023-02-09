using SpaceInvaders.Components.GameComponents;
using SpaceInvadersGameWindow.Components.GameComponents;
using SpaceInvadersGameWindow.Components.GameComponents.NetworkedComponents;
using SpaceInvadersGameWindow.Systems.Networking;
using System.Diagnostics;
using System.Numerics;
using System.Windows.Media;

namespace SpaceInvadersGameWindow.Components.Initializers
{
    internal class MultiplayerGameInitializer : NetworkClient
    {
        public MultiplayerGameInitializer(string ip, int port, string username)
        {
            if (ConnectToAddress(ip, port))
            {
                SendMessage(username);
                BeginRead();

                Wall.Ceiling = new Wall(new Vector2(256, 5), new Vector2(256 / 2, 2.5f), @"Resources\RawFiles\Images\Pixels\Red.png");
                Wall.Floor = new Wall(new Vector2(256, 5), new Vector2(256 / 2, 224), @"Resources\RawFiles\Images\Pixels\Green.png");
                Wall.LeftWall = new Wall(new Vector2(5, 256), new Vector2(0, 256 / 2));
                Wall.RightWall = new Wall(new Vector2(5, 256), new Vector2(256 - 16, 256 / 2));
            }
        }
        protected override void DecodeMessage(string msg)
        {
            Debug.WriteLine(msg);
            string gotNick = msg.Split('$')[0];

            if (msg.Contains("INITIATE PLAYER:"))
            {
                if (gotNick == GameInitializers.username) // if local player
                    new NetworkedPlayer(new Vector2(100, 200), gotNick, client.GetStream());
                else
                    new NetworkedPlayer(new Vector2(100, 50), gotNick);
            }

            if (gotNick == GameInitializers.username) return; // prevent message loopback

            if (msg.Contains("PLAYER POS:"))
            {
                int x, y;
                try
                {
                    string coords = msg.Split(':')[1];
                    coords = coords.Substring(1, coords.Length - 2);
                    x = int.Parse(coords.Split(',')[0]); y = int.Parse(coords.Split(',')[1]);
                }
                catch { x = 0; y = 0; };

                NetworkedPlayer.currentPlayers[gotNick].transform.Position = new Vector2(x, y);
            }
            else if (msg.Contains("INITIATE BULLET:"))
            {
                int x, y;
                try
                {
                    string coords = msg.Split(':')[1];
                    coords = coords.Substring(1, coords.Length - 2);
                    x = int.Parse(coords.Split(',')[0]); y = int.Parse(coords.Split(',')[1]);
                }
                catch { x = 0; y = 0; };

                new NetworkedBullet(new Vector2(x, y));
            }
            //else if (msg.Contains("BULLET POS:"))
            //{
            //    int x, y;
            //    try
            //    {
            //        string coords = msg.Split(':')[1];
            //        coords = coords.Substring(1, coords.Length - 2);
            //        x = int.Parse(coords.Split(',')[0]); y = int.Parse(coords.Split(',')[1]);
            //    }
            //    catch { x = 0; y = 0; };
            //
            //    NetworkedBullet.NetworkedBullets[gotNick].transform.Position = new Vector2(x, y);
            //}
            //else if (msg.Contains("BULLET EXPLOSION POS:"))
            //{
            //    NetworkedBullet.NetworkedBullets[gotNick].BulletExplosion();
            //}
        }
    }
}