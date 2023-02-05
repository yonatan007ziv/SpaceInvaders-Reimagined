using SpaceInvaders.Components.PhysicsEngine.Collider;
using SpaceInvaders.Systems;
using SpaceInvadersGameWindow;
using SpaceInvadersGameWindow.Components.GameComponents;
using System.Numerics;
using System.Threading.Tasks;

namespace SpaceInvaders.Components.GameComponents
{
    internal class PlayerBullet : Bullet
    {
        public static PlayerBullet? instance;
        public PlayerBullet(Vector2 pos, int dir) : base(pos, dir, 3)
        {
            instance = this;
            bulletSpeed *= 1.75f;

            SoundManager.PlaySound(@"Resources\RawFiles\Sounds\Shoot.wav");

            BulletLoop();
        }
        private async void BulletLoop()
        {
            Vector2 SpeedVector = new Vector2(0, bulletSpeed);
            while (this.col.TouchingCollider() == null || this.col.TouchingCollider()!.parent is Player || this.col.TouchingCollider()!.parent is Bullet)
            {
                NextClip();
                transform.Position += SpeedVector;
                await Task.Delay(1000 / MainWindow.TargetFPS);
            }

            Collider col = this.col.TouchingCollider()!;
            if(col.parent is Invader)
                ((Invader)col.parent).Death();

            BulletExplosion();
            instance = null;
        }
    }
}