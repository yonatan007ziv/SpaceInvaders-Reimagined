using GameWindow.Components.PhysicsEngine.Collider;
using GameWindow;
using GameWindow.Components.GameComponents;
using System;
using System.Numerics;
using System.Threading.Tasks;

namespace GameWindow.Components.GameComponents
{
    internal class InvaderBullet : Bullet
    {
        private static Random random = new Random();
        public InvaderBullet(Vector2 pos, int dir) : base(pos, dir, random.Next(0, 3))
        {
            BulletLoop();
        }
        private async void BulletLoop()
        {
            Vector2 SpeedVector = new Vector2(0, bulletSpeed);
            while (this.col.TouchingCollider() == null || this.col.TouchingCollider()!.parent is Invader || this.col.TouchingCollider()!.parent is Bullet)
            {
                NextClip();
                transform.Position += SpeedVector;
                await Task.Delay(1000 / MainWindow.TargetFPS);
            }

            Collider col = this.col.TouchingCollider()!;
            if (col.parent is Player player)
                player.Kill();

            BulletExplosion();
        }
    }
}