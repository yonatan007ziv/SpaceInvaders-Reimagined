namespace GameplayServer.GameData
{
    /// <summary>
    /// Struct to save the player's data
    /// </summary>
    internal struct PlayerData
    {
        public string username;
        public int xPos;
        public char team;

        /// <summary>
        /// Constructs a new <see cref="PlayerData"/> data
        /// </summary>
        public PlayerData()
        {
            username = "";
            xPos = 75;
            team = '\0';
        }
    }
}