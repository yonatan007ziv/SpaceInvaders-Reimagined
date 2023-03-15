using GameWindow.Components.Miscellaneous;
using GameWindow.Components.PhysicsEngine.Collider;
using GameWindow.Components.UIElements;
using GameWindow.Systems;
using System.Collections.Generic;
using System.Numerics;
using System.Threading.Tasks;

namespace GameWindow.Components.GameComponents
{
    class Bullet
    {
        public enum BulletTypes
        {
            Charge,
            Imperfect,
            ZigZag,
            Normal
        }
        public static List<Bullet> AllBullets = new List<Bullet>();
        private BulletTypes bulletType;
        private float times;
        public Transform transform;
        protected Sprite sprite;
        protected Collider col;
        protected float bulletSpeed = 3;

        public static void DisposeAll()
        {
            foreach (Bullet b in AllBullets)
                b.Dispose();
            AllBullets.Clear();
        }
        public Bullet(Vector2 pos, int dir, BulletTypes bulletType, Collider.Layers colliderLayer)
        {
            AllBullets.Add(this);
            this.bulletType = bulletType;
            bulletSpeed *= dir;
            transform = new Transform(new Vector2(3, 7), pos);
            col = new Collider(transform, this, colliderLayer);
            sprite = new Sprite(transform, @"Resources\Images\Bullet.png");
        }
        int timesCounter = 0;
        public void NextClip()
        {
            timesCounter++;
            if(timesCounter == 4)
            {
                times++;
                timesCounter = 0;
            }

            times %= 4;
            switch (bulletType)
            {
                case BulletTypes.Charge:
                    sprite.image.Source = Sprite.BitmapFromPath(@$"Resources\Images\Bullet\Charge\Charge{times + 1}.png");
                    break;
                case BulletTypes.Imperfect:
                    sprite.image.Source = Sprite.BitmapFromPath(@$"Resources\Images\Bullet\Imperfect\Imperfect{times + 1}.png");
                    break;
                case BulletTypes.ZigZag:
                    sprite.image.Source = Sprite.BitmapFromPath(@$"Resources\Images\Bullet\ZigZag\ZigZag{times + 1}.png");
                    break;
                case BulletTypes.Normal:
                    sprite.image.Source = Sprite.BitmapFromPath(@$"Resources\Images\Bullet\Bullet.png");
                    break;
            }
        }
        public void BulletExplosion()
        {
            AllBullets.Remove(this);
            Dispose();

            SoundManager.PlaySound(@"Resources\Sounds\BulletExplosion.wav");

            // Bullet Explosion
            transform = new Transform(new Vector2(6, 8), transform.Position);
            sprite = new Sprite(transform, @"Resources\Images\Bullet\BulletExplosion.png");

            Task.Delay(500).ContinueWith((p) =>
            {
                transform.Dispose();
                sprite.Dispose();
            });
        }
        private void Dispose()
        {
            sprite.Dispose();
            transform.Dispose();
            col?.Dispose();
        }
    }
}