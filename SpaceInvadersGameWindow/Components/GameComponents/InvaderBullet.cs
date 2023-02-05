using SpaceInvaders.Components.PhysicsEngine.Collider;
using SpaceInvadersGameWindow;
using SpaceInvadersGameWindow.Components.GameComponents;
using System;
using System.Numerics;
using System.Threading.Tasks;

namespace SpaceInvaders.Components.GameComponents
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
            if (col.parent is Player)
                ((Player)col.parent).Kill();

            BulletExplosion();
        }
    }
}