using SpaceInvaders.Components.Miscellaneous;
using SpaceInvaders.Components.PhysicsEngine.Collider;
using SpaceInvaders.Components.Renderer;
using SpaceInvaders.Resources;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace SpaceInvaders.Components.GameComponents
{
    internal class Wall : ICollidable
    {
        public static List<Wall> walls = new List<Wall>();
        Transform transform;
        SpriteRenderer sR;
        Collider col;
        public Wall(Vector2 pos, Vector2 scale)
        {
            walls.Add(this);

            transform = new Transform(pos,scale);
            sR = new SpriteRenderer(transform.position, transform.scale);
            col = new Collider(this, transform.position, transform.scale);
        }
    }
}
