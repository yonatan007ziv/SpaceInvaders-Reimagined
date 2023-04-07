using GameWindow.Components.Initializers;
using GameWindow.Components.Miscellaneous;
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
        public Collider col;
        private PlayerController controller;
        private Sprite sprite;
        private LocalGame currentGame;
        public Player(Vector2 pos, LocalGame currentGame)
        {
            this.currentGame = currentGame;

            transform = new Transform(new Vector2(13, 8), pos);
            col = new Collider(transform, this, Collider.Layers.Player);

            // UI Objects need to be created in an STA thread
            Application.Current.Dispatcher.Invoke(() => sprite = new Sprite(transform, @"Resources\Images\Player\Player.png"));

            controller = new PlayerController(this);

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
            PlayerController.Disabled = true;

            SoundManager.StopAllSounds();
            SoundManager.PlaySound(Sounds.PlayerDeath);
            transform.Scale = new Vector2(16, 8);

            LocalGame.FreezeUnfreeze(true);
            await DeathAnimation();
            LocalGame.FreezeUnfreeze(false);

            Respawn();
            await RespawnAnimation();
            invincible = false;
        }
        private async Task DeathAnimation()
        {
            for (int i = 0; i < 12; i++)
            {
                sprite.ChangeImage($@"Resources\Images\Player\PlayerDeath{i % 2 + 1}.png");
                await Task.Delay(1000 / (MainWindow.TARGET_FPS / 6));
            }
        }
        private void Respawn()
        {
            transform.Scale = new Vector2(13, 8);
            sprite.ChangeImage(@"Resources\Images\Player\Player.png");
            PlayerController.Disabled = false;
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
        public static void PauseUnpause(bool pause) => PlayerController.Disabled = pause;
    }
}