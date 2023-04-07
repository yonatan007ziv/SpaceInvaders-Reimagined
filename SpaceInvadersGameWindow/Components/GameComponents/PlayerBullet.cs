using GameWindow.Components.Miscellaneous;
using GameWindow.Systems;
using System.Numerics;
using System.Threading.Tasks;

namespace GameWindow.Components.GameComponents
{
    internal class PlayerBullet : Bullet
    {
        public const int PLAYER_BULLET_SPEED = 5;
        public static PlayerBullet? instance;
        public PlayerBullet(Vector2 pos) : base(pos, -PLAYER_BULLET_SPEED, BulletTypes.Normal, Collider.Layers.PlayerBullet)
        {
            instance = this;

            SoundManager.PlaySound(Sounds.BulletInitiated);

            col.IgnoreLayer(Collider.Layers.Player);
            col.IgnoreLayer(Collider.Layers.PlayerBullet);

            BulletLoop();
        }
        private async void BulletLoop()
        {
            while (col.TouchingCollider() == null && !bulletHit)
            {
                transform.Position += new Vector2(0, bulletSpeed);
                await Task.Delay(1000 / (MainWindow.TARGET_FPS * 8));
            }
            if (bulletHit)
            {
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

            BulletExplosion();
            instance = null;
        }
    }
}