using GameWindow.Components.Miscellaneous;
using GameWindow.Systems;
using System.Numerics;
using System.Windows.Input;

namespace GameWindow.Components.GameComponents
{
    internal class PlayerController
    {
        public static bool Disabled = false;
        private Transform playerTransform;
        private Collider playerCol;
        public PlayerController(Player player)
        {
            playerTransform = player.transform;
            playerCol = player.col;
            InputHandler.AddInputLoop(InputLoop);
        }

        private void InputLoop()
        {
            if (Disabled) return;

            //Touching walls?
            int axis = InputHandler.GetAxis("Horizontal");
            Collider? col = playerCol.TouchingCollider();
            if (axis == 1 && (col == null || col.parent != Wall.RightWall) ||
                axis == -1 && (col == null || col.parent != Wall.LeftWall))
                playerTransform.Position += new Vector2(axis, 0);

            if (InputHandler.keysDown.Contains(Key.Space) && PlayerBullet.instance == null)
                new PlayerBullet(playerTransform.Position);
        }

        public void Dispose() => InputHandler.RemoveInputLoop(InputLoop);
    }
}