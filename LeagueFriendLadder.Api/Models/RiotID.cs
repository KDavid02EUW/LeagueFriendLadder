using System.ComponentModel.DataAnnotations;

namespace LeagueFriendLadder.Models
{
    public class RiotID
    {
        [Required]
        public string SummonerName { get; set; }

        [Required]
        public string Region { get; set; }
    }
}
