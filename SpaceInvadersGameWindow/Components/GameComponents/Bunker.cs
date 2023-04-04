using System.Collections.Generic;
using System.Numerics;

namespace GameWindow.Components.GameComponents
{
    internal class Bunker
    {
        public static List<Bunker> AllBunkers = new List<Bunker>();
        public List<BunkerPart> parts = new List<BunkerPart>();

        public Bunker(Vector2 pos)
        {
            AllBunkers.Add(this);
            parts.Add(new BunkerPart(BunkerPart.BunkerParts.TopLeft, pos + new Vector2(-9, -4)));
            parts.Add(new BunkerPart(BunkerPart.BunkerParts.BottomLeft, pos + new Vector2(-9, 4)));
            parts.Add(new BunkerPart(BunkerPart.BunkerParts.MiddleTopLeft, pos + new Vector2(-3, -4)));
            parts.Add(new BunkerPart(BunkerPart.BunkerParts.MiddleBottomLeft, pos + new Vector2(-3, 4)));
            parts.Add(new BunkerPart(BunkerPart.BunkerParts.MiddleBottomRight, pos + new Vector2(3, 4)));
            parts.Add(new BunkerPart(BunkerPart.BunkerParts.MiddleTopRight, pos + new Vector2(3, -4)));
            parts.Add(new BunkerPart(BunkerPart.BunkerParts.BottomRight, pos + new Vector2(9, 4)));
            parts.Add(new BunkerPart(BunkerPart.BunkerParts.TopRight, pos + new Vector2(9, -4)));
        }

        public static void DisposeAll()
        {
            foreach (Bunker b in AllBunkers)
                b.Dispose();
            AllBunkers.Clear();
        }

        public void Dispose()
        {
            foreach (BunkerPart p in parts)
                p.Dispose();
            parts.Clear();
        }
    }
}