using GameWindow.Components.PhysicsEngine.Collider;
using System;
using System.Numerics;
using System.Threading.Tasks;

namespace GameWindow.Components.GameComponents
{
    internal class InvaderBullet : Bullet
    {
        private static Random random = new Random();
        public InvaderBullet(Vector2 pos, int speed) : base(pos, speed, (BulletTypes)random.Next(0, 3), Collider.Layers.InvaderBullet)
        {
            col.IgnoreLayer(Collider.Layers.Invader);
            col.IgnoreLayer(Collider.Layers.InvaderBullet);

            BulletLoop();
        }
        private async void BulletLoop()
        {
            while (col.TouchingCollider() == null && !bulletHit)
            {
                NextClip();
                transform.Position += new Vector2(0, bulletSpeed);
                await Task.Delay(1000 / MainWindow.TARGET_FPS);
            }
            if (bulletHit)
                return;

            Collider touching = col.TouchingCollider()!;
            if (touching.parent is Player player)
                player.Kill();
            else if (touching.parent is BunkerPart bunker)
                bunker.Hit();
            else if (touching.parent is PlayerBullet bullet)
                bullet.BulletExplosion();

            BulletExplosion();
        }
    }
}