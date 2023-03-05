using GameWindow.Components.PhysicsEngine.Collider;
using GameWindow.Systems;
using GameWindow;
using GameWindow.Components.GameComponents;
using System.Numerics;
using System.Threading.Tasks;

namespace GameWindow.Components.GameComponents
{
    internal class PlayerBullet : Bullet
    {
        public static PlayerBullet? instance;
        public PlayerBullet(Vector2 pos, int dir) : base(pos, dir, 3)
        {
            instance = this;
            bulletSpeed *= 1.75f;

            //SoundManager.PlaySound(@"Resources\Sounds\Shoot.wav");

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
            if(col.parent is Invader inv)
                inv.Death();

            BulletExplosion();
            instance = null;
        }
    }
}