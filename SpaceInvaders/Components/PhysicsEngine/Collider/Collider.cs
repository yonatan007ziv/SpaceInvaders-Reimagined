using SpaceInvaders.DataStructures;

namespace SpaceInvaders.Components.PhysicsEngine.Collider
{
    internal class Collider
    {
        private static List<Collider> AllColliders = new List<Collider>();

        public object parent;
        private Vector2 pos;
        private Vector2 size;

        public Collider(object parent, Vector2 size, Vector2 pos)
        {
            this.parent = parent;

            this.size = size;
            this.pos = pos;

            AllColliders.Add(this);
        }

        public void SetPosition(Vector2 pos)
        {
            this.pos = pos;
        }

        public void SetScale(Vector2 size)
        {
            this.size = size;
        }

        public Collider? TouchingCollider()
        {
            foreach (Collider c in AllColliders)
            {
                if (c == this) continue;

                // check collision 
                Rectangle thisRect = new Rectangle(this.pos.x, this.pos.y, this.size.x, this.size.y);
                Rectangle otherRect = new Rectangle(c.pos.x, c.pos.y, c.size.x, c.size.y);

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