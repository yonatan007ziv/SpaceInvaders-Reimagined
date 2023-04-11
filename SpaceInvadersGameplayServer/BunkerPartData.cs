namespace GameplayServer
{
    internal class BunkerPartData
    {
        public int BunkerID;
        public PieceTypes part;
        public int stage = 1;
        public BunkerPartData(PieceTypes type, int BunkerID)
        {
            this.part = type;
            this.BunkerID = BunkerID;
        }
    }
}