using GameWindow.Components.GameComponents;
using GameWindow.Components.Miscellaneous;
using GameWindow.Components.PhysicsEngine.Collider;
using GameWindow.Systems;
using System;
using System.Net.Sockets;
using System.Numerics;
using System.Text;
using System.Windows.Input;

namespace GameWindow.Components.GameComponents.NetworkedComponents
{
    internal class NetworkedCharacterController
    {
        private NetworkedBullet? myBullet = null;
        InputHandler inputHandler;
        Transform transform;
        Collider col;
        Action looper;
        private NetworkStream ns;
        public NetworkedCharacterController(Transform transform, Collider col, NetworkStream ns)
        {
            inputHandler = MainWindow.instance!.inputHandler;
            this.transform = transform;
            this.col = col;
            this.ns = ns;

            looper = InputLoop;
            inputHandler.AddInputLoop(looper);
        }
        private void InputLoop()
        {
            //Touching walls?
            int axis = inputHandler.GetAxis("Horizontal");
            Collider? col = this.col.TouchingCollider();
            if (axis == 1 && (col == null || col.parent != Wall.RightWall) || axis == -1 && (col == null || col.parent != Wall.LeftWall))
            {
                transform.Position += new Vector2(axis, 0);
                SendMessage($"PLAYER POS:{transform.Position.X}");
            }
            if (inputHandler.keysDown.Contains(Key.Space) && myBullet == null)
            {
                myBullet = new NetworkedBullet(transform.Position, OnBulletHit);
                SendMessage($"INITIATE BULLET:({transform.Position.X},{transform.Position.Y})");
            }
        }
        private void OnBulletHit(string msg)
        {
            SendMessage(msg);
            myBullet = null;
        }
        private void SendMessage(string msg)
        {
            byte[] buffer = Encoding.UTF8.GetBytes(msg);
            ns.Write(buffer, 0, buffer.Length);
        }
        public void Dispose()
        {
            inputHandler.RemoveInputLoop(looper);
        }
    }
}
