using GameWindow.Components.Miscellaneous;
using GameWindow.Components.PhysicsEngine.Collider;
using GameWindow.Systems;
using System.Numerics;
using System.Windows.Input;

namespace GameWindow.Components.GameComponents
{
    internal class CharacterController
    {
        public static bool Disabled = false;
        private Transform transform;
        private Collider col;
        public CharacterController(Transform transform, Collider col)
        {
            this.transform = transform;
            this.col = col;
            InputHandler.AddInputLoop(InputLoop);
        }

        private void InputLoop()
        {
            if (Disabled) return;

            //Touching walls?
            int axis = InputHandler.GetAxis("Horizontal");
            Collider? col = this.col.TouchingCollider();
            if (axis == 1 && (col == null || col.parent != Wall.RightWall) ||
                axis == -1 && (col == null || col.parent != Wall.LeftWall))
                transform.Position += new Vector2(axis, 0);

            if (InputHandler.keysDown.Contains(Key.Space) && PlayerBullet.instance == null)
                new PlayerBullet(transform.Position, -6);
        }

        public void Dispose()
        {
            InputHandler.RemoveInputLoop(InputLoop);
        }
    }
}