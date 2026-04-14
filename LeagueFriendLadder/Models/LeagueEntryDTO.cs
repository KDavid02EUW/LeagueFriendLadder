namespace LeagueFriendLadder.Models
{
    public class LeagueEntryDTO
    {
        public string QueueType { get; set; } = "";
        public string Puuid { get; set; } = "";
        public string SummonerName { get; set; } = "";
        public string Tag { get; set; } = "";
        public string Tier { get; set; } = "";
        public string Rank { get; set; } = "";
        public int LeaguePoints { get; set; } = 0;
        public int Wins { get; set;} = 0;
        public int Losses { get; set; } = 0;
        public Boolean HotStreak { get; set; }
        public double Winrate { get; set; }
        public string Region { get; set; } = "";
    }
}
