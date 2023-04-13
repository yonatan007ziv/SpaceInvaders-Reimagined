using GameWindow.Components.GameComponents;
using GameWindow.Components.Miscellaneous;
using GameWindow.Systems;
using System.Numerics;
using System.Windows.Input;
using static GameWindow.Components.Miscellaneous.Delegates;

namespace GameWindow.Components.NetworkedComponents
{
    internal class NetworkedPlayerController
    {
        public bool disabled = false;
        Transform transform;
        Collider col;
        private ActionString sendMessage;
        private NetworkedPlayer player;

        public NetworkedPlayerController(NetworkedPlayer player, Transform transform, Collider col, ActionString sendMessage)
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
                sendMessage($"PlayerPosition:{transform.Position.X}");
            }
            if (InputHandler.keysDown.Contains(Key.Space) && player.myBullet == null)
                player.myBullet = new NetworkedBullet(transform.Position, sendMessage);
        }
        public void Dispose()
        {
            InputHandler.RemoveInputLoop(InputLoop);
        }
    }
}