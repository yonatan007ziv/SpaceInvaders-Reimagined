using GameWindow.Components.Initializers;
using GameWindow.Components.Miscellaneous;
using GameWindow.Components.PhysicsEngine.Collider;
using GameWindow.Components.UIElements;
using GameWindow.Systems;
using System.Numerics;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;

namespace GameWindow.Components.GameComponents
{
    internal class Player
    {
        public Transform transform;
        private Collider col;
        private Sprite sprite;
        private CharacterController controller;
        private LocalGame currentGame;
        public Player(Vector2 pos, LocalGame currentGame)
        {
            this.currentGame = currentGame;

            transform = new Transform(new Vector2(13, 8), pos);
            col = new Collider(transform, this, Collider.Layers.Player);
            sprite = new Sprite(transform, @"Resources\Images\Player\Player.png");

            controller = new CharacterController(transform, col);
        }
        private bool invincible = false;
        public void StopInput()
        {
            controller.Dispose();
        }
        public void StartInput()
        {
            controller = new CharacterController(transform, col);
        }
        public async void Die()
        {
            if (invincible) return;
            if (currentGame.LivesLeft == 0)
            {
                currentGame.Lost();
                return;
            }
            currentGame.LivesLeft--;
            invincible = true;
            StopInput();

            SoundManager.PlaySound(@"Resources\Sounds\PlayerDeath.wav");

            for (int i = 0; i < 12; i++)
            {
                sprite.image.Source = Sprite.BitmapFromPath(@$"Resources\Images\Player\PlayerDeath{i % 2 + 1}.png");
                await Task.Delay(1000 / (MainWindow.TARGET_FPS / 6));
            }
            Respawn();
            Invincibility();
        }
        private void Respawn()
        {
            col.Dispose();
            sprite.Dispose();
            float x = transform.Position.X;
            transform.Dispose();

            transform = new Transform(new Vector2(13, 8), new Vector2(x, MainWindow.referenceSize.Y * 0.8f));
            col = new Collider(transform, this, Collider.Layers.Player);
            sprite = new Sprite(transform, @"Resources\Images\Player\Player.png");

            StartInput();
        }
        private async void Invincibility()
        {
            BitmapImage playerSprite = Sprite.BitmapFromPath(@"Resources\Images\Player\Player.png");
            for (int i = 0; i < 13; i++)
            {
                if (i % 2 == 0)
                    sprite.image.Source = playerSprite;
                else
                    sprite.image.Source = null;
                await Task.Delay(1000 / 10);
            }
            invincible = false;
        }
        public void Dispose()
        {
            controller?.Dispose();
            col.Dispose();
            sprite.Dispose();
            transform.Dispose();
        }
    }
}