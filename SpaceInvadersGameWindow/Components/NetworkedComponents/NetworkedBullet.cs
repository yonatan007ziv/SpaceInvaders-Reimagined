using GameWindow.Components.GameComponents.NetworkedComponents;
using GameWindow.Components.Initializers;
using GameWindow.Components.Miscellaneous;
using GameWindow.Components.PhysicsEngine.Collider;
using GameWindow.Components.UIElements;
using GameWindow.Systems;
using System.Numerics;
using System.Threading.Tasks;
using System.Windows;
using static GameWindow.Components.Miscellaneous.Delegates;

namespace GameWindow.Components.GameComponents
{
    class NetworkedBullet
    {
        private ActionString? sendMessage;
        private Action? killBullet;
        private string shooter;

        public Transform transform;
        private Sprite sprite;
        private Collider col;
        private readonly float bulletSpeed = 3;

        public NetworkedBullet(Vector2 pos, ActionString sendMessage, Action killBullet)
        {
            Application.Current.Dispatcher.Invoke(() =>
            {
                transform = new Transform(new Vector2(3, 7), pos);
                col = new Collider(transform, this);
                sprite = new Sprite(transform, @"Resources\Images\Bullet\Bullet.png");
            });

            this.shooter = GameInitializers.username!;
            this.sendMessage = sendMessage;
            this.killBullet = killBullet;
            LocalBulletLoop();
        }
        public NetworkedBullet(string shooter)
        {
            Application.Current.Dispatcher.Invoke(() =>
            {
                transform = new Transform(new Vector2(3, 7), NetworkedPlayer.currentPlayers[shooter].transform.Position);
                col = new Collider(transform, this);
                sprite = new Sprite(transform, @"Resources\Images\Bullet\Bullet.png");
            });

            this.shooter = shooter;
            NetworkedPlayer.currentPlayers[shooter].myBullet = this;
            OnlineBulletLoop();
        }

        public async void LocalBulletLoop()
        {
            Vector2 SpeedVector = new Vector2(0, -bulletSpeed);
            while (col!.TouchingCollider() == null || (col.TouchingCollider()!.parent is NetworkedPlayer /*&& ((NetworkedPlayer)col.TouchingCollider()).username == GameInitializers.username) */|| col.TouchingCollider()!.parent is Bullet))
            {
                transform.Position += SpeedVector;
                await Task.Delay(1000 / MainWindow.TargetFPS);
            }

            Collider hit = col.TouchingCollider()!;
            if (hit.parent is NetworkedPlayer player)
                sendMessage!($"BULLET HIT:{player.username}");
            sendMessage!($"BULLET EXPLOSION:");
            killBullet!();
            BulletExplosion();
        }
        public async void OnlineBulletLoop()
        {
            Vector2 SpeedVector = new Vector2(0, bulletSpeed);
            while (col!.TouchingCollider() == null || col.TouchingCollider()!.parent is NetworkedPlayer || col.TouchingCollider()!.parent is Bullet)
            {
                transform.Position += SpeedVector;
                await Task.Delay(1000 / MainWindow.TargetFPS);
            }
        }
        public async void BulletExplosion()
        {
            SoundManager.PlaySound(@"Resources\Sounds\BulletExplosion.wav");

            // Bullet Explosion
            await Application.Current.Dispatcher.Invoke(async () =>
            {
                Transform ExplosionTransform = new Transform(new Vector2(6, 8), transform.Position);
                Sprite ExplosionSprite = new Sprite(ExplosionTransform, @"Resources\Images\Bullet\BulletExplosion.png");
                
                await Task.Delay(500);

                ExplosionTransform.Dispose();
                ExplosionSprite.Dispose();
            });

            Dispose();
        }

        public void Dispose()
        {
            NetworkedPlayer.currentPlayers[shooter].myBullet = null;
            sprite.Dispose();
            col?.Dispose();
            transform.Dispose();
        }
    }
}