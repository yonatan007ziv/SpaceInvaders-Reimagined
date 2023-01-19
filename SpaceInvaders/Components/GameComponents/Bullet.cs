using SpaceInvaders.Components.Miscellaneous;
using SpaceInvaders.Components.PhysicsEngine.Collider;
using SpaceInvaders.Components.Renderer;
using SpaceInvaders.DataStructures;
using SpaceInvaders.Resources;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;

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

            sR = new SpriteRenderer(Sprites.Bullet, pos);
            transform = new Transform(pos, new Vector2(sR.Size) * 5);
            col = new Collider(this, new Vector2(sR.Size), pos);

            transform.AddPositionDel((pos) => sR.SetPosition(pos));
            transform.AddPositionDel((pos) => col.SetPosition(pos));

            transform.AddScaleDel((pos) => sR.SetScale(pos));
            transform.AddScaleDel((pos) => col.SetScale(pos));

            GameWindow.Instance!.Controls.Add(sR);

            BulletLoop();
        }
        async void BulletLoop()
        {
            while (this.col.TouchingCollider() == null)
            {
                transform.AddPosY(-30);
                await Task.Delay(1000 / 30);
            }
            Collider col = this.col.TouchingCollider()!;
            ((ICollidable)col.parent).ColliderHit(this.col);
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