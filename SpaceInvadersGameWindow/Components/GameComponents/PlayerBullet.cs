using SpaceInvaders.Components.PhysicsEngine.Collider;
using SpaceInvaders.Systems;
using SpaceInvadersGameWindow.Components.GameComponents;
using System.Numerics;
using System.Threading.Tasks;

namespace SpaceInvaders.Components.GameComponents
{
    internal class PlayerBullet : Bullet
    {
        public static PlayerBullet? instance;
        public PlayerBullet(Vector2 pos) : base(pos)
        {
            instance = this;
            SoundManager.PlaySound(@"Resources\RawFiles\Sounds\Shoot.wav");

            BulletLoop();
        }
        private async void BulletLoop()
        {
            while (this.col.TouchingCollider() == null || this.col.TouchingCollider()!.parent is Player || this.col.TouchingCollider()!.parent is Bullet)
            {
                transform.Position += new Vector2(0, -bulletSpeed);
                await Task.Delay(1000 / 60);
            }

            Collider col = this.col.TouchingCollider()!;
            if(col.parent is Invader)
            {
                ((Invader)col.parent).Death();
            }
            Dispose();
        }


        public void Dispose()
        {
            sprite.Dispose();
            col.Dispose();
            transform.Dispose();
            instance = null;
        }
    }
}