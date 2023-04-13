using GameWindow.Components.Miscellaneous;
using GameWindow.Components.UIElements;
using GameWindow.Systems;
using System.Numerics;
using System.Threading.Tasks;

namespace GameWindow.Components.GameComponents
{
    internal class PlayerBullet : Bullet
    {
        public const int PLAYER_BULLET_SPEED = 5;

        public static PlayerBullet? instance;

        public PlayerBullet(Vector2 pos) : base(pos, -PLAYER_BULLET_SPEED, BulletType.Normal, CollisionLayer.PlayerBullet)
        {
            instance = this;

            SoundManager.PlaySound(Sound.BulletInitiated);

            col.IgnoreLayer(CollisionLayer.Player);
            col.IgnoreLayer(CollisionLayer.PlayerBullet);

            clips.Enqueue(Sprite.BitmapFromPath(@"Resources\Images\Bullet\Bullet.png"));

            _ = BulletLoop();
        }
        protected override async Task BulletLoop()
        {
            await base.BulletLoop();

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