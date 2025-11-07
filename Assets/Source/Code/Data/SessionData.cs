namespace Source.Code.Data
{
    /// <summary>
    /// Data that is stored during the session and is not saved.
    /// </summary>
    public class SessionData
    {
        public int ActiveShipsCount;
        public int ShipSpeed;
        
        public int GrayTeamScore;
        public int RedTeamScore;
        
        public float ResourceSpawnTimer;
        public float ResourceSpawnInterval = 1;
        
        public bool IsHowStarshipsPath;
    }
}