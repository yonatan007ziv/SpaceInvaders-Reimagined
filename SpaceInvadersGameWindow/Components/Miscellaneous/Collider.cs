using System.Collections.Generic;
using System.Drawing;
using System.Linq;

namespace GameWindow.Components.Miscellaneous
{
    public enum CollisionLayer
    {
        Wall = 1,
        Bunker,
        Player,
        OnlinePlayer,
        Invader,
        PlayerBullet,
        InvaderBullet,
        TeamA,
        TeamB,
    }

    internal class Collider
    {
        private static List<Collider> AllColliders = new List<Collider>();

        public object parent;
        private CollisionLayer layer;
        private CollisionLayer[] ignoredLayers = new CollisionLayer[8];
        private Transform transform;

        public Collider(Transform transform, object parent, CollisionLayer layer)
        {
            AllColliders.Add(this);
            this.layer = layer;
            this.transform = transform;
            this.parent = parent;
        }
        public Collider? TouchingCollider()
        {
            for (int i = 0; i < AllColliders.Count; i++)
            {
                Collider c = AllColliders[i];
                if (c == this || ignoredLayers.Contains(c.layer)) continue;

                // check collision
                Rectangle thisRect = new Rectangle((int)transform.CenteredPosition.X, (int)transform.CenteredPosition.Y, (int)transform.ActualScale.X, (int)transform.ActualScale.Y);
                Rectangle otherRect = new Rectangle((int)c.transform.CenteredPosition.X, (int)c.transform.CenteredPosition.Y, (int)c.transform.ActualScale.X, (int)c.transform.ActualScale.Y);

                // return first detected collision
                if (thisRect.IntersectsWith(otherRect))
                    return c;
            }
            return null;
        }

        private int currentLayerIndex = 0;
        public void IgnoreLayer(CollisionLayer layer)
        {
            ignoredLayers[currentLayerIndex++] = layer;
        }
        public void Dispose()
        {
            AllColliders.Remove(this);
        }
    }
}