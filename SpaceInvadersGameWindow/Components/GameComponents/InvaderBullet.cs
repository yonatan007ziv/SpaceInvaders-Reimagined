using SpaceInvaders.Components.Miscellaneous;
using SpaceInvaders.Components.PhysicsEngine;
using SpaceInvaders.Components.PhysicsEngine.Collider;
using SpaceInvaders.Components.Renderer;
using SpaceInvadersGameWindow;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using SpaceInvadersGameWindow.Components.GameComponents;

namespace SpaceInvaders.Components.GameComponents
{
    internal class InvaderBullet : Bullet
    {
        public InvaderBullet(Vector2 pos) : base(pos)
        {
            InvaderBulletLoop();
        }
        private async void InvaderBulletLoop()
        {
            while (this.col.TouchingCollider() == null || this.col.TouchingCollider()!.parent is Invader || this.col.TouchingCollider()!.parent is Bullet)
            {
                transform.Position += new Vector2(0, bulletSpeed);
                await Task.Delay(1000 / 60);
            }

            Collider col = this.col.TouchingCollider()!;
            if (col.parent is Player)
                ((Player)col.parent).Kill();
            Dispose();
        }

        public void Dispose()
        {
            sprite.Dispose();
            col.Dispose();
            transform.Dispose();
        }
    }
}