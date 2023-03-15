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
    class NetworkedBullet : Bullet
    {
        private ActionString? sendMessage;
        private Action? killBullet;
        private string shooter;

        public NetworkedBullet(Vector2 pos, ActionString sendMessage, Action killBullet) :
            base(pos, -1, BulletTypes.Normal, Collider.Layers.PlayerBullet)
        {
            this.shooter = GameInitializers.username!;
            this.sendMessage = sendMessage;
            this.killBullet = killBullet;
            LocalBulletLoop();
        }
        public NetworkedBullet(string shooter) :
            base(NetworkedPlayer.currentPlayers[shooter].transform.Position, -1, BulletTypes.Normal, Collider.Layers.OnlinePlayerBullet)
        {
            this.shooter = shooter;
            NetworkedPlayer.currentPlayers[shooter].myBullet = this;
            OnlineBulletLoop();
        }

        public async void LocalBulletLoop()
        {
            Vector2 SpeedVector = new Vector2(0, -bulletSpeed);
            while (col.TouchingCollider() == null)
            {
                transform.Position += SpeedVector;
                await Task.Delay(1000 / MainWindow.TARGET_FPS);
            }

            if (col.TouchingCollider()!.parent is NetworkedPlayer player)
                sendMessage!($"BULLET HIT:{player.username}");

            sendMessage!($"BULLET EXPLOSION:");
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