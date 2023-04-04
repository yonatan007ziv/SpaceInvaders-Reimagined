using GameWindow.Components.Miscellaneous;
using GameWindow.Components.PhysicsEngine.Collider;
using GameWindow.Components.UIElements;
using GameWindow.Systems;
using System.Collections.Generic;
using System.Numerics;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media.Imaging;
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
        public NetworkedCharacterController? controller;
        private CustomLabel nameTag;
        public string username;

        public NetworkedPlayer(Vector2 pos, string username, ActionString sendMessage) // local
        {
            currentPlayers.Add(username, this);

            this.username = username;

            transform = new Transform(new Vector2(13, 8), pos);
            col = new Collider(transform, this, Collider.Layers.Player);
            controller = new NetworkedCharacterController(this, transform, col, sendMessage);

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
            col = new Collider(transform, this, Collider.Layers.OnlinePlayer);

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

            SoundManager.PlaySound(SoundManager.Sounds.PlayerDeath);

            transform.Scale = new Vector2(16, 8);
            sprite.Dispose();

            for (int i = 0; i < 12; i++)
            {
                Application.Current.Dispatcher.Invoke(() => sprite = new Sprite(transform, @$"Resources\Images\Player\PlayerDeath{i % 2 + 1}.png"));
                await Task.Delay(1000 / 10);
                sprite.Dispose();
            }

            Respawn(isOpponent: false);
            Invincibility();
            controller!.disabled = false;
        }
        public async void OnlineKill()
        {
            if (invincible) return;
            invincible = true;

            SoundManager.PlaySound(SoundManager.Sounds.PlayerDeath);

            transform.Scale = new Vector2(16, 8);
            sprite.Dispose();

            for (int i = 0; i < 12; i++)
            {
                // UI Objects need to be changed in an STA thread
                Application.Current.Dispatcher.Invoke(() => sprite = new Sprite(transform, @$"Resources\Images\Player\OpponentPlayerDeath{i % 2 + 1}.png"));
                await Task.Delay(1000 / 10);
                sprite.Dispose();
            }

            Respawn(isOpponent: true);
            Invincibility();
        }
        private void Respawn(bool isOpponent)
        {
            transform.Scale = new Vector2(13, 8);
            Application.Current.Dispatcher.Invoke(() => sprite = new Sprite(transform, @"Resources\Images\Player\" + (isOpponent ? "Opponent" : "") + "Player.png"));
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