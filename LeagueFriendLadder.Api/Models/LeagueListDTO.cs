namespace LeagueFriendLadder.Models
{
    public class LeagueListDTO
    {
        public string Name { get; set; }
        public string Tier { get; set; }
        public int LeaguePoints { get; set; }
        public string Rank { get; set; }
        public double Wins { get; set; }
        public double Losses { get; set; }

        public double Winrate = 0;
        public string Region { get; set; }

        public LeagueListDTO() {
            this.Winrate = Math.Round(this.Wins / (this.Wins + this.Losses) * 100);
        }
    }
}
