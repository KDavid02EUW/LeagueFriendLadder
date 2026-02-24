namespace LeagueFriendLadder.Models
{
    public class LeagueEntryDTO
    {
        public string QueueType { get; set;}
        public string Puuid { get; set; }
        public string SummonerName { get; set; }
        public string Tag { get; set; }
        public string Tier { get; set;}
        public string Rank { get; set;}
        public int LeaguePoints { get; set;}
        public int Wins { get; set;}
        public int Losses { get; set;}
        public Boolean HotStreak { get; set; }
        public double Winrate { get; set; }
    }
}
