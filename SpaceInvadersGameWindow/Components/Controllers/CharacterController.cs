﻿using SpaceInvaders.Components.GameComponents;
using SpaceInvaders.Components.Miscellaneous;
using SpaceInvaders.Components.PhysicsEngine.Collider;
using SpaceInvaders.Systems;
using SpaceInvadersGameWindow;
using System;
using System.Numerics;
using System.Windows.Input;

namespace SpaceInvaders.Components.Controllers
{
    internal class CharacterController
    {
        InputHandler inputHandler;
        Transform transform;
        Collider col;
        Action looper;
        public CharacterController(Transform transform, Collider col)
        {
            inputHandler = InputHandler.instance!;
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