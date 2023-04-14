using GameWindow.Components.Miscellaneous;
using GameWindow.Components.UIElements;
using System;
using System.Numerics;

namespace GameWindow.Components.GameComponents
{
    /// <summary>
    /// A class implementing a <see cref="Invader"/> bullet
    /// </summary>
    internal class InvaderBullet : Bullet
    {
        public const int INVADER_BULLET_SPEED = 3;
        private static Random random = new Random();

        /// <summary>
        /// Builds a <see cref="InvaderBullet"/> instance
        /// </summary>
        /// <param name="pos"> A <see cref="Vector2"/> representing the position of the bullet </param>
        public InvaderBullet(Vector2 pos) : base(pos, INVADER_BULLET_SPEED, (BulletType)random.Next(0, 3), CollisionLayer.InvaderBullet)
        {
            col.AddIgnoreLayer(CollisionLayer.Invader);
            col.AddIgnoreLayer(CollisionLayer.InvaderBullet);

            // Add relevant clips
            switch (bulletType)
            {
                case BulletType.Charge:
                    clips.Enqueue(Sprite.BitmapFromPath(@"Resources\Images\Bullet\Charge\Charge1.png"));
                    clips.Enqueue(Sprite.BitmapFromPath(@"Resources\Images\Bullet\Charge\Charge2.png"));
                    clips.Enqueue(Sprite.BitmapFromPath(@"Resources\Images\Bullet\Charge\Charge3.png"));
                    clips.Enqueue(Sprite.BitmapFromPath(@"Resources\Images\Bullet\Charge\Charge4.png"));
                    break;
                case BulletType.Imperfect:
                    clips.Enqueue(Sprite.BitmapFromPath(@"Resources\Images\Bullet\Imperfect\Imperfect1.png"));
                    clips.Enqueue(Sprite.BitmapFromPath(@"Resources\Images\Bullet\Imperfect\Imperfect2.png"));
                    clips.Enqueue(Sprite.BitmapFromPath(@"Resources\Images\Bullet\Imperfect\Imperfect3.png"));
                    clips.Enqueue(Sprite.BitmapFromPath(@"Resources\Images\Bullet\Imperfect\Imperfect4.png"));
                    break;
                case BulletType.ZigZag:
                    clips.Enqueue(Sprite.BitmapFromPath(@"Resources\Images\Bullet\ZigZag\ZigZag1.png"));
                    clips.Enqueue(Sprite.BitmapFromPath(@"Resources\Images\Bullet\ZigZag\ZigZag2.png"));
                    clips.Enqueue(Sprite.BitmapFromPath(@"Resources\Images\Bullet\ZigZag\ZigZag3.png"));
                    clips.Enqueue(Sprite.BitmapFromPath(@"Resources\Images\Bullet\ZigZag\ZigZag4.png"));
                    break;
            }

            BulletLoop();
        }

        /// <summary>
        /// The <see cref="InvaderBullet"/> hit logic and loop
        /// </summary>
        protected async void BulletLoop()
        {
            await BulletMovementLoop();

            Collider? touching = col.TouchingCollider();

            if (touching == null)
            {
                BulletExplosion();
                return;
            }

            if (touching.parent is Player player)
                player.Kill();
            else if (touching.parent is BunkerPart bunker)
                bunker.Hit();
            else if (touching.parent is PlayerBullet bullet)
                bullet.BulletExplosion();

            BulletExplosion();
        }
    }
}