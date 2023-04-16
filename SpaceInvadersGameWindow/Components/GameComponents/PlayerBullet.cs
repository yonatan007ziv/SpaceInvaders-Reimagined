using GameWindow.Components.Miscellaneous;
using GameWindow.Components.UIElements;
using GameWindow.Systems;
using System.Numerics;
using System.Threading.Tasks;

namespace GameWindow.Components.GameComponents
{
    /// <summary>
    /// A class implementing a <see cref="Player"/> bullet
    /// </summary>
    internal class PlayerBullet : Bullet
    {
        public const int PLAYER_BULLET_SPEED = 7;

        public static PlayerBullet? instance;

        /// <summary>
        /// Builds a <see cref="PlayerBullet"/> instance
        /// </summary>
        /// <param name="pos"> A <see cref="Vector2"/> representing the bullet's position </param>
        public PlayerBullet(Vector2 pos) : base(pos, -PLAYER_BULLET_SPEED, BulletType.Normal, CollisionLayer.PlayerBullet)
        {
            if (MainWindow.username != "shifragoras")
                instance = this;

            SoundManager.PlaySound(Sound.BulletInitiated);

            col.AddIgnoreLayer(CollisionLayer.Player);
            col.AddIgnoreLayer(CollisionLayer.PlayerBullet);

            clips.Enqueue(Sprite.BitmapFromPath(@"Resources\Images\Bullet\Bullet.png"));

            BulletLoop();
        }

        /// <summary>
        /// The <see cref="PlayerBullet"/> hit logic and loop
        /// </summary>
        private async void BulletLoop()
        {
            await BulletMovementLoop();

            Collider? touching = col.TouchingCollider();

            if (touching == null)
            {
                BulletExplosion();

                await Task.Delay(250);
                instance = null;
                return;
            }

            if (touching.parent is Invader inv)
                inv.Kill();
            else if (touching.parent is BunkerPart bunker)
                bunker.Hit();
            else if (touching.parent is InvaderBullet bullet)
                bullet.BulletExplosion();

            BulletExplosion();

            await Task.Delay(250);
            instance = null;
        }
    }
}