using SpaceInvaders.Components.Miscellaneous;
using SpaceInvaders.Components.PhysicsEngine.Collider;
using SpaceInvaders.Systems;
using SpaceInvadersGameWindow.Components.GameComponents;
using SpaceInvadersGameWindow.Components.UIElements;
using System.Numerics;
using System.Threading.Tasks;

namespace SpaceInvaders.Components.GameComponents
{
    internal class PlayerBullet : Bullet
    {
        public static PlayerBullet? instance;
        public PlayerBullet(Vector2 pos, int dir) : base(pos, dir)
        {
            instance = this;
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
                await Task.Delay(1000 / 60);
            }

            Collider col = this.col.TouchingCollider()!;
            if(col.parent is Invader)
                ((Invader)col.parent).Death();

            BulletExplosion();
            instance = null;
        }
    }
}