using System.Collections.Generic;
using System.Drawing;

namespace GameWindow.Components.Miscellaneous
{
    /// <summary>
    /// Represents the types of available collision layers in the game
    /// </summary>
    public enum CollisionLayer
    {
        Wall,
        Bunker,
        Player,
        OnlinePlayer,
        Invader,
        PlayerBullet,
        InvaderBullet,
        TeamA,
        TeamB,
        BunkerA,
        BunkerB,
    }

    /// <summary>
    /// A class implementing colliders
    /// </summary>
    internal class Collider
    {
        private static readonly List<Collider> AllColliders = new List<Collider>();

        public readonly object parent;
        private readonly Transform transform;
        private readonly CollisionLayer myLayer;
        private readonly List<CollisionLayer> ignoredLayers = new List<CollisionLayer>();

        /// <summary>
        /// Builds a collider and adds it to <see cref="AllColliders"/>
        /// </summary>
        /// <param name="transform"> The parent transform </param>
        /// <param name="parent"> The parent object </param>
        /// <param name="myLayer"> The collider <see cref="CollisionLayer"/> </param>
        public Collider(Transform transform, object parent, CollisionLayer myLayer)
        {
            AllColliders.Add(this);
            this.myLayer = myLayer;
            this.transform = transform;
            this.parent = parent;
        }

        /// <summary>
        /// Collider detection using the <see cref="Rectangle"/> class
        /// </summary>
        /// <returns> The first detected touching <see cref="Collider"/> </returns>
        public Collider? TouchingCollider()
        {
            Rectangle thisRect =
                new Rectangle((int)transform.CenteredPosition.X, (int)transform.CenteredPosition.Y,
                    (int)transform.ActualScale.X, (int)transform.ActualScale.Y);
            for (int i = 0; i < AllColliders.Count; i++)
            {
                Collider c = AllColliders[i];
                if (c == this || ignoredLayers.Contains(c.myLayer)) continue;

                // check collision
                Rectangle otherRect = new Rectangle((int)c.transform.CenteredPosition.X, (int)c.transform.CenteredPosition.Y, (int)c.transform.ActualScale.X, (int)c.transform.ActualScale.Y);

                // return first detected collision
                if (thisRect.IntersectsWith(otherRect))
                    return c;
            }
            return null;
        }

        /// <summary>
        /// Adds a layer to <see cref="ignoredLayers"/>
        /// </summary>
        /// <param name="layer"> The layer to ignore </param>
        public void AddIgnoreLayer(CollisionLayer layer)
        {
            ignoredLayers.Add(layer);
        }

        /// <summary>
        /// Disposes the current collider object
        /// </summary>
        public void Dispose()
        {
            AllColliders.Remove(this);
        }
    }
}