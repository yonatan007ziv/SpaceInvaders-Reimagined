using SpaceInvaders.Components.GameComponents;
using SpaceInvadersGameWindow.Components.GameComponents;
using SpaceInvadersGameWindow.Systems.Networking;

namespace SpaceInvadersGameWindow.Components.Initializers
{
    internal class MultiplayerGameInitializer : NetworkClient
    {
        private string username;
        public MultiplayerGameInitializer(string ip, int port, string username)
        {
            this.username = username;
            ConnectToAddress(ip, port);
            SendMessage(username);
            BeginRead();
            //new Player(new System.Numerics.Vector2(10, 250));
        }
        protected override void DecodeMessage(string msg)
        {
            if (msg.Contains("INITIATE PLAYER"))
            {
                if (msg.Split(':')[1] == username)
                    new Player(new System.Numerics.Vector2(100, 150));
                else
                    new OnlinePlayer(new System.Numerics.Vector2(100, 50), msg.Split(':')[1]);
            }
        }
    }
}