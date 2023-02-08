using SpaceInvaders.Components.GameComponents;
using SpaceInvaders.Components.Miscellaneous;
using SpaceInvaders.Components.PhysicsEngine.Collider;
using SpaceInvaders.Systems;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace SpaceInvadersGameWindow.Components.GameComponents.NetworkedComponents
{
    internal class NetworkedCharacterController
    {
        InputHandler inputHandler;
        Transform transform;
        Collider col;
        Action looper;
        private NetworkStream ns;
        public NetworkedCharacterController(Transform transform, Collider col, NetworkStream ns)
        {
            inputHandler = InputHandler.instance!;
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
                SendPlayerCoords();
            }
            if (inputHandler.keysDown.Contains(Key.Space) && NetworkedBullet.localBullet == null)
                new NetworkedBullet(transform.Position, ns);
        }
        private void SendPlayerCoords()
        {
            byte[] buffer = Encoding.UTF8.GetBytes($"PLAYER POS:({transform.Position.X},{transform.Position.Y})");
            ns.Write(buffer, 0, buffer.Length);
        }
        public void Dispose()
        {
            inputHandler.RemoveInputLoop(looper);
        }
    }
}
