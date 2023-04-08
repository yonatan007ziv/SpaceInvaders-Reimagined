using GameWindow.Components.Miscellaneous;
using System;
using System.Numerics;
using System.Threading.Tasks;

namespace GameWindow.Components.GameComponents
{
    internal class InvaderBullet : Bullet
    {
        public const int INVADER_BULLET_SPEED = 3;
        private static Random random = new Random();

        public InvaderBullet(Vector2 pos) : base(pos, INVADER_BULLET_SPEED, (BulletType)random.Next(0, 3), CollisionLayer.InvaderBullet)
        {
            col.IgnoreLayer(CollisionLayer.Invader);
            col.IgnoreLayer(CollisionLayer.InvaderBullet);

            BulletLoop();
        }

        private async void BulletLoop()
        {
            while (col.TouchingCollider() == null && !bulletHit)
            {
                NextClip();
                transform.Position += new Vector2(0, bulletSpeed);
                await Task.Delay(1000 / MainWindow.TARGET_FPS);
            }
            if (bulletHit)
                return;

            Collider touching = col.TouchingCollider()!;
            if (touching.parent is Player player)
                player.Kill();
            else if (touching.parent is BunkerPart bunker)
                bunker.Hit();
            else if (touching.parent is PlayerBullet bullet)
                bullet.BulletExplosion();

            BulletExplosion();
        }

        private float times;
        int timesCounter = 0;

        public void NextClip()
        {
            timesCounter++;
            if (timesCounter == 4)
            {
                times++;
                timesCounter = 0;
            }

            times %= 4;
            string imagePath;
            switch (bulletType)
            {
                default:
                    imagePath = @$"Resources\Images\Bullet\Bullet.png";
                    break;
                case BulletType.Charge:
                    imagePath = @$"Resources\Images\Bullet\Charge\Charge{times + 1}.png";
                    break;
                case BulletType.Imperfect:
                    imagePath = @$"Resources\Images\Bullet\Imperfect\Imperfect{times + 1}.png";
                    break;
                case BulletType.ZigZag:
                    imagePath = @$"Resources\Images\Bullet\ZigZag\ZigZag{times + 1}.png";
                    break;
            }
            sprite.ChangeImage(imagePath);
        }
    }
}