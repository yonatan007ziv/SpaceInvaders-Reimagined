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

namespace SpaceInvaders.Components.GameComponents
{
    internal class InvaderBullet
    {
        public Transform transform;
        private SpriteRenderer sR;
        private Collider col;
        public InvaderBullet(Vector2 pos)
        {
            transform = new Transform(new Vector2(1, 7) * MainWindow.GlobalTempZoom, pos);
            col = new Collider(this, transform.scale, transform.position);
            sR = new SpriteRenderer(@"Resources\RawFiles\Images\Bullet.png", transform.scale, transform.position);

            transform.AddPositionDel((pos) => sR.SetPosition(pos));
            transform.AddPositionDel((pos) => col.SetPosition(pos));

            transform.AddScaleDel((pos) => sR.SetScale(pos));
            transform.AddScaleDel((pos) => col.SetScale(pos));

            BulletLoop();
        }
        private async void BulletLoop()
        {
            while (this.col.TouchingCollider() == null || this.col.TouchingCollider()!.parent is Invader)
            {
                transform.AddPosY(10);
                await Task.Delay(1000 / 60);
            }
            Collider col = this.col.TouchingCollider()!;
            if (col.parent is Player)
                ((Player)col.parent).Kill();
            Dispose();
        }


        public void Dispose()
        {
            sR.Dispose();
            col.Dispose();
        }
    }
}