using GameWindow.Components.GameComponents;
using GameWindow.Components.Miscellaneous;
using GameWindow.Components.UIElements;
using System.Numerics;

namespace GameWindow.Components.NetworkedComponents
{
    /// <summary>
    /// A class implementing a networked player bullet
    /// </summary>
    class NetworkedBullet : Bullet
    {
        private string shooter;

        /// <summary>
        /// Builds the local player's networked bullet
        /// </summary>
        /// <param name="pos"> A <see cref="Vector2"/> representing the bullet's position </param>
        /// <param name="sendMessage"> Delegate used to send messages to the "Game-Server" </param>
        public NetworkedBullet(Vector2 pos) :
            base(pos, -6, BulletType.Normal, CollisionLayer.PlayerBullet)
        {
            if (NetworkedPlayer.localPlayer!.SendMessage != null)
                NetworkedPlayer.localPlayer.SendMessage($"InitiateBullet");

            shooter = MainWindow.username!;
            col.AddIgnoreLayer(NetworkedPlayer.localPlayer!.team == 'A' ? CollisionLayer.TeamA : CollisionLayer.TeamB);
            col.AddIgnoreLayer(NetworkedPlayer.localPlayer!.team == 'A' ? CollisionLayer.BunkerA : CollisionLayer.BunkerB);
            col.AddIgnoreLayer(CollisionLayer.PlayerBullet);

            clips.Enqueue(Sprite.BitmapFromPath(@"Resources\Images\Bullet\Bullet.png"));

            LocalBulletLoop();
        }

        /// <summary>
        /// Builds a opponent player's networked bullet
        /// </summary>
        /// <param name="shooter"> Shooter's name </param>
        public NetworkedBullet(string shooter) :
            base(NetworkedPlayer.currentPlayers[shooter].transform.Position, NetworkedPlayer.currentPlayers[shooter].team == NetworkedPlayer.localPlayer!.team ? -6 : 6, BulletType.Normal, CollisionLayer.PlayerBullet)
        {
            this.shooter = shooter;
            NetworkedPlayer.currentPlayers[shooter].myBullet = this;
            col.AddIgnoreLayer(NetworkedPlayer.currentPlayers[shooter].team == 'A' ? CollisionLayer.TeamA : CollisionLayer.TeamB);
            col.AddIgnoreLayer(NetworkedPlayer.currentPlayers[shooter].team == 'A' ? CollisionLayer.BunkerA : CollisionLayer.BunkerB);
            col.AddIgnoreLayer(CollisionLayer.PlayerBullet);

            clips.Enqueue(Sprite.BitmapFromPath(@"Resources\Images\Bullet\Bullet.png"));

            OnlineBulletLoop();
        }

        /// <summary>
        /// Local bullet loop
        /// </summary>
        public async void LocalBulletLoop()
        {
            await BulletMovementLoop();
            if (disposed) return;

            Collider? hit = col.TouchingCollider();

            if (hit == null)
            {
                if (NetworkedPlayer.localPlayer!.SendMessage != null)
                    NetworkedPlayer.localPlayer.SendMessage($"BulletExplosion");

                NetworkedPlayer.localPlayer!.myBullet = null;
                BulletExplosion();
                NetworkedPlayer.currentPlayers[shooter].myBullet = null;
                return;
            }

            if (hit.parent is NetworkedPlayer player && !player.invincible)
            {
                if (NetworkedPlayer.localPlayer!.SendMessage != null)
                    NetworkedPlayer.localPlayer.SendMessage($"BulletHit:Player({player.username})");
            }
            else if (hit.parent is NetworkedBunkerPart part)
            {
                if (NetworkedPlayer.localPlayer!.SendMessage != null)
                    NetworkedPlayer.localPlayer.SendMessage($"BulletHit:BunkerPart({part.bunkerID},{(int)part.part},{part.imagePathIndex + 1})");
            }

            if (NetworkedPlayer.localPlayer!.SendMessage != null)
                NetworkedPlayer.localPlayer.SendMessage($"BulletExplosion");

            NetworkedPlayer.localPlayer!.myBullet = null;
            BulletExplosion();
            NetworkedPlayer.currentPlayers[shooter].myBullet = null;
        }

        /// <summary>
        /// Online bullet loop
        /// </summary>
        public async void OnlineBulletLoop()
        {
            await BulletMovementLoop();
        }
    }
}