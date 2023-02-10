using GameWindow.Components.Miscellaneous;
using GameWindow.Components.PhysicsEngine.Collider;
using GameWindow.Components.UIElements;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Numerics;
using System.Windows;

namespace GameWindow.Components.GameComponents.NetworkedComponents
{
    internal class NetworkedPlayer
    {
        public static NetworkedPlayer? localPlayer;
        public static Dictionary<string, NetworkedPlayer> currentPlayers = new Dictionary<string, NetworkedPlayer>();

        public Transform transform;
        private Collider col;
        private Sprite sprite;
        private NetworkedCharacterController? characterController;
        private CustomLabel nameTag;
        public string username;
        public NetworkedPlayer(Vector2 pos, string username, NetworkStream ns) // local
        {
            currentPlayers.Add(username, this);

            this.username = username;
            localPlayer = this;

            transform = new Transform(new Vector2(13, 8), pos);
            col = new Collider(transform, this);
            characterController = new NetworkedCharacterController(transform, col, ns);

            Application.Current.Dispatcher.Invoke(() =>
            {
                sprite = new Sprite(transform, @"Resources\Images\Player\Player.png");
                nameTag = new CustomLabel(transform, username, System.Windows.Media.Colors.Purple);
            });
        }

        public NetworkedPlayer(Vector2 pos, string username) // online
        {
            if (currentPlayers.ContainsKey(username)) return;

            currentPlayers.Add(username, this);

            this.username = username;

            transform = new Transform(new Vector2(13, 8), pos);
            col = new Collider(transform, this);

            Application.Current.Dispatcher.Invoke(() =>
            {
                sprite = new Sprite(transform, @"Resources\Images\Player\OpponentPlayer.png");
                nameTag = new CustomLabel(transform, username, System.Windows.Media.Colors.Purple);
            });
        }

        public void Dispose()
        {
            transform.Dispose();
            col.Dispose();
            sprite.Dispose();
            characterController?.Dispose();
            nameTag.Dispose();
        }
    }
}