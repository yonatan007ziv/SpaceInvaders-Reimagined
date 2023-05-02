using GameWindow.Components.GameComponents;
using System.Data;
using System.Numerics;

namespace GameWindow.Components.NetworkedComponents
{
    /// <summary>
    /// A class implementing a networked bunker
    /// </summary>
    internal class NetworkedBunker
    {
        public static NetworkedBunker[] Bunkers = new NetworkedBunker[8];

        public NetworkedBunkerPart[] parts = new NetworkedBunkerPart[8];

        /// <summary>
        /// Get the x position for the bunker
        /// </summary>
        /// <param name="bunkerID"> Bunker to get X </param>
        private static float GetX(int bunkerID)
        {
            if (bunkerID == 0 || bunkerID == 4)
                return 0.4f * (MainWindow.referenceSize.X / 2);
            else if (bunkerID == 1 || bunkerID == 5)
                return 0.8f * (MainWindow.referenceSize.X / 2);
            else if (bunkerID == 2 || bunkerID == 6)
                return 1.2f * (MainWindow.referenceSize.X / 2);
            else
                return 1.6f * (MainWindow.referenceSize.X / 2);
        }

        /// <summary>
        /// Builds a networked bunker
        /// </summary>
        /// <param name="BunkerID"> Bunker to build </param>
        /// <param name="up"> Whether the bunker should appear at the top of the screen </param>
        public NetworkedBunker(int BunkerID, bool up)
        {
            Bunkers[BunkerID] = this;
            float x = GetX(BunkerID);
            float y = up ? 50 : 2 * MainWindow.referenceSize.Y / 3;

            int reverser = up ? -1 : 1;
            parts[0] = new NetworkedBunkerPart(BunkerPartType.TopLeft, new Vector2(x, y) + new Vector2(-9, -4 * reverser), BunkerID, up);
            parts[1] = new NetworkedBunkerPart(BunkerPartType.BottomLeft, new Vector2(x, y) + new Vector2(-9, 4 * reverser), BunkerID, up);
            parts[2] = new NetworkedBunkerPart(BunkerPartType.MiddleTopLeft, new Vector2(x, y) + new Vector2(-3, -4 * reverser), BunkerID, up);
            parts[3] = new NetworkedBunkerPart(BunkerPartType.MiddleBottomLeft, new Vector2(x, y) + new Vector2(-3, 4 * reverser), BunkerID, up);
            parts[4] = new NetworkedBunkerPart(BunkerPartType.MiddleTopRight, new Vector2(x, y) + new Vector2(3, -4 * reverser), BunkerID, up);
            parts[5] = new NetworkedBunkerPart(BunkerPartType.MiddleBottomRight, new Vector2(x, y) + new Vector2(3, 4 * reverser), BunkerID, up);
            parts[6] = new NetworkedBunkerPart(BunkerPartType.TopRight, new Vector2(x, y) + new Vector2(9, -4 * reverser), BunkerID, up);
            parts[7] = new NetworkedBunkerPart(BunkerPartType.BottomRight, new Vector2(x, y) + new Vector2(9, 4 * reverser), BunkerID, up);
        }

        /// <summary>
        /// Does the bunker exist
        /// </summary>
        /// <returns> Whether the bunker exists or not </returns>
        public static void InitiateEmptyBunkers()
        {
            for (int i = 0; i < Bunkers.Length; i++)
            {
                Bunkers[i] = new NetworkedBunker(i, NetworkedPlayer.localPlayer!.team == 'A' && 4 <= i && i <= 7
                    || NetworkedPlayer.localPlayer.team == 'B' && 0 <= i && i <= 3);
                foreach (NetworkedBunkerPart p in Bunkers[i].parts)
                {
                    p.imagePathIndex = 4;
                    p.Hit();
                }
            }
        }

        /// <summary>
        /// Does the bunker exist
        /// </summary>
        /// <returns> Whether the bunker exists or not </returns>
        public bool BunkerExists()
        {
            if (parts[0] == null) return false;

            foreach (NetworkedBunkerPart part in parts)
                if (1 <= part.imagePathIndex && part.imagePathIndex <= 4)
                    return true;
            return false;
        }

        /// <summary>
        /// Disposes the current <see cref="NetworkedBunker"/>
        /// </summary>
        public void Reset()
        {
            for (int i = 0; i < 8; i++)
                parts[i]?.ResetPart();
        }

        /// <summary>
        /// Disposes the current <see cref="NetworkedBunker"/>
        /// </summary>
        public void Dispose()
        {
            for (int i = 0; i < 8; i++)
                parts[i]?.Dispose();
        }

        /// <summary>
        /// Disposes all <see cref="NetworkedBunker"/>
        /// </summary>
        public static void DisposeAll()
        {
            for (int i = 0; i < 8; i++)
                Bunkers[i]?.Dispose();
        }
    }
}