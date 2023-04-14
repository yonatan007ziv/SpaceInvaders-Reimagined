using GameWindow.Components.Miscellaneous;
using GameWindow.Systems;
using System.Numerics;
using System.Windows.Input;

namespace GameWindow.Components.GameComponents
{
    /// <summary>
    /// A class implementing controls for the <see cref="Player"/>
    /// </summary>
    internal class PlayerController
    {
        public static bool disabled = false;
        
        private Transform playerTransform;
        private Collider playerCol;

        /// <summary>
        /// Builds a controller for the <see cref="Player"/> by adding an input loop to <see cref="InputHandler"/>
        /// </summary>
        /// <param name="player"> Current player </param>
        public PlayerController(Player player)
        {
            playerTransform = player.transform;
            playerCol = player.col;
            InputHandler.AddInputLoop(InputLoop);
        }

        /// <summary>
        /// Input loop for the <see cref="Player"/>
        /// </summary>
        private void InputLoop()
        {
            if (disabled) return;

            //Touching walls?
            int axis = InputHandler.GetAxis("Horizontal");
            Collider? col = playerCol.TouchingCollider();
            if (axis == 1 && (col == null || col.parent != Wall.RightWall) ||
                axis == -1 && (col == null || col.parent != Wall.LeftWall))
                playerTransform.Position += new Vector2(axis, 0);

            if (InputHandler.keysDown.Contains(Key.Space) && PlayerBullet.instance == null)
                new PlayerBullet(playerTransform.Position);
        }

        /// <summary>
        /// Disposes the current <see cref="InputLoop"/> from <see cref="InputHandler"/>
        /// </summary>
        public void Dispose() => InputHandler.RemoveInputLoop(InputLoop);
    }
}