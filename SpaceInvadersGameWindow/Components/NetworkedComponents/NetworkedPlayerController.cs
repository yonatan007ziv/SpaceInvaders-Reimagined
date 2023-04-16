using GameWindow.Components.GameComponents;
using GameWindow.Components.Miscellaneous;
using GameWindow.Systems;
using System.Numerics;
using System.Windows.Input;

namespace GameWindow.Components.NetworkedComponents
{
    /// <summary>
    /// Implementation of the networked player controller
    /// </summary>
    internal class NetworkedPlayerController
    {
        public bool disabled = false;
        Transform transform;
        Collider col;
        private DelegatesActions.ActionString sendMessage;
        private NetworkedPlayer player;

        /// <summary>
        /// Builds a new <see cref="NetworkedPlayerController"/>
        /// </summary>
        /// <param name="player"> The local <see cref="NetworkedPlayer"/> </param>
        /// <param name="sendMessage"> Delegate used to send messages to the "Game-Server" </param>
        public NetworkedPlayerController(NetworkedPlayer player, DelegatesActions.ActionString sendMessage)
        {
            this.player = player;
            transform = player.transform;
            col = player.col;
            this.sendMessage = sendMessage;
            InputHandler.AddInputLoop(InputLoop);
        }

        /// <summary>
        /// Player's input loop
        /// </summary>
        private void InputLoop()
        {
            if (disabled) return;

            //Touching walls?
            int axis = InputHandler.GetAxis("Horizontal");
            Collider? col = this.col.TouchingCollider();
            if (axis == 1 && (col == null || col.parent != Wall.RightWall) || axis == -1 && (col == null || col.parent != Wall.LeftWall))
            {
                transform.Position += new Vector2(axis, 0);
                sendMessage($"PlayerPosition:{transform.Position.X}");
            }
            if (InputHandler.keysDown.Contains(Key.Space) && player.myBullet == null)
                player.myBullet = new NetworkedBullet(transform.Position);
        }

        /// <summary>
        /// Disposes the current <see cref="NetworkedPlayerController"/> by removing its <see cref="InputLoop"/> from the <see cref="InputHandler.inputLoopDel"/>
        /// </summary>
        public void Dispose()
        {
            InputHandler.RemoveInputLoop(InputLoop);
        }
    }
}