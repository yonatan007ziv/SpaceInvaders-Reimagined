using GameWindow.Components.GameComponents;
using GameWindow.Components.Miscellaneous;
using GameWindow.Components.UIElements;
using GameWindow.Systems;
using System.Collections.Generic;
using System.Numerics;
using System.Threading.Tasks;
using System.Windows;
using static GameWindow.Components.Miscellaneous.Delegates;

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
        /// <param name="sendMessage"> Delegate used to send messages to the "Game-Server" </param>
        public NetworkedPlayer(Vector2 pos, string username, char team, ActionString sendMessage) // local
        {
            currentPlayers.Add(username, this);

            localPlayer = this;
            this.username = username;
            this.team = team;

            transform = new Transform(new Vector2(13, 8), pos);

            CollisionLayer myLayer = team == 'A' ? CollisionLayer.TeamA : CollisionLayer.TeamB;
            col = new Collider(transform, this, myLayer);
            col.AddIgnoreLayer(myLayer);

            controller = new NetworkedPlayerController(this, sendMessage);

            Application.Current.Dispatcher.Invoke(() =>
            { // UI Objects need to be created in an STA thread
                sprite = new Sprite(transform, @"Resources\Images\Player\Player.png");
                nameTag = new CustomLabel(transform, username, System.Windows.Media.Colors.Black);
            });

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

            string path = localPlayer!.team == team ? @"Resources\Images\Player\Player.png" : @"Resources\Images\Player\OpponentPlayer.png";
            Application.Current.Dispatcher.Invoke(() =>
            { // UI Objects need to be created in an STA thread

                sprite = new Sprite(transform, path);
                nameTag = new CustomLabel(transform, username, System.Windows.Media.Colors.Black);
            });

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

                sprite.ChangeImage(@$"Resources\Images\Player\PlayerDeath{i % 2 + 1}.png");
                await Task.Delay(1000 / 10);
            }

            Respawn(isOpponent: false);
            await InvincibilityAnimation();
            invincible = false;
            controller!.disabled = false;
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

            string path = localPlayer!.team == team ? @"Resources\Images\Player\PlayerDeath" : @"Resources\Images\Player\OpponentPlayerDeath";

            for (int i = 0; i < 12; i++)
            {
                sprite.ChangeImage($"{path + (i % 2 + 1)}.png");
                await Task.Delay(1000 / 10);
            }

            Respawn(isOpponent: localPlayer!.team != team);
            await InvincibilityAnimation();
        }

        /// <summary>
        /// Respaws the current <see cref="NetworkedPlayer"/>
        /// </summary>
        /// <param name="isOpponent"> Is sprite flipped or not </param>
        private void Respawn(bool isOpponent)
        {
            transform.Scale = new Vector2(13, 8);
            sprite.ChangeImage(@"Resources\Images\Player\" + (isOpponent ? "Opponent" : "") + "Player.png");
        }

        /// <summary>
        /// Run invincibility for <see cref="Player.INVINCIBILITY_PERIOD"/> seconds
        /// </summary>
        /// <returns> A task representing the asynchronous operation of the invincibility animation </returns>
        private async Task InvincibilityAnimation()
        {
            for (int i = 0; i < Player.INVINCIBILITY_PERIOD; i++)
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