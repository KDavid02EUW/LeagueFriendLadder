using LeagueFriendLadder.Models;
using System.Net.Http.Json;

public class RiotService
{
    private readonly HttpClient _http;
    private readonly string apiKey = "RGAPI-15de7229-e9c5-49f8-a1d0-69a63a7bb4ab";

    public RiotService(HttpClient http)
    {
        _http = http;
    }

    public async Task<RiotAccount?> GetRiotID(string riotId)
    {
        var parts = riotId.Split("#");
        if (parts.Length < 2)
        {
            throw new ArgumentException("Invalid format. Use Name#Tag.");
        }

        var summonerName = parts[0];
        var tag = parts[1];

        var url = $"https://europe.api.riotgames.com/riot/account/v1/accounts/by-riot-id/{summonerName}/{tag}?api_key={apiKey}";
        return await _http.GetFromJsonAsync<RiotAccount>(url);
    }

    public async Task<LeagueEntryDTO?> GetSummonerDetailsByPuuidAsync(string puuid, string region)
    {
        var url = $"https://{region.ToLower()}.api.riotgames.com/lol/league/v4/entries/by-puuid/{puuid}?api_key={apiKey}";
        var response = await _http.GetFromJsonAsync<List<LeagueEntryDTO>>(url);

        if (response != null)
        {
            foreach (var entry in response)
            {
                if (entry.QueueType == "RANKED_SOLO_5x5")
                {
                    double games = entry.Wins + entry.Losses;
                    entry.Winrate = Math.Round((entry.Wins / games) * 100);
                    return entry;
                }
            }
        }

        return null;
    }
    public async Task<RiotAccount?> GetRiotIDByPuuid(string puuid)
    {
        var url = $"https://europe.api.riotgames.com/riot/account/v1/accounts/by-puuid/{puuid}?api_key={apiKey}";
        return await _http.GetFromJsonAsync<RiotAccount>(url);
    }
}
