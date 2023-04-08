using GameWindow.Components.Miscellaneous;
using GameWindow.Components.UIElements;
using GameWindow.Systems;
using System.Collections.Generic;
using System.Numerics;
using System.Threading.Tasks;
using System.Windows;
using static GameWindow.Components.Miscellaneous.Delegates;

namespace GameWindow.Components.GameComponents.NetworkedComponents
{
    internal class NetworkedPlayer
    {
        public static Dictionary<string, NetworkedPlayer> currentPlayers = new Dictionary<string, NetworkedPlayer>();

        public bool invincible = false;
        public NetworkedBullet? myBullet = null;
        public Transform transform;
        private Collider col;
        private Sprite sprite;
        public NetworkedPlayerController? controller;
        private CustomLabel nameTag;
        public string username;

        public NetworkedPlayer(Vector2 pos, string username, ActionString sendMessage) // local
        {
            currentPlayers.Add(username, this);

            this.username = username;

            transform = new Transform(new Vector2(13, 8), pos);
            col = new Collider(transform, this, CollisionLayer.Player);
            controller = new NetworkedPlayerController(this, transform, col, sendMessage);

            Application.Current.Dispatcher.Invoke(() =>
            { // UI Objects need to be created in an STA thread
                sprite = new Sprite(transform, @"Resources\Images\Player\Player.png");
                nameTag = new CustomLabel(transform, username, System.Windows.Media.Colors.Purple);
            });

            // Suppressing the "Null When Leaving a Constructor" warning
            sprite!.ToString();
            nameTag!.ToString();
        }

        public NetworkedPlayer(Vector2 pos, string username) // online
        {
            currentPlayers.Add(username, this);
            this.username = username;

            transform = new Transform(new Vector2(13, 8), pos);
            col = new Collider(transform, this, CollisionLayer.OnlinePlayer);

            Application.Current.Dispatcher.Invoke(() =>
            { // UI Objects need to be created in an STA thread
                sprite = new Sprite(transform, @"Resources\Images\Player\OpponentPlayer.png");
                nameTag = new CustomLabel(transform, username, System.Windows.Media.Colors.Purple);
            });

            // Suppressing the "Null When Leaving a Constructor" warning
            sprite!.ToString();
            nameTag!.ToString();
        }
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
            Invincibility();
            controller!.disabled = false;
        }
        public async void OnlineKill()
        {
            if (invincible) return;
            invincible = true;

            SoundManager.PlaySound(Sound.PlayerDeath);

            transform.Scale = new Vector2(16, 8);

            for (int i = 0; i < 12; i++)
            {
                sprite.ChangeImage(@$"Resources\Images\Player\OpponentPlayerDeath{i % 2 + 1}.png");
                await Task.Delay(1000 / 10);
            }

            Respawn(isOpponent: true);
            Invincibility();
        }
        private void Respawn(bool isOpponent)
        {
            transform.Scale = new Vector2(13, 8);
            sprite.ChangeImage(@"Resources\Images\Player\" + (isOpponent ? "Opponent" : "") + "Player.png");
        }
        private async void Invincibility()
        {
            for (int i = 0; i < 13; i++)
            {
                sprite.Visible(i % 2 == 0);
                await Task.Delay(1000 / 10);
            }
            invincible = false;
        }
        public void Dispose()
        {
            controller?.Dispose();
            transform.Dispose();
            col.Dispose();
            sprite.Dispose();
            nameTag.Dispose();
        }
    }
}