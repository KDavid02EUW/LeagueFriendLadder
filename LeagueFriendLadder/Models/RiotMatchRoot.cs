namespace LeagueFriendLadder.Models
{
    public class RiotMatchRoot
    {
        public RiotInfo info { get; set; } = new();
    }

    public class RiotInfo
    {
        public List<ParticipantDTO> participants { get; set; } = new();
    }
}
