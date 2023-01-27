using SpaceInvaders.Components.Controllers;
using SpaceInvaders.Components.Renderer;
using SpaceInvaders.Components.Miscellaneous;
using SpaceInvaders.Components.PhysicsEngine.Collider;
using System.Numerics;
using SpaceInvadersGameWindow;
using System.Threading.Tasks;
using SpaceInvaders.Systems;
using System;

namespace SpaceInvaders.Components.GameComponents
{
    internal class Player
    {
        private int timesToLive = 3;

        public Transform transform;
        private Collider col;
        private SpriteRenderer sR;
        private CharacterController controller;
        public Player(Vector2 pos)
        {
            transform = new Transform(new Vector2(13, 8), pos);
            col = new Collider(transform, this);
            sR = new SpriteRenderer(transform, @"Resources\RawFiles\Images\Player\Player.png");

            controller = new CharacterController(transform, col);
        }
        public async void Kill()
        {
            controller.Dispose();

            SoundManager.PlaySound(@"Resources\RawFiles\Sounds\PlayerDeath.wav", () => Respawn());
            for (int i = 0; i < 12; i++)
            {
                sR.Source = SpriteRenderer.BitmapImageMaker(@$"Resources\RawFiles\Images\Player\PlayerDeath{i % 2 + 1}.png");
                await Task.Delay(1000 / 10);
            }
            Respawn();
        }
        private void Respawn()
        {
            timesToLive--;
            col.Dispose();
            sR.Dispose();
            transform.Dispose();

            transform = new Transform(new Vector2(13, 8), transform.position);
            col = new Collider(transform, this);
            sR = new SpriteRenderer(transform, @"Resources\RawFiles\Images\Player\Player.png");

            controller = new CharacterController(transform, col);
        }
    }
}