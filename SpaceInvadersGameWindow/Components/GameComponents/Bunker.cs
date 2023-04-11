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
        public static Bunker[] AllBunkers = new Bunker[4];
        public BunkerPart[] parts = new BunkerPart[8];

        public Bunker(Vector2 pos)
        {
            parts[0] = new BunkerPart(PieceTypes.TopLeft, pos + new Vector2(-9, -4));
            parts[1] = new BunkerPart(PieceTypes.BottomLeft, pos + new Vector2(-9, 4));
            parts[2] = new BunkerPart(PieceTypes.MiddleTopLeft, pos + new Vector2(-3, -4));
            parts[3] = new BunkerPart(PieceTypes.MiddleBottomLeft, pos + new Vector2(-3, 4));
            parts[4] = new BunkerPart(PieceTypes.MiddleTopRight, pos + new Vector2(3, -4));
            parts[5] = new BunkerPart(PieceTypes.MiddleBottomRight, pos + new Vector2(3, 4));
            parts[6] = new BunkerPart(PieceTypes.TopRight, pos + new Vector2(9, -4));
            parts[7] = new BunkerPart(PieceTypes.BottomRight, pos + new Vector2(9, 4));
        }

        public static void DisposeAll()
        {
            for (int i = 0; i < AllBunkers.Length; i++)
            {
                AllBunkers[i]?.Dispose();
                AllBunkers[i] = null!;
            }
        }

        public void Dispose()
        {
            for (int i = 0; i < parts.Length; i++)
            {
                parts[i].Dispose();
                parts[i] = null!;
            }
        }
    }
}