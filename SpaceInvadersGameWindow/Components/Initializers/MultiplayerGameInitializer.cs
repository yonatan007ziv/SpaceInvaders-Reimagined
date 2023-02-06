using SpaceInvaders.Components.GameComponents;
using SpaceInvadersGameWindow.Systems.Networking;

namespace SpaceInvadersGameWindow.Components.Initializers
{
    internal class MultiplayerGameInitializer : NetworkClient
    {
        public MultiplayerGameInitializer(string ip, int port, string username)
        {
            ConnectToAddress(ip, port);
            SendMessage(username);

            new Player(new System.Numerics.Vector2(10, 250));
        }
        protected override void DecodeMessage(string msg)
        {
            
        }
    }
}