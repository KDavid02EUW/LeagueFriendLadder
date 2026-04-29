using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LeagueFriendLadder.Api.Models
{
    [Table("summoners")]
    public class Summoner
    {
        [Key]
        public string puuid {  get; set; }
        public string name { get; set; }
        public string tag { get; set; }
        public string tier { get; set; }
        public string rank { get; set; }
        public int lp {  get; set; }
        public int win { get; set; }
        public int loss { get; set; }
        public double winrate { get; set; }
        public string region { get; set; }
        public int userid   { get; set; }
    }
}
