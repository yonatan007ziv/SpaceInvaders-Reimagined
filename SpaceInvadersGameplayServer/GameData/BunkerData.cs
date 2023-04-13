namespace GameplayServer.GameData
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
    /// Struct to save the Bunker's data
    /// </summary>
    internal struct BunkerData
    {
        public int BunkerID;
        public BunkerPartData[] parts = new BunkerPartData[8];
        
        /// <summary>
        /// Constructs a new <see cref="BunkerData"/> data
        /// </summary>
        /// <param name="BunkerID"> The bunker ID between 0 (inclusive) and 7 (inclusive) </param>
        public BunkerData(int BunkerID)
        {
            this.BunkerID = BunkerID;
            parts[0] = new BunkerPartData(BunkerPartType.TopLeft, BunkerID);
            parts[1] = new BunkerPartData(BunkerPartType.BottomLeft, BunkerID);
            parts[2] = new BunkerPartData(BunkerPartType.MiddleTopLeft, BunkerID);
            parts[3] = new BunkerPartData(BunkerPartType.MiddleBottomLeft, BunkerID);
            parts[4] = new BunkerPartData(BunkerPartType.MiddleTopRight, BunkerID);
            parts[5] = new BunkerPartData(BunkerPartType.MiddleBottomRight, BunkerID);
            parts[6] = new BunkerPartData(BunkerPartType.TopRight, BunkerID);
            parts[7] = new BunkerPartData(BunkerPartType.BottomRight, BunkerID);
        }
    }
}