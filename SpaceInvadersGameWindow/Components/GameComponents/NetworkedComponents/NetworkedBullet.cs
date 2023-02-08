using SpaceInvaders.Components.GameComponents;
using SpaceInvaders.Components.Miscellaneous;
using SpaceInvaders.Components.PhysicsEngine.Collider;
using SpaceInvaders.Systems;
using SpaceInvadersGameWindow.Components.GameComponents.NetworkedComponents;
using SpaceInvadersGameWindow.Components.UIElements;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace SpaceInvadersGameWindow.Components.GameComponents
{
    class NetworkedBullet
    {
        public static Dictionary<string, NetworkedBullet> NetworkedBullets = new Dictionary<string, NetworkedBullet>();
        public static NetworkedBullet? localBullet;

        private NetworkStream? ns;
        public Transform transform;
        private Sprite sprite;
        private Collider? col;
        private float bulletSpeed = 3;

        public NetworkedBullet(Vector2 pos, NetworkStream ns)
        {
            localBullet = this;
            //this.owner = owner;
            this.ns = ns;
            transform = new Transform(new Vector2(3, 7), pos);
            col = new Collider(transform, this);
            sprite = new Sprite(transform, @"Resources\RawFiles\Images\Bullet\Bullet.png");
            SendMessage($"INITIATE BULLET:({transform.Position.X},{transform.Position.Y})");
            BulletLoop();
        }
        public NetworkedBullet(Vector2 pos,string nickname)
        {
            NetworkedBullets.Add(nickname, this);

            transform = new Transform(new Vector2(3, 7), pos);
            sprite = new Sprite(transform, @"Resources\RawFiles\Images\Bullet\Bullet.png");
        }

        public async void BulletLoop()
        {
            Vector2 SpeedVector = new Vector2(0, -bulletSpeed);
            while (col!.TouchingCollider() == null || col.TouchingCollider()!.parent is NetworkedPlayer || col.TouchingCollider()!.parent is Bullet)
            {
                transform.Position += SpeedVector;
                SendMessage($"BULLET POS:({transform.Position.X},{transform.Position.Y})");
                await Task.Delay(1000 / MainWindow.TargetFPS);
            }

            //Collider col = this.col.TouchingCollider()!;
            //if (col.parent is Invader)
            //    ((Invader)col.parent).Death();

            BulletExplosion();
            localBullet = null;
        }
        public async void BulletExplosion()
        {
            SendMessage($"BULLET EXPLOSION POS:({transform.Position.X},{transform.Position.Y})");

            SoundManager.PlaySound(@"Resources\RawFiles\Sounds\BulletExplosion.wav");

            // Bullet Explosion
            Transform ExplosionTransform = new Transform(new Vector2(6, 8), transform.Position);
            Sprite ExplosionSprite = new Sprite(ExplosionTransform, @"Resources\RawFiles\Images\Bullet\BulletExplosion.png");

            await Task.Delay(500);

            ExplosionTransform.Dispose();
            ExplosionSprite.Dispose();
            Dispose();
        }

        private void SendMessage(string msg)
        {
            byte[] buffer = Encoding.UTF8.GetBytes(msg);
            ns!.Write(buffer, 0, buffer.Length);
        }

        public void Dispose()
        {
            sprite.Dispose();
            col?.Dispose();
            transform.Dispose();
        }
    }
}