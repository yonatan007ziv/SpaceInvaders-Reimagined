using SpaceInvaders.Components.PhysicsEngine.Collider;
using SpaceInvaders.Components.Renderer;
using SpaceInvaders.DataStructures;
using SpaceInvaders.Resources;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SpaceInvaders.Components.GameComponents
{
    internal class Wall : ICollidable
    {
        public static List<Wall> walls = new List<Wall>();
        SpriteRenderer sR;
        Collider col;
        public Wall(Vector2 pos, Vector2 size)
        {
            walls.Add(this);

            sR = new SpriteRenderer(Sprites.UFO, pos);
            sR.Size = size.ToSize();
            col = new Collider(this, new Vector2(sR.Size), pos);

            GameWindow.Instance!.Controls.Add(sR);

        }
        public static void CheckWallCollisions()
        {
            foreach (Wall w in walls)
                w.WallCollision();
        }
        public static bool WaitForNext = false;
        public void WallCollision()
        {
            Collider? col = this.col.TouchingCollider();
            Debug.WriteLine(col?.parent is Invader);
            if (col != null && !WaitForNext)
            {
                switch (col.parent)
                {
                    default:
                        break;
                    case Invader i:
                        Invader.MoveInvadersDown();
                        Invader.dir *= -1;
                        WaitForNext = true;
                        break;
                }
            }
        }


        public void ColliderHit(Collider hit)
        {

        }
    }
}
