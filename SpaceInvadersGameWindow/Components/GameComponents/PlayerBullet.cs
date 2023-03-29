using GameWindow.Components.GameComponents.Bunker;
using GameWindow.Components.PhysicsEngine.Collider;
using GameWindow.Systems;
using System.Numerics;
using System.Threading.Tasks;

namespace GameWindow.Components.GameComponents
{
    internal class PlayerBullet : Bullet
    {
        public static PlayerBullet? instance;
        public PlayerBullet(Vector2 pos, int dir) : base(pos, dir, BulletTypes.Normal, Collider.Layers.PlayerBullet)
        {
            instance = this;
            bulletSpeed *= 2;

            SoundManager.PlaySound("Shoot");
            col.IgnoreLayer(Collider.Layers.Player);
            col.IgnoreLayer(Collider.Layers.PlayerBullet);
            col.IgnoreLayer(Collider.Layers.InvaderBullet);

            BulletLoop();
        }
        private async void BulletLoop()
        {
            Vector2 SpeedVector = new Vector2(0, bulletSpeed);
            while (col.TouchingCollider() == null)
            {
                NextClip();
                transform.Position += SpeedVector;
                await Task.Delay(1000 / MainWindow.TARGET_FPS);
            }

            Collider touching = col.TouchingCollider()!;
            if (touching.parent is Invader inv)
                inv.Death();
            if (touching.parent is BunkerPart bunker)
                bunker.Hit();

            BulletExplosion();
            instance = null;
        }
    }
}