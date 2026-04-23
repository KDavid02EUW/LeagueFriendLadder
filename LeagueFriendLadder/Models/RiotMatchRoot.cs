namespace LeagueFriendLadder.Models
{
    public class RiotMatchRoot
    {
        public RiotInfo info { get; set; } = new();
    }

    public class RiotInfo
    {

        public long gameEndTimeStamp { get; set; }
        public long gameStartTimeStamp { get; set; }
        public List<ParticipantDTO> participants { get; set; } = new();
    }
}
