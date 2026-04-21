namespace LeagueFriendLadder.Models
{
    using System.Text.Json.Serialization;

    public class ArenaAugment
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Tooltip { get; set; } = string.Empty;

        [JsonPropertyName("iconLarge")]
        public string IconLarge { get; set; } = string.Empty;

        [JsonPropertyName("iconSmall")]
        public string IconSmall { get; set; } = string.Empty;
    }
    public class ArenaAugmentRoot
    {
        public List<ArenaAugment> Augments { get; set; } = new();
    }
}
