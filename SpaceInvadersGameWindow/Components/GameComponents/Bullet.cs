using GameWindow.Components.Miscellaneous;
using GameWindow.Components.UIElements;
using System.Collections.Generic;
using System.Numerics;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media.Imaging;

namespace GameWindow.Components.GameComponents
{
    /// <summary>
    /// Represents the types of available bullets in the game
    /// </summary>
    public enum BulletType
    {
        Charge,
        Imperfect,
        ZigZag,
        Normal
    }

    /// <summary>
    /// Base class for bullets
    /// </summary>
    abstract class Bullet
    {
        public static List<Bullet> AllBullets = new List<Bullet>();

        protected Transform transform;
        protected Sprite sprite;
        protected Collider col;

        private int clipCounter = 0;
        private float originalBulletSpeed;
        protected Queue<BitmapImage> clips = new Queue<BitmapImage>();
        protected float bulletSpeed;
        protected bool bulletHit;
        protected BulletType bulletType;

        /// <summary>
        /// Disposes all bullets by setting their bulletHit field to true, and disposing of their collision, sprite, and transform objects
        /// Also clears the list of all bullets and sets the instance of PlayerBullet to null
        /// </summary>
        public static void DisposeAll()
        {
            for (int i = 0; i < AllBullets.Count; i++)
            {
                if (AllBullets[i] == null) continue;

                AllBullets[i].bulletHit = true;
                AllBullets[i]?.col.Dispose();
                AllBullets[i]?.sprite.Dispose();
                AllBullets[i]?.transform.Dispose();
            }
            PlayerBullet.instance = null;
            AllBullets.Clear();
        }

        /// <summary>
        /// Pauses or unpauses all bullets by setting their bulletSpeed to zero if pause is true, or to their originalBulletSpeed if pause is false
        /// </summary>
        /// <param name="pause"> If true, all bullets are paused, if false, all bullets are unpaused</param>
        public static void PauseUnpauseBullets(bool pause)
        {
            for (int i = 0; i < AllBullets.Count; i++)
            {
                if (AllBullets[i] == null) continue;

                if (pause)
                    AllBullets[i].bulletSpeed = 0;
                else
                    AllBullets[i].bulletSpeed = AllBullets[i].originalBulletSpeed;
            }
        }

        /// <summary>
        /// Constructor for the Bullet class, adds the newly created bullet to the AllBullets list,
        /// initializes the bullet's properties, creates and sets up its transform, collider and sprite objects
        /// </summary>
        /// <param name="pos"> The position of the bullet </param>
        /// <param name="speed"> The speed of the bullet </param>
        /// <param name="bulletType"> The type of the bullet </param>
        /// <param name="colliderLayer"> The collision layer of the bullet </param>
        public Bullet(Vector2 pos, int speed, BulletType bulletType, CollisionLayer colliderLayer)
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

        /// <summary>
        /// The basic bullet loop, runs until the bullet hits a collider that is not ignored
        /// </summary>
        /// <returns> A <see cref="Task"/> representing the async state of the bullet loop </returns>
        protected virtual async Task BulletLoop()
        {
            while (col.TouchingCollider() == null && !bulletHit)
            {
                NextClip();
                transform.Position += new Vector2(0, bulletSpeed);
                await Task.Delay(1000 / MainWindow.TARGET_FPS);
            }
        }

        /// <summary>
        /// Cycles the bullet's clip every 4 <see cref="NextClip"/> iterations
        /// </summary>
        private void NextClip()
        {
            if (clipCounter % 4 != 3)
            {
                clipCounter++;
                return;
            }

            BitmapImage image = clips.Dequeue();
            clips.Enqueue(image);

            sprite.ChangeImage(image);

            clipCounter = 0;
        }

        /// <summary>
        /// Triggers the explosion of the bullet, disposing its collider and changing its sprite
        /// </summary>
        public void BulletExplosion()
        {
            bulletHit = true;
            col.Dispose();

            // Bullet Explosion
            transform.Scale = new Vector2(6, 8);
            sprite.ChangeImage(@"Resources\Images\Bullet\BulletExplosion.png");

            Task.Delay(500).ContinueWith((p) => Dispose());
        }

        /// <summary>
        /// Disposes current <see cref="Bullet"/> object
        /// </summary>
        private void Dispose()
        {
            col.Dispose();
            sprite.Dispose();
            transform.Dispose();
            AllBullets.Remove(this);
        }
    }
}