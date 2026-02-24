namespace LeagueFriendLadder.Models
{
    public class RiotAccount
    {
        public string Puuid { get; set; }
        public string GameName { get; set; } 
        public string TagLine { get; set; } 

        public RiotAccount(string puuid, string gameName, string tagLine, string accountId)
        {
            Puuid = puuid;
            GameName = gameName;
            TagLine = tagLine;
        }
        public RiotAccount()
        {
        }
    }
}
