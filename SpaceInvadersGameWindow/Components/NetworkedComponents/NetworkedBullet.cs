using GameWindow.Components.GameComponents;
using GameWindow.Components.Initializers;
using GameWindow.Components.Miscellaneous;
using System.Numerics;
using static GameWindow.Components.Miscellaneous.Delegates;

namespace GameWindow.Components.NetworkedComponents
{
    /// <summary>
    /// A class implementing a networked player bullet
    /// </summary>
    class NetworkedBullet : Bullet
    {
        private ActionString? sendMessage;
        private string shooter;

        /// <summary>
        /// Builds the local player's networked bullet
        /// </summary>
        /// <param name="pos"> A <see cref="Vector2"/> representing the bullet's position </param>
        /// <param name="sendMessage"> Delegate used to send messages to the "Game-Server" </param>
        public NetworkedBullet(Vector2 pos, ActionString sendMessage) :
            base(pos, -6, BulletType.Normal, CollisionLayer.PlayerBullet)
        {
            sendMessage($"InitiateBullet");
            shooter = GameInitializers.username!;
            this.sendMessage = sendMessage;
            col.AddIgnoreLayer(NetworkedPlayer.localPlayer!.team == 'A' ? CollisionLayer.TeamA : CollisionLayer.TeamB);
            col.AddIgnoreLayer(CollisionLayer.PlayerBullet);

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
            col.AddIgnoreLayer(CollisionLayer.PlayerBullet);
            OnlineBulletLoop();
        }

        /// <summary>
        /// Local bullet loop
        /// </summary>
        public async void LocalBulletLoop()
        {
            await BulletMovementLoop();

            if (col.TouchingCollider()!.parent is NetworkedPlayer player && !player.invincible)
                sendMessage!($"BulletHit:Player({player.username})");
            else if (col.TouchingCollider()!.parent is NetworkedBunkerPart part)
                sendMessage!($"BulletHit:BunkerPart({part.bunkerID},{(int)part.part},{part.imagePathIndex + 1})");

            sendMessage!($"BulletExplosion");
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