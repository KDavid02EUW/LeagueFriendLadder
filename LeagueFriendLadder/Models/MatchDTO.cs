namespace LeagueFriendLadder.Models
{
    public class MatchDTO
    {
        public string MatchID { get; set; } = "";
        public List<ParticipantDTO> Participants { get; set; } = new();
    }
    public class ParticipantDTO
    {
        public string championName { get; set; } = "";
        public int champLevel { get; set; } = 0;
        public int deaths { get; set; } = 0;
        public int damageDealtToTurrets { get; set; } = 0;
        public int totalDamageDealtToChampions { get; set; } = 0;
        public int totalDamageTaken { get; set; } = 0;
        public int damageDealtToObjectives { get; set; } = 0;
        public int damageSelfMitigated { get; set; } = 0;
        public int controlWardsPlaced { get; set; } = 0;
        public int dragonKills { get; set; } = 0;
        public int goldEarned { get; set; } = 0;
        public string teamPosition { get; set; } = "";
        public int item0 { get; set; } = 0;
        public int item1 { get; set; } = 0;
        public int item2 { get; set; } = 0;
        public int item3 { get; set; } = 0;
        public int item4 { get; set; } = 0;
        public int item5 { get; set; } = 0;
        public int item6 { get; set; } = 0;
        public int kills { get; set; } = 0;
        public int largestCrit { get; set; } = 0;
        public int largestMultiKill { get; set; } = 0;
        public int summoner1ID { get; set; } = 0;
        public int summoner2ID { get; set; } = 0;
        public string riotIDGameName { get; set; } = "";
        public string riotIDTagLine { get; set; } = "";
        public int totalHeal { get; set; } = 0;
        public int visionScore { get; set; } = 0;
        public int visionWardsBought { get; set; } = 0;
        public int assists { get; set; } = 0;
        public bool win { get; set; } = false;
    }
}
