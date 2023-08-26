using GameWindow.Components.Miscellaneous;
using GameWindow.Components.UIElements;
using GameWindow.Factories;
using System.Collections.Generic;
using System.Numerics;
using System.Threading.Tasks;

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
        protected Queue<Image>? clips = new Queue<Image>();
        protected float bulletSpeed;
        private bool paused;
        protected bool disposed;
        protected BulletType bulletType;

        /// <summary>
        /// Pauses or unpauses all bullets by setting their bulletSpeed to zero if pause is true, or to their originalBulletSpeed if pause is false
        /// </summary>
        /// <param name="pause"> If true, all bullets are paused, if false, all bullets are unpaused</param>
        public static void PauseAll()
        {
            for (int i = 0; i < AllBullets.Count; i++)
            {
                if (AllBullets[i] == null) continue;
                AllBullets[i].Pause();
            }
        }

        public static void UnpauseAll()
        {
            for (int i = 0; i < AllBullets.Count; i++)
            {
                if (AllBullets[i] == null) continue;
                AllBullets[i].Unpause();
            }
        }

        public void Pause()
        {
            bulletSpeed = 0;
            paused = true;
        }

        public void Unpause()
        {
            bulletSpeed = originalBulletSpeed;
            paused = false;
        }

        /// <summary>
        /// Constructor for the Bullet class, adds the newly created bullet to the AllBullets list,
        /// initializes the bullet's properties, creates and sets up its transform, collider and sprite objects
        /// </summary>
        /// <param name="pos"> A <see cref="Vector2"/> representing the bullet's position </param>
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
            sprite = UIElementFactory.CreateSprite(transform, Image.Bullet);
        }

        /// <summary>
        /// The basic bullet loop, runs until the bullet hits a collider that is not ignored
        /// </summary>
        /// <returns> A <see cref="Task"/> representing the async state of the bullet loop </returns>
        protected async Task BulletMovementLoop()
        {
            while (col.TouchingCollider() == null && !disposed)
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
            if (paused) return;
            if (clipCounter % 4 != 3)
            {
                clipCounter++;
                return;
            }

            Image image = clips?.Dequeue() ?? Image.MissingSprite;
            clips?.Enqueue(image);

            sprite.ChangeImage(image);

            clipCounter = 0;
        }

        /// <summary>
        /// Triggers the explosion of the bullet, disposing its collider and changing its sprite
        /// </summary>
        public async void BulletExplosion()
        {
            col.Dispose();

            // Bullet Explosion
            transform.Scale = new Vector2(6, 8);
            sprite.ChangeImage(Image.BulletExplosion);

            await Task.Delay(500);
            Dispose();
            AllBullets.Remove(this);
        }

        /// <summary>
        /// Disposes current <see cref="Bullet"/> object
        /// </summary>
        private void Dispose()
        {
            disposed = true;
            col.Dispose();
            sprite.Dispose();
            transform.Dispose();
        }

        /// <summary>
        /// Disposes all bullets by setting their bulletHit field to true, and disposing of their collision, sprite, and transform objects
        /// Also clears the list of all bullets and sets the instance of PlayerBullet to null
        /// </summary>
        public static void DisposeAll()
        {
            for (int i = 0; i < AllBullets.Count; i++)
            {
                if (AllBullets[i] == null) continue;
                AllBullets[i].Dispose();
            }
            PlayerBullet.instance = null;
            AllBullets.Clear();
        }
    }
}