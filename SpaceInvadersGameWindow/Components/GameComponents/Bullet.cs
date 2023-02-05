using SpaceInvaders.Components.Miscellaneous;
using SpaceInvaders.Components.PhysicsEngine.Collider;
using SpaceInvaders.Systems;
using SpaceInvadersGameWindow.Components.UIElements;
using System.Numerics;
using System.Threading.Tasks;

namespace SpaceInvadersGameWindow.Components.GameComponents
{
    class Bullet
    {
        private enum BulletTypes
        {
            Charge,
            Imperfect,
            ZigZag,
            Normal
        }

        private BulletTypes bulletType;
        private float times;
        public Transform transform;
        protected Sprite sprite;
        protected Collider col;
        protected float bulletSpeed = 3;
        public Bullet(Vector2 pos, int dir, int type)
        {
            bulletType = (BulletTypes)type;
            bulletSpeed *= dir;
            transform = new Transform(new Vector2(3, 7), pos);
            col = new Collider(transform, this);
            sprite = new Sprite(transform, @"Resources\RawFiles\Images\Bullet.png");
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
                    sprite.image.Source = Sprite.BitmapFromPath(@$"Resources\RawFiles\Images\Bullet\Charge\Charge{times + 1}.png");
                    return;
                case BulletTypes.Imperfect:
                    sprite.image.Source = Sprite.BitmapFromPath(@$"Resources\RawFiles\Images\Bullet\Imperfect\Imperfect{times + 1}.png");
                    return;
                case BulletTypes.ZigZag:
                    sprite.image.Source = Sprite.BitmapFromPath(@$"Resources\RawFiles\Images\Bullet\ZigZag\ZigZag{times + 1}.png");
                    return;
                case BulletTypes.Normal:
                    sprite.image.Source = Sprite.BitmapFromPath(@$"Resources\RawFiles\Images\Bullet\Bullet.png");
                    return;
            }
        }
        public async void BulletExplosion()
        {
            SoundManager.PlaySound(@"Resources\RawFiles\Sounds\BulletExplosion.wav");

            // Bullet Explosion
            Transform ExplosionTransform = new Transform(new Vector2(6, 8), transform.Position);
            Sprite ExplosionSprite = new Sprite(ExplosionTransform, @"Resources\RawFiles\Images\Bullet\BulletExplosion.png");

            await Task.Delay(500);

            ExplosionTransform.Dispose();
            ExplosionSprite.Dispose();
            Dispose();
        }
        public void Dispose()
        {
            sprite.Dispose();
            col.Dispose();
            transform.Dispose();
        }
    }
}