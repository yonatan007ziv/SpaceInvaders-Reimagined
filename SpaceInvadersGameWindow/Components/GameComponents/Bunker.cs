using System.Numerics;

namespace GameWindow.Components.GameComponents
{
    /// <summary>
    /// Represents the types of available Bunker Parts in the game
    /// </summary>
    internal enum BunkerPartType
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

    /// <summary>
    /// Class for representing a <see cref="Bunker"/>
    /// </summary>
    internal class Bunker
    {
        public static Bunker[] AllBunkers = new Bunker[4];

        public BunkerPart[] parts = new BunkerPart[8];

        /// <summary>
        /// Builds a <see cref="Bunker"/>
        /// </summary>
        /// <param name="pos"> A <see cref="Vector2"/> representing the position of the bunker on the screen </param>
        public Bunker(Vector2 pos)
        {
            parts[0] = new BunkerPart(BunkerPartType.TopLeft, pos + new Vector2(-9, -4));
            parts[1] = new BunkerPart(BunkerPartType.BottomLeft, pos + new Vector2(-9, 4));
            parts[2] = new BunkerPart(BunkerPartType.MiddleTopLeft, pos + new Vector2(-3, -4));
            parts[3] = new BunkerPart(BunkerPartType.MiddleBottomLeft, pos + new Vector2(-3, 4));
            parts[4] = new BunkerPart(BunkerPartType.MiddleTopRight, pos + new Vector2(3, -4));
            parts[5] = new BunkerPart(BunkerPartType.MiddleBottomRight, pos + new Vector2(3, 4));
            parts[6] = new BunkerPart(BunkerPartType.TopRight, pos + new Vector2(9, -4));
            parts[7] = new BunkerPart(BunkerPartType.BottomRight, pos + new Vector2(9, 4));
        }

        /// <summary>
        /// Disposes of the bunker by disposing its parts
        /// </summary>
        public void Dispose()
        {
            for (int i = 0; i < parts.Length; i++)
            {
                parts[i].Dispose();
                parts[i] = null!;
            }
        }

        /// <summary>
        /// Disposes all <see cref="Bunker"/> objects by iterating through <see cref="AllBunkers"/> and disposing them
        /// </summary>
        public static void DisposeAll()
        {
            for (int i = 0; i < AllBunkers.Length; i++)
            {
                AllBunkers[i]?.Dispose();
                AllBunkers[i] = null!;
            }
        }
    }
}