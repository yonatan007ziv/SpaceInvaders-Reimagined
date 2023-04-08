using System.Collections.Generic;
using System.Numerics;

namespace GameWindow.Components.GameComponents
{
    internal enum PieceTypes
    {
        TopLeft,
        BottomLeft,
        MiddleTopLeft,
        MiddleBottomLeft,
        MiddleTopRight,
        MiddleBottomRight,
        TopRight,
        BottomRight,
    }

    internal class Bunker
    {
        public static List<Bunker> AllBunkers = new List<Bunker>();
        public List<BunkerPart> parts = new List<BunkerPart>();

        public Bunker(Vector2 pos)
        {
            AllBunkers.Add(this);
            parts.Add(new BunkerPart(PieceTypes.TopLeft, pos + new Vector2(-9, -4)));
            parts.Add(new BunkerPart(PieceTypes.BottomLeft, pos + new Vector2(-9, 4)));
            parts.Add(new BunkerPart(PieceTypes.MiddleTopLeft, pos + new Vector2(-3, -4)));
            parts.Add(new BunkerPart(PieceTypes.MiddleBottomLeft, pos + new Vector2(-3, 4)));
            parts.Add(new BunkerPart(PieceTypes.MiddleBottomRight, pos + new Vector2(3, 4)));
            parts.Add(new BunkerPart(PieceTypes.MiddleTopRight, pos + new Vector2(3, -4)));
            parts.Add(new BunkerPart(PieceTypes.BottomRight, pos + new Vector2(9, 4)));
            parts.Add(new BunkerPart(PieceTypes.TopRight, pos + new Vector2(9, -4)));
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