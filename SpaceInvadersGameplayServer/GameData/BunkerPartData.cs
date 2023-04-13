namespace GameplayServer.GameData
{
    /// <summary>
    /// Struct to save the Bunker Part's data
    /// </summary>
    internal struct BunkerPartData
    {
        public int BunkerID;
        public BunkerPartType partType;
        public int stage = 1;

        /// <summary>
        /// Constructs a new <see cref="BunkerPartData"/> data
        /// </summary>
        /// <param name="partType"> The type of the part </param>
        /// <param name="BunkerID"> ID of the bunker it belongs to </param>
        public BunkerPartData(BunkerPartType partType, int BunkerID)
        {
            this.partType = partType;
            this.BunkerID = BunkerID;
        }
    }
}