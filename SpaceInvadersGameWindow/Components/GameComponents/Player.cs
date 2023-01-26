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
            transform = new Transform(new Vector2(13, 8) * MainWindow.GlobalTempZoom, pos);
            col = new Collider(this, transform.scale, transform.position);
            sR = new SpriteRenderer(@"Resources\RawFiles\Images\Player\Player.png", transform.scale, transform.position);

            transform.AddScaleDel((vec) => col.SetScale(vec));
            transform.AddScaleDel((vec) => sR.SetScale(vec));

            transform.AddPositionDel((vec) => col.SetPosition(vec));
            transform.AddPositionDel((vec) => sR.SetPosition(vec));

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
        }
        private void Respawn()
        {
            timesToLive--;
            col.Dispose();
            sR.Dispose();

            transform = new Transform(new Vector2(13, 8) * MainWindow.GlobalTempZoom, transform.position);
            col = new Collider(this, transform.scale, transform.position);
            sR = new SpriteRenderer(@"Resources\RawFiles\Images\Player\Player.png", transform.scale, transform.position);

            transform.AddScaleDel((vec) => col.SetScale(vec));
            transform.AddScaleDel((vec) => sR.SetScale(vec));

            transform.AddPositionDel((vec) => col.SetPosition(vec));
            transform.AddPositionDel((vec) => sR.SetPosition(vec));

            controller = new CharacterController(transform, col);
        }
    }
}