using SpaceInvaders.Components.Miscellaneous;
using SpaceInvaders.Components.PhysicsEngine.Collider;
using SpaceInvadersGameWindow.Components.UIElements;
using System.Collections.Generic;
using System.Numerics;

namespace SpaceInvadersGameWindow.Components.GameComponents
{
    internal class OnlinePlayer
    {
        public static Dictionary<string, OnlinePlayer> currentPlayers = new Dictionary<string, OnlinePlayer>();

        public Transform transform;
        private Collider col;
        private Sprite sprite;

        public OnlinePlayer(Vector2 pos, string nickname)
        {
            currentPlayers.Add(nickname, this);

            transform = new Transform(new Vector2(13, 8), pos);
            col = new Collider(transform, this);
            sprite = new Sprite(transform, @"Resources\RawFiles\Images\Player\Player.png");
        }
    }
}
