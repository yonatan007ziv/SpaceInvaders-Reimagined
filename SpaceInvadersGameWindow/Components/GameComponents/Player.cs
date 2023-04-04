using GameWindow.Components.Initializers;
using GameWindow.Components.Miscellaneous;
using GameWindow.Components.PhysicsEngine.Collider;
using GameWindow.Components.UIElements;
using GameWindow.Systems;
using System.Numerics;
using System.Threading.Tasks;
using System.Windows;

namespace GameWindow.Components.GameComponents
{
    internal class Player
    {
        public Transform transform;
        public CharacterController controller;
        private Collider col;
        private Sprite sprite;
        private LocalGame currentGame;
        public Player(Vector2 pos, LocalGame currentGame)
        {
            this.currentGame = currentGame;

            transform = new Transform(new Vector2(13, 8), pos);
            col = new Collider(transform, this, Collider.Layers.Player);

            // UI Objects need to be created in an STA thread
            Application.Current.Dispatcher.Invoke(() => sprite = new Sprite(transform, @"Resources\Images\Player\Player.png"));

            controller = new CharacterController(transform, col);

            // Suppressing the "Null When Leaving a Constructor" warning
            sprite!.ToString();
        }
        private bool invincible = false;
        public async void Kill()
        {
            if (invincible) return;
            if (--currentGame.LivesLeft == 0)
            {
                currentGame.Lost();
                return;
            }

            invincible = true;
            CharacterController.Disabled = true;

            SoundManager.PlaySound(SoundManager.Sounds.PlayerDeath);
            transform.Scale = new Vector2(16, 8);

            await DeathAnimation();
            Respawn();
            await RespawnAnimation();
            invincible = false;
        }
        private async Task DeathAnimation()
        {
            sprite.Dispose();
            for (int i = 0; i < 12; i++)
            {
                Application.Current.Dispatcher.Invoke(() => sprite = new Sprite(transform, $@"Resources\Images\Player\PlayerDeath{i % 2 + 1}.png"));
                await Task.Delay(1000 / (MainWindow.TARGET_FPS / 6));
                sprite.Dispose();
            }
        }
        private void Respawn()
        {
            sprite.Dispose();
            transform.Scale = new Vector2(13, 8);
            Application.Current.Dispatcher.Invoke(() => sprite = new Sprite(transform, @"Resources\Images\Player\Player.png"));
            CharacterController.Disabled = false;
        }
        private async Task RespawnAnimation()
        {
            for (int i = 0; i < 13; i++)
            {
                sprite.Visible(i % 2 == 0);
                await Task.Delay(1000 / 10);
            }
        }
        public void Dispose()
        {
            controller.Dispose();
            col.Dispose();
            sprite.Dispose();
            transform.Dispose();
        }

        public static void PauseUnpause(bool pause)
        {
            CharacterController.Disabled = pause;
        }
    }
}