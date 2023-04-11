using System.IO;
using System.Numerics;

namespace GameplayServer
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
    internal class BunkerData
    {
        public int BunkerID;
        public BunkerPartData[] parts = new BunkerPartData[8];

        public BunkerData(int BunkerID)
        {
            this.BunkerID = BunkerID;
            parts[0] = new BunkerPartData(PieceTypes.TopLeft, BunkerID);
            parts[1] = new BunkerPartData(PieceTypes.BottomLeft, BunkerID);
            parts[2] = new BunkerPartData(PieceTypes.MiddleTopLeft, BunkerID);
            parts[3] = new BunkerPartData(PieceTypes.MiddleBottomLeft, BunkerID);
            parts[4] = new BunkerPartData(PieceTypes.MiddleTopRight, BunkerID);
            parts[5] = new BunkerPartData(PieceTypes.MiddleBottomRight, BunkerID);
            parts[6] = new BunkerPartData(PieceTypes.TopRight, BunkerID);
            parts[7] = new BunkerPartData(PieceTypes.BottomRight, BunkerID);
        }
    }
}