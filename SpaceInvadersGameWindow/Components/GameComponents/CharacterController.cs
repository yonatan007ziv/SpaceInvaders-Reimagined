using GameWindow.Components.GameComponents;
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
        InputHandler inputHandler;
        Transform transform;
        Collider col;
        Action looper;
        public CharacterController(Transform transform, Collider col)
        {
            inputHandler = MainWindow.instance!.inputHandler;
            this.transform = transform;
            this.col = col;

            looper = InputLoop;
            inputHandler.AddInputLoop(looper);
        }
        private void InputLoop()
        {
            //Touching walls?
            int axis = inputHandler.GetAxis("Horizontal");
            Collider? col = this.col.TouchingCollider();
            if (axis == 1 && (col == null || col.parent != Wall.RightWall) ||
                axis == -1 && (col == null || col.parent != Wall.LeftWall))
                transform.Position += new Vector2(axis, 0);
            if (inputHandler.keysDown.Contains(Key.Space) && PlayerBullet.instance == null)
                new PlayerBullet(transform.Position, -1);
        }
        public void Dispose()
        {
            inputHandler.RemoveInputLoop(looper);
        }
    }
}