using GameWindow.Components.Miscellaneous;
using GameWindow.Components.PhysicsEngine.Collider;
using GameWindow.Components.UIElements;
using System.Collections.Generic;
using System.Numerics;
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

        public Transform transform;
        private float originalBulletSpeed;
        protected float bulletSpeed;
        protected Sprite sprite;
        protected Collider col;
        protected bool bulletHit;
        private float times;
        private BulletTypes bulletType;

        public static void DisposeAll()
        {
            for (int i = 0; i < AllBullets.Count; i++)
            {
                AllBullets[i].bulletHit = true;
                AllBullets[i].col.Dispose();
                AllBullets[i].sprite.Dispose();
                AllBullets[i].transform.Dispose();
            }
            AllBullets.Clear();
        }
        public static void PauseUnpauseBullets(bool pause)
        {
            for (int i = 0; i < AllBullets.Count; i++)
                if (pause)
                    AllBullets[i].bulletSpeed = 0;
                else
                    AllBullets[i].bulletSpeed = AllBullets[i].originalBulletSpeed;
        }
        public Bullet(Vector2 pos,int speed, BulletTypes bulletType, Collider.Layers colliderLayer)
        {
            AllBullets.Add(this);
            this.bulletType = bulletType;
            originalBulletSpeed = speed;
            bulletSpeed = originalBulletSpeed;
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
            if (timesCounter == 4)
            {
                times++;
                timesCounter = 0;
            }

            times %= 4;

            sprite.Dispose();
            string imagePath;
            switch (bulletType)
            {
                default:
                    imagePath = @$"Resources\Images\Bullet\Bullet.png";
                    break;
                case BulletTypes.Charge:
                    imagePath = @$"Resources\Images\Bullet\Charge\Charge{times + 1}.png";
                    break;
                case BulletTypes.Imperfect:
                    imagePath = @$"Resources\Images\Bullet\Imperfect\Imperfect{times + 1}.png";
                    break;
                case BulletTypes.ZigZag:
                    imagePath = @$"Resources\Images\Bullet\ZigZag\ZigZag{times + 1}.png";
                    break;
            }

            // UI Objects need to be created in an STA thread
            Application.Current.Dispatcher.Invoke(() => sprite = new Sprite(transform, imagePath));
        }
        public void BulletExplosion()
        {
            bulletHit = true;
            col.Dispose();
            sprite.Dispose();

            // Bullet Explosion
            transform.Scale = new Vector2(6, 8);
            Application.Current.Dispatcher.Invoke(() => sprite = new Sprite(transform, @"Resources\Images\Bullet\BulletExplosion.png"));

            Task.Delay(500).ContinueWith((p) => Dispose());
        }
        private void Dispose()
        {
            col.Dispose();
            sprite.Dispose();
            transform.Dispose();
            AllBullets.Remove(this);
        }
    }
}