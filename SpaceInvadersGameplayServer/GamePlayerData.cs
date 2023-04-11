namespace GameplayServer
{
    internal struct GamePlayerData
    {
        public string username;
        public int xPos; // default 50
        public char team; // 'A' or 'B'
        public GamePlayerData()
        {
            username = "";
            xPos = 75;
            team = '\0';
        }
    }
}