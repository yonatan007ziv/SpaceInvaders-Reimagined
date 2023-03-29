using GameWindow.Components.PhysicsEngine.Collider;
using GameWindow;
using GameWindow.Components.GameComponents;
using System;
using System.Numerics;
using System.Threading.Tasks;
using GameWindow.Components.GameComponents.Bunker;

namespace GameWindow.Components.GameComponents
{
    internal class InvaderBullet : Bullet
    {
        private static Random random = new Random();
        public InvaderBullet(Vector2 pos, int dir) : base(pos, dir, (BulletTypes)random.Next(0, 3), Collider.Layers.InvaderBullet)
        {
            col.IgnoreLayer(Collider.Layers.Invader);
            col.IgnoreLayer(Collider.Layers.PlayerBullet);
            col.IgnoreLayer(Collider.Layers.InvaderBullet);

            BulletLoop();
        }
        private async void BulletLoop()
        {
            Vector2 SpeedVector = new Vector2(0, bulletSpeed);
            while (col.TouchingCollider() == null)
            {
                NextClip();
                transform.Position += SpeedVector;
                await Task.Delay(1000 / MainWindow.TARGET_FPS);
            }

            Collider touching = col.TouchingCollider()!;
            if (touching.parent is Player player)
                player.Kill();
            if (touching.parent is BunkerPart bunker)
                bunker.Hit();

            BulletExplosion();
        }
    }
}