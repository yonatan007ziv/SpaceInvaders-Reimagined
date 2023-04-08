using GameWindow.Components.GameComponents.NetworkedComponents;
using GameWindow.Components.Initializers;
using GameWindow.Components.Miscellaneous;
using System.Numerics;
using System.Threading.Tasks;
using static GameWindow.Components.Miscellaneous.Delegates;

namespace GameWindow.Components.GameComponents
{
    class NetworkedBullet : Bullet
    {
        private ActionString? sendMessage;
        private Action? killBullet;
        private string shooter;

        public NetworkedBullet(Vector2 pos, ActionString sendMessage, Action killBullet) :
            base(pos, -6, BulletType.Normal, CollisionLayer.PlayerBullet)
        {
            this.shooter = GameInitializers.username!;
            this.sendMessage = sendMessage;
            this.killBullet = killBullet;
            col.IgnoreLayer(CollisionLayer.Player);
            col.IgnoreLayer(CollisionLayer.OnlinePlayerBullet);
            LocalBulletLoop();
        }
        public NetworkedBullet(string shooter) :
            base(NetworkedPlayer.currentPlayers[shooter].transform.Position, 6, BulletType.Normal, CollisionLayer.OnlinePlayerBullet)
        {
            this.shooter = shooter;
            NetworkedPlayer.currentPlayers[shooter].myBullet = this;
            col.IgnoreLayer(CollisionLayer.OnlinePlayer);
            col.IgnoreLayer(CollisionLayer.PlayerBullet);
            OnlineBulletLoop();
        }

        public async void LocalBulletLoop()
        {
            Vector2 SpeedVector = new Vector2(0, bulletSpeed);
            while (col.TouchingCollider() == null)
            {
                transform.Position += SpeedVector;
                await Task.Delay(1000 / MainWindow.TARGET_FPS);
            }

            if (col.TouchingCollider()!.parent is NetworkedPlayer player)
            {
                sendMessage!($"BULLET HIT:{player.username}");
                player.OnlineKill();
            }

            sendMessage!($"BULLET EXPLOSION:");
            killBullet!();
            BulletExplosion();
            NetworkedPlayer.currentPlayers[shooter].myBullet = null;
        }
        public async void OnlineBulletLoop()
        {
            Vector2 SpeedVector = new Vector2(0, bulletSpeed);
            while (col.TouchingCollider() == null /*&& !BulletHit*/)
            {
                transform.Position += SpeedVector;
                await Task.Delay(1000 / MainWindow.TARGET_FPS);
            }
        }
    }
}