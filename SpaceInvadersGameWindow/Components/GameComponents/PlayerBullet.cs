using SpaceInvaders.Components.Miscellaneous;
using SpaceInvaders.Components.PhysicsEngine.Collider;
using SpaceInvaders.Components.Renderer;
using SpaceInvaders.Systems;
using SpaceInvadersGameWindow;
using System.Numerics;
using System.Threading.Tasks;

namespace SpaceInvaders.Components.GameComponents
{
    internal class PlayerBullet
    {
        public static PlayerBullet? instance;
        public Transform transform;
        private SpriteRenderer sR;
        private Collider col;
        public PlayerBullet(Vector2 pos)
        {
            instance = this;
            SoundManager.PlaySound(@"Resources\RawFiles\Sounds\Shoot.wav");

            transform = new Transform(new Vector2(1, 7), pos);
            col = new Collider(transform, this);
            sR = new SpriteRenderer(transform, @"Resources\RawFiles\Images\Bullet.png");

            BulletLoop();
        }
        private async void BulletLoop()
        {
            while (this.col.TouchingCollider() == null || this.col.TouchingCollider()!.parent is Player)
            {
                transform.AddPosY(-5);
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
            sR.Dispose();
            col.Dispose();
            transform.Dispose();
            instance = null;
        }
    }
}