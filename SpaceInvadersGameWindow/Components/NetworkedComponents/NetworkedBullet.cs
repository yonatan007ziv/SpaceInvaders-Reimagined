using GameWindow.Components.GameComponents;
using GameWindow.Components.Initializers;
using GameWindow.Components.Miscellaneous;
using System.Numerics;
using System.Threading.Tasks;
using static GameWindow.Components.Miscellaneous.Delegates;

namespace GameWindow.Components.NetworkedComponents
{
    class NetworkedBullet : Bullet
    {
        private ActionString? sendMessage;
        private Action? killBullet;
        private string shooter;

        public NetworkedBullet(Vector2 pos, ActionString sendMessage, Action killBullet) :
            base(pos, -6, BulletType.Normal, CollisionLayer.PlayerBullet)
        {
            shooter = GameInitializers.username!;
            this.sendMessage = sendMessage;
            this.killBullet = killBullet;
            col.IgnoreLayer(NetworkedPlayer.localPlayer.team == 'A' ? CollisionLayer.TeamA : CollisionLayer.TeamB);
            col.IgnoreLayer(CollisionLayer.PlayerBullet);
            LocalBulletLoop();
        }
        public NetworkedBullet(string shooter) :
            base(NetworkedPlayer.currentPlayers[shooter].transform.Position, NetworkedPlayer.currentPlayers[shooter].team == NetworkedPlayer.localPlayer.team ? -6 : 6, BulletType.Normal, CollisionLayer.PlayerBullet)
        {
            this.shooter = shooter;
            NetworkedPlayer.currentPlayers[shooter].myBullet = this;
            col.IgnoreLayer(NetworkedPlayer.currentPlayers[shooter].team == 'A' ? CollisionLayer.TeamA : CollisionLayer.TeamB);
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

            if (col.TouchingCollider()!.parent is NetworkedPlayer player && !player.invincible)
                sendMessage!($"BULLET HIT:(Player,{player.username})");
            else if (col.TouchingCollider()!.parent is NetworkedBunkerPart part)
                sendMessage!($"BULLET HIT:(BunkerPart%{part.BunkerID},{(int)part.part})");

            sendMessage!($"BULLET EXPLOSION");
            killBullet!();
            BulletExplosion();
            NetworkedPlayer.currentPlayers[shooter].myBullet = null;
        }
        public async void OnlineBulletLoop()
        {
            Vector2 SpeedVector = new Vector2(0, bulletSpeed);
            while (col.TouchingCollider() == null)
            {
                transform.Position += SpeedVector;
                await Task.Delay(1000 / MainWindow.TARGET_FPS);
            }
        }
    }
}