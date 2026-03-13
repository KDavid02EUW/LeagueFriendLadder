namespace LeagueFriendLadder.Models
{
    public class LeagueListDTO
    {
        public string Name { get; set; } = string.Empty;
        public string Tier { get; set; } = string.Empty;
        public int LeaguePoints { get; set; } = 0;
        public string Rank { get; set; } = string.Empty;
        public double Wins { get; set; } = 0;
        public double Losses { get; set; } = 0;

        public double Winrate = 0;
        public string Region { get; set; } = string.Empty;

        public LeagueListDTO() {
            this.Winrate = Math.Round(this.Wins / (this.Wins + this.Losses) * 100);
        }
    }
}
