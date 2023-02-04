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
using SpaceInvaders.Systems;
using SpaceInvadersGameWindow.Components.UIElements;

namespace SpaceInvaders.Components.GameComponents
{
    internal class InvaderBullet : Bullet
    {
        public InvaderBullet(Vector2 pos, int dir) : base(pos, dir)
        {
            BulletLoop();
        }
        private async void BulletLoop()
        {
            Vector2 SpeedVector = new Vector2(0, bulletSpeed);
            while (this.col.TouchingCollider() == null || this.col.TouchingCollider()!.parent is Invader || this.col.TouchingCollider()!.parent is Bullet)
            {
                NextClip();
                transform.Position += SpeedVector;
                await Task.Delay(1000 / 60);
            }

            Collider col = this.col.TouchingCollider()!;
            if (col.parent is Player)
                ((Player)col.parent).Kill();

            BulletExplosion();
        }
    }
}