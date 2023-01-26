using System.Collections.Generic;
using System.Drawing;
using System.Numerics;

namespace SpaceInvaders.Components.PhysicsEngine.Collider
{
    internal class Collider
    {
        private static List<Collider> AllColliders = new List<Collider>();

        public object parent;
        private Vector2 pos;
        private Vector2 scale;

        public Collider(object parent, Vector2 scale, Vector2 pos)
        {
            this.parent = parent;

            SetScale(scale);
            SetPosition(pos);

            AllColliders.Add(this);
        }

        public void SetPosition(Vector2 pos)
        {
            this.pos = pos;
        }

        public void SetScale(Vector2 size)
        {
            this.scale = size;
        }

        public Collider? TouchingCollider()
        {
            foreach (Collider c in AllColliders)
            {
                if (c == this) continue;

                // check collision
                Rectangle thisRect = new Rectangle((int)pos.X, (int)pos.Y, (int)scale.X, (int)scale.Y);
                Rectangle otherRect = new Rectangle((int)c.pos.X, (int)c.pos.Y, (int)c.scale.X, (int)c.scale.Y);

                if (thisRect.IntersectsWith(otherRect))
                    return c;
            }
            return null;
        }

        public void Dispose()
        {
            AllColliders.Remove(this);
        }
    }
}