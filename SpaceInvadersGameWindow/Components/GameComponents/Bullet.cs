using SpaceInvaders.Components.Miscellaneous;
using SpaceInvaders.Components.PhysicsEngine;
using SpaceInvaders.Components.PhysicsEngine.Collider;
using SpaceInvaders.Components.Renderer;
using SpaceInvaders.Resources;
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
    internal class Bullet
    {
        public static Bullet? instance;
        public Transform transform;
        private SpriteRenderer sR;
        private Collider col;
        public Bullet(Vector2 pos)
        {
            instance = this;
            transform = new Transform(pos, new Vector2(1, 7) * 5);
            col = new Collider(this, transform.position, transform.scale);
            sR = new SpriteRenderer(@"Resources\RawFiles\Images\Bullet.png", transform.position, transform.scale);

            transform.AddPositionDel((pos) => sR.SetPosition(pos));
            transform.AddPositionDel((pos) => col.SetPosition(pos));

            transform.AddScaleDel((pos) => sR.SetScale(pos));
            transform.AddScaleDel((pos) => col.SetScale(pos));

            BulletLoop();
        }
        private async void BulletLoop()
        {
            while (this.col.TouchingCollider() == null)
            {
                transform.AddPosY(-10);
                await Task.Delay(1000 / 60);
            }
            Dispose();
        }


        public void Dispose()
        {
            sR.Dispose();
            col.Dispose();
            instance = null;
        }
    }
}