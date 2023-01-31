using SpaceInvaders.Components.Miscellaneous;
using System.Collections.Generic;
using System.Drawing;
using System.Numerics;

namespace SpaceInvaders.Components.PhysicsEngine.Collider
{
    internal class Collider
    {
        private static List<Collider> AllColliders = new List<Collider>();

        private Transform transform;
        public object parent;

        public Collider(Transform transform, object parent)
        {
            AllColliders.Add(this);
            this.transform = transform;
            this.parent = parent;
            transform.OnSizeChanged();
        }
        public Collider? TouchingCollider()
        {
            foreach (Collider c in AllColliders)
            {
                if (c == this) continue;

                // check collision
                Rectangle thisRect = new Rectangle((int)transform.CenteredPosition.X, (int)transform.CenteredPosition.Y, (int)transform.ActualScale.X, (int)transform.ActualScale.Y);
                Rectangle otherRect = new Rectangle((int)c.transform.CenteredPosition.X, (int)c.transform.CenteredPosition.Y, (int)c.transform.ActualScale.X, (int)c.transform.ActualScale.Y);

                // return first detected collision
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