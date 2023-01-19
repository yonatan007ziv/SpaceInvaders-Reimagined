using SpaceInvaders.Components.GameComponents;
using SpaceInvaders.Components.Miscellaneous;
using SpaceInvaders.Components.PhysicsEngine;
using SpaceInvaders.Components.PhysicsEngine.Collider;
using SpaceInvaders.DataStructures;
using SpaceInvaders.Systems;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace SpaceInvaders.Components.Controllers
{
    internal class CharacterController
    {
        InputHandler inputHandler;
        Transform transform;
        Collider col;
        public CharacterController(Transform transform, Collider col)
        {
            this.transform = transform;
            this.col = col;
            inputHandler = InputHandler.instance!;
            inputHandler.AddInputLoop(() => InputLoop());
        }
        private void InputLoop()
        {
            transform.AddPosX(10 * inputHandler.GetAxis("Horizontal"));
            if (inputHandler.keysDown.Contains(Keys.Space) && Bullet.instance == null)
                new Bullet(new Vector2(transform.DesiredPos.x, transform.DesiredPos.y - transform.scale.y / 2));
        }
    }
}