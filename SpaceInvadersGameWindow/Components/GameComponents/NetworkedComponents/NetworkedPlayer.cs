using SpaceInvaders.Components.Miscellaneous;
using SpaceInvaders.Components.PhysicsEngine.Collider;
using SpaceInvadersGameWindow.Components.UIElements;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Numerics;
using System.Windows;

namespace SpaceInvadersGameWindow.Components.GameComponents.NetworkedComponents
{
    internal class NetworkedPlayer
    {
        public static Dictionary<string, NetworkedPlayer> currentPlayers = new Dictionary<string, NetworkedPlayer>();

        public Transform transform;
        private Collider col;
        private Sprite sprite;
        private NetworkedCharacterController? characterController;
        private CustomLabel nameTag;
        public NetworkedPlayer(Vector2 pos, string nickname, NetworkStream ns) // local
        {
            currentPlayers.Add(nickname, this);

            transform = new Transform(new Vector2(13, 8), pos);
            col = new Collider(transform, this);
            characterController = new NetworkedCharacterController(transform, col, ns);

            Application.Current.Dispatcher.Invoke(() =>
            {
                sprite = new Sprite(transform, @"Resources\RawFiles\Images\Player\Player.png");
                nameTag = new CustomLabel(transform, nickname, System.Windows.Media.Colors.Purple);
            });
        }

        public NetworkedPlayer(Vector2 pos, string nickname) // online
        {
            if (currentPlayers.ContainsKey(nickname)) return;

            currentPlayers.Add(nickname, this);

            transform = new Transform(new Vector2(13, 8), pos);
            col = new Collider(transform, this);

            Application.Current.Dispatcher.Invoke(() =>
            {
                sprite = new Sprite(transform, @"Resources\RawFiles\Images\Player\OpponentPlayer.png");
                nameTag = new CustomLabel(transform, nickname, System.Windows.Media.Colors.Purple);
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