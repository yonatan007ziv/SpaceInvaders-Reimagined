using GameWindow.Components.Miscellaneous;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;

namespace GameWindow.Components.PhysicsEngine.Collider
{
    internal class Collider
    {
        public enum Layers
        {
            Wall = 1,
            Bunker,
            Player,
            OnlinePlayer,
            Invader,
            PlayerBullet,
            OnlinePlayerBullet,
            InvaderBullet,
        }
        private static List<Collider> AllColliders = new List<Collider>();

        private Layers layer;
        private Layers[] IgnoredLayers = new Layers[8];
        private Transform transform;
        public object parent;

        public Collider(Transform transform, object parent, Layers layer)
        {
            AllColliders.Add(this);
            this.layer = layer;
            this.transform = transform;
            this.parent = parent;
        }
        public Collider? TouchingCollider()
        {
            foreach (Collider c in AllColliders)
            {
                if (c == this || IgnoredLayers.Contains(c.layer)) continue;

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
        public void IgnoreLayer(Layers layer)
        {
            IgnoredLayers[currentLayerIndex++] = layer;
        }
        public void Dispose()
        {
            AllColliders.Remove(this);
        }
    }
}