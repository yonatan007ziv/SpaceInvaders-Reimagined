using GameWindow.Components.Miscellaneous;
using GameWindow.Components.PhysicsEngine.Collider;
using GameWindow.Systems;
using System.Numerics;
using System.Windows.Input;
using static GameWindow.Components.Miscellaneous.Delegates;

namespace GameWindow.Components.GameComponents.NetworkedComponents
{
    internal class NetworkedCharacterController
    {
        public bool disabled = false;
        Transform transform;
        Collider col;
        private ActionString sendMessage;
        private NetworkedPlayer player;
        public NetworkedCharacterController(NetworkedPlayer player, Transform transform, Collider col, ActionString sendMessage)
        {
            this.player = player;
            this.transform = transform;
            this.col = col;
            this.sendMessage = sendMessage;
            InputHandler.AddInputLoop(InputLoop);
        }

        private void InputLoop()
        {
            if (disabled) return;

            //Touching walls?
            int axis = InputHandler.GetAxis("Horizontal");
            Collider? col = this.col.TouchingCollider();
            if (axis == 1 && (col == null || col.parent != Wall.RightWall) || axis == -1 && (col == null || col.parent != Wall.LeftWall))
            {
                transform.Position += new Vector2(axis, 0);
                sendMessage($"PLAYER POS:{transform.Position.X}");
            }
            if (InputHandler.keysDown.Contains(Key.Space) && player.myBullet == null)
            {
                player.myBullet = new NetworkedBullet(transform.Position, sendMessage, () => player.myBullet = null);
                sendMessage($"INITIATE BULLET:");
            }
        }

        public void Dispose()
        {
            InputHandler.RemoveInputLoop(InputLoop);
        }
    }
}
