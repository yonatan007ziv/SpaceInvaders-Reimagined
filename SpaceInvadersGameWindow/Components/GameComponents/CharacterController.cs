using GameWindow.Components.Initializers;
using GameWindow.Components.Miscellaneous;
using GameWindow.Components.PhysicsEngine.Collider;
using GameWindow.Systems;
using System;
using System.Numerics;
using System.Windows.Input;
namespace GameWindow.Components.GameComponents
{
    internal class CharacterController
    {
        public bool disabled = false;
        private InputHandler inputHandler;
        private Transform transform;
        private Collider col;
        public CharacterController(Transform transform, Collider col)
        {
            inputHandler = MainWindow.instance!.inputHandler;
            this.transform = transform;
            this.col = col;
        }

        private void InputLoop()
        {
            if (disabled) return;

            //Touching walls?
            int axis = inputHandler.GetAxis("Horizontal");
            Collider? col = this.col.TouchingCollider();
            if (axis == 1 && (col == null || col.parent != Wall.RightWall) ||
                axis == -1 && (col == null || col.parent != Wall.LeftWall))
                transform.Position += new Vector2(axis, 0);

            if (inputHandler.keysDown.Contains(Key.Space) && PlayerBullet.instance == null)
                new PlayerBullet(transform.Position, -1);
        }

        public void EnableInput()
        {
            inputHandler.AddInputLoop(InputLoop);
        }
        public void DisableInput()
        {
            inputHandler.RemoveInputLoop(InputLoop);
        }
    }
}