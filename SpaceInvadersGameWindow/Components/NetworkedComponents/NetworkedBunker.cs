using GameWindow.Components.GameComponents;
using System.Collections.Generic;
using System.Numerics;

namespace GameWindow.Components.NetworkedComponents
{
    internal class NetworkedBunker
    {
        public static NetworkedBunker[] Bunkers = new NetworkedBunker[8];

        public NetworkedBunkerPart[] parts = new NetworkedBunkerPart[8];

        public NetworkedBunker(int BunkerID, bool up)
        {
            GetX(BunkerID, out float x);

            float y = up ? 50 : 2 * MainWindow.referenceSize.Y / 3;
            Bunkers[BunkerID] = this;

            int reverser = up ? -1 : 1;
            parts[0] = new NetworkedBunkerPart(PieceTypes.TopLeft, new Vector2(x, y) + new Vector2(-9, -4 * reverser), BunkerID, up);
            parts[1] = new NetworkedBunkerPart(PieceTypes.BottomLeft, new Vector2(x, y) + new Vector2(-9, 4 * reverser), BunkerID, up);
            parts[2] = new NetworkedBunkerPart(PieceTypes.MiddleTopLeft, new Vector2(x, y) + new Vector2(-3, -4 * reverser), BunkerID, up);
            parts[3] = new NetworkedBunkerPart(PieceTypes.MiddleBottomLeft, new Vector2(x, y) + new Vector2(-3, 4 * reverser), BunkerID, up);
            parts[4] = new NetworkedBunkerPart(PieceTypes.MiddleTopRight, new Vector2(x, y) + new Vector2(3, -4 * reverser), BunkerID, up);
            parts[5] = new NetworkedBunkerPart(PieceTypes.MiddleBottomRight, new Vector2(x, y) + new Vector2(3, 4 * reverser), BunkerID, up);
            parts[6] = new NetworkedBunkerPart(PieceTypes.TopRight, new Vector2(x, y) + new Vector2(9, -4 * reverser), BunkerID, up);
            parts[7] = new NetworkedBunkerPart(PieceTypes.BottomRight, new Vector2(x, y) + new Vector2(9, 4 * reverser), BunkerID, up);
        }
        private static void GetX(int bunkerID, out float x)
        {
            if (bunkerID == 0 || bunkerID == 4)
                x = 0.4f * (MainWindow.referenceSize.X / 2);
            else if (bunkerID == 1 || bunkerID == 5)
                x = 0.8f * (MainWindow.referenceSize.X / 2);
            else if (bunkerID == 2 || bunkerID == 6)
                x = 1.2f * (MainWindow.referenceSize.X / 2);
            else
                x = 1.6f * (MainWindow.referenceSize.X / 2);
        }
        private void Dispose()
        {
            for (int i = 0; i < 8; i++)
                    parts[i]?.Dispose();
        }
        public static void DisposeAll()
        {
            for (int i = 0; i < 8; i++)
                Bunkers[i]?.Dispose();
        }
    }
}