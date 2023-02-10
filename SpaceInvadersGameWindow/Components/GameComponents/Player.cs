using GameWindow.Components.Miscellaneous;
using GameWindow.Components.PhysicsEngine.Collider;
using GameWindow.Systems;
using GameWindow.Components.GameComponents;
using GameWindow.Components.UIElements;
using System.Numerics;
using System.Threading.Tasks;

namespace GameWindow.Components.GameComponents
{
    internal class Player
    {
        private int livesLeft = 3;

        public Transform transform;
        private Collider col;
        private Sprite sprite;
        private CharacterController controller;
        public Player(Vector2 pos)
        {
            transform = new Transform(new Vector2(13, 8), pos);
            col = new Collider(transform, this);
            sprite = new Sprite(transform, @"Resources\Images\Player\Player.png");

            controller = new CharacterController(transform, col);
        }
        private bool invincible = false;
        public async void Kill()
        {
            if (invincible) return;
            //else if (livesLeft == 0) 

            invincible = true;
            controller.Dispose();

            SoundManager.PlaySound(@"Resources\Sounds\PlayerDeath.wav");

            for (int i = 0; i < 12; i++)
            {
                sprite.image.Source = Sprite.BitmapFromPath(@$"Resources\Images\Player\PlayerDeath{i % 2 + 1}.png");
                await Task.Delay(1000 / 10);
            }
            Respawn();
            Invincibility();
        }
        private void Respawn()
        {
            livesLeft--;
            col.Dispose();
            sprite.Dispose();
            transform.Dispose();

            transform = new Transform(new Vector2(13, 8), new Vector2(50, 200));
            col = new Collider(transform, this);
            sprite = new Sprite(transform, @"Resources\Images\Player\Player.png");

            controller = new CharacterController(transform, col);
        }
        private async void Invincibility()
        {
            for (int i = 0; i < 13; i++)
            {
                if (i % 2 == 0)
                    sprite.image.Source = Sprite.BitmapFromPath(@$"Resources\Images\Player\Player.png");
                else
                    sprite.image.Source = null;
                await Task.Delay(1000 / 10);
            }
            invincible = false;
        }
    }
}