using GameWindow.Components.PhysicsEngine.Collider;
using GameWindow.Systems;
using System.Numerics;
using System.Threading.Tasks;

namespace GameWindow.Components.GameComponents
{
    internal class PlayerBullet : Bullet
    {
        public static PlayerBullet? instance;
        public PlayerBullet(Vector2 pos, int speed) : base(pos, speed, BulletTypes.Normal, Collider.Layers.PlayerBullet)
        {
            instance = this;

            SoundManager.PlaySound(SoundManager.Sounds.BulletInitiated);

            col.IgnoreLayer(Collider.Layers.Player);
            col.IgnoreLayer(Collider.Layers.PlayerBullet);

            BulletLoop();
        }
        private async void BulletLoop()
        {
            while (col.TouchingCollider() == null && !bulletHit)
            {
                transform.Position += new Vector2(0, bulletSpeed);
                await Task.Delay(1000 / MainWindow.TARGET_FPS);
            }
            if (bulletHit)
            {
                SoundManager.PlaySound(SoundManager.Sounds.BulletExplosion);
                instance = null;
                return;
            }


            Collider touching = col.TouchingCollider()!;
            if (touching.parent is Invader inv)
                inv.Kill();
            else if (touching.parent is BunkerPart bunker)
                bunker.Hit();
            else if (touching.parent is InvaderBullet bullet)
                bullet.BulletExplosion();

            SoundManager.PlaySound(SoundManager.Sounds.BulletExplosion);
            BulletExplosion();
            instance = null;
        }
    }
}