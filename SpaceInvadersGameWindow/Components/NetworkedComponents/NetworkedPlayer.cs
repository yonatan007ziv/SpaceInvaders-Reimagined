using GameWindow.Components.GameComponents;
using GameWindow.Components.Miscellaneous;
using GameWindow.Components.UIElements;
using GameWindow.Factories;
using GameWindow.Systems;
using System.Collections.Generic;
using System.Numerics;
using System.Threading.Tasks;

namespace GameWindow.Components.NetworkedComponents
{
    /// <summary>
    /// A class implementing a networked player
    /// </summary>
    internal class NetworkedPlayer
    {
        public static Dictionary<string, NetworkedPlayer> currentPlayers = new Dictionary<string, NetworkedPlayer>();
        public static NetworkedPlayer? localPlayer;

        public bool invincible = false;
        public DelegatesActions.ActionString? SendMessage;
        public NetworkedBullet? myBullet;
        public Transform transform;
        public Collider col;
        private Sprite sprite;
        public NetworkedPlayerController? controller;
        private CustomLabel nameTag;
        public string username;
        public char team;

        /// <summary>
        /// Builds a networked player
        /// </summary>
        /// <param name="pos"> A <see cref="Vector2"/> representing the player's position </param>
        /// <param name="username"> The username of the player </param>
        /// <param name="team"> Team of the player </param>
        /// <param name="SendMessage"> Delegate used to send messages to the "Game-Server" </param>
        public NetworkedPlayer(Vector2 pos, string username, char team, DelegatesActions.ActionString SendMessage) // local
        {
            currentPlayers.Add(username, this);

            localPlayer = this;
            this.SendMessage = SendMessage;
            this.username = username;
            this.team = team;

            transform = new Transform(new Vector2(13, 8), pos);

            CollisionLayer myLayer = team == 'A' ? CollisionLayer.TeamA : CollisionLayer.TeamB;
            col = new Collider(transform, this, myLayer);
            col.AddIgnoreLayer(myLayer);

            controller = new NetworkedPlayerController(this, SendMessage);

            sprite = UIElementFactory.CreateSprite(transform, Image.Player);
            nameTag = UIElementFactory.CreateLabel(transform, username, System.Windows.Media.Colors.Black);

            // Suppressing the "Null When Leaving a Constructor" warning
            sprite!.ToString();
            nameTag!.ToString();
        }

        /// <summary>
        /// Builds a networked player
        /// </summary>
        /// <param name="pos"> A <see cref="Vector2"/> representing the player's position </param>
        /// <param name="username"> The username of the player </param>
        /// <param name="team"> Team of the player </param>
        public NetworkedPlayer(Vector2 pos, string username, char team) // online
        {
            currentPlayers.Add(username, this);
            this.username = username;
            this.team = team;

            transform = new Transform(new Vector2(13, 8), pos);

            CollisionLayer myLayer = team == 'A' ? CollisionLayer.TeamA : CollisionLayer.TeamB;
            col = new Collider(transform, this, myLayer);
            col.AddIgnoreLayer(myLayer);

            Image image = localPlayer!.team == team ? Image.Player : Image.OpponentPlayer;
            sprite = UIElementFactory.CreateSprite(transform, image);
            nameTag = UIElementFactory.CreateLabel(transform, username, System.Windows.Media.Colors.Black);

            // Suppressing the "Null When Leaving a Constructor" warning
            sprite!.ToString();
            nameTag!.ToString();
        }

        /// <summary>
        /// Local player's kill procedure
        /// </summary>
        public async void LocalKill()
        {
            if (invincible) return;

            controller!.disabled = true;
            invincible = true;

            SoundManager.PlaySound(Sound.PlayerDeath);

            transform.Scale = new Vector2(16, 8);

            for (int i = 0; i < 12; i++)
            {

                sprite.ChangeImage(i % 2 == 0 ? Image.PlayerDeath1 : Image.PlayerDeath2);
                await Task.Delay(1000 / 10);
            }

            Respawn(isOpponent: false);
            controller!.disabled = false;
            await InvincibilityAnimation();
            invincible = false;
        }

        /// <summary>
        /// Online player's kill procedure
        /// </summary>
        public async void OnlineKill()
        {
            if (invincible) return;
            invincible = true;

            SoundManager.PlaySound(Sound.PlayerDeath);

            transform.Scale = new Vector2(16, 8);

            bool ally = localPlayer!.team == team;

            for (int i = 0; i < 12; i++)
            {
                sprite.ChangeImage(i % 2 == 0 ? (ally ? Image.PlayerDeath1 : Image.OpponentPlayerDeath1) : (ally ? Image.PlayerDeath2 : Image.OpponentPlayerDeath2));
                await Task.Delay(1000 / 10);
            }

            Respawn(isOpponent: localPlayer!.team != team);
            await InvincibilityAnimation();
            invincible = false;
        }

        /// <summary>
        /// Respaws the current <see cref="NetworkedPlayer"/>
        /// </summary>
        /// <param name="isOpponent"> Is sprite flipped or not </param>
        private void Respawn(bool isOpponent)
        {
            transform.Scale = new Vector2(13, 8);
            sprite.ChangeImage(isOpponent ? Image.OpponentPlayer : Image.Player);
        }

        /// <summary>
        /// Run invincibility for <see cref="Player.INVINCIBILITY_PERIOD"/> seconds
        /// </summary>
        /// <returns> A task representing the asynchronous operation of the invincibility animation </returns>
        private async Task InvincibilityAnimation()
        {
            for (int i = 0; i < Player.INVINCIBILITY_PERIOD * 10; i++)
            {
                sprite.Visible(i % 2 == 0);
                await Task.Delay(1000 / 10);
            }
            sprite.Visible(true);
        }

        /// <summary>
        /// Disposes the current <see cref="NetworkedPlayer"/>
        /// </summary>
        public void Dispose()
        {
            currentPlayers.Remove(username);
            controller?.Dispose();
            transform.Dispose();
            col.Dispose();
            sprite.Dispose();
            nameTag.Dispose();
        }

        /// <summary>
        /// Disposes all <see cref="NetworkedPlayer"/>
        /// </summary>
        public static void DisposeAll()
        {
            foreach (NetworkedPlayer p in currentPlayers.Values)
                p.Dispose();
        }
    }
}