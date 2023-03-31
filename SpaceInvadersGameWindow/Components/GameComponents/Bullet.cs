using GameWindow.Components.Miscellaneous;
using GameWindow.Components.PhysicsEngine.Collider;
using GameWindow.Components.UIElements;
using GameWindow.Systems;
using System.Collections.Generic;
using System.Numerics;
using System.Security.Policy;
using System.Threading.Tasks;
using System.Windows;

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

        //public bool BulletHit = false;
        public Transform transform;
        protected float bulletSpeed = 3;
        protected Sprite sprite;
        protected Collider col;
        private float times;
        private BulletTypes bulletType;

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

            // UI Objects need to be created in an STA thread
            Application.Current.Dispatcher.Invoke(() => sprite = new Sprite(transform, @"Resources\Images\Bullet\Bullet.png"));

            // Suppressing the "Null When Leaving a Constructor" warning
            sprite!.ToString();
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
                    sprite.ChangeImage(Sprite.BitmapFromPath(@$"Resources\Images\Bullet\Charge\Charge{times + 1}.png"));
                    break;
                case BulletTypes.Imperfect:
                    sprite.ChangeImage(Sprite.BitmapFromPath(@$"Resources\Images\Bullet\Imperfect\Imperfect{times + 1}.png"));
                    break;
                case BulletTypes.ZigZag:
                    sprite.ChangeImage(Sprite.BitmapFromPath(@$"Resources\Images\Bullet\ZigZag\ZigZag{times + 1}.png"));
                    break;
                case BulletTypes.Normal:
                    sprite.ChangeImage(Sprite.BitmapFromPath(@$"Resources\Images\Bullet\Bullet.png"));
                    break;
            }
        }
        public void BulletExplosion()
        {
            //BulletHit = true;
            AllBullets.Remove(this);
            col.Dispose();

            // Bullet Explosion
            transform.Scale = new Vector2(6, 8);
            sprite.ChangeImage(Sprite.BitmapFromPath(@"Resources\Images\Bullet\BulletExplosion.png"));

            Task.Delay(500).ContinueWith((p) => Dispose());
        }
        private void Dispose()
        {
            sprite.Dispose();
            transform.Dispose();
            col.Dispose();
        }
    }
}