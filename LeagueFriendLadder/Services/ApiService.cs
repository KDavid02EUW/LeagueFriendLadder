using LeagueFriendLadder.Models;
using System.Net.Http.Json; // Fontos a PostAsJsonAsync-hez!

namespace LeagueFriendLadder.Services
{
    public class ApiService
    {
        private readonly HttpClient _http;

        public ApiService(HttpClient http)
        {
            _http = http;
        }
        public async Task<(bool Success, string? Error)> SavePlayerToDb(LeagueEntryDTO player)
        {
            try
            {
                var url = $"api/Summoner/save?tag={Uri.EscapeDataString(player.Tag)}&region={player.Region}";

                var response = await _http.PostAsJsonAsync(url, player);

                if (response.IsSuccessStatusCode)
                {
                    Console.WriteLine($"{player.SummonerName} saved to db.");
                    return (true, null);
                }
                else
                {
                    var error = await response.Content.ReadAsStringAsync();
                    return (false, error);
                }
            }
            catch (Exception ex)
            {
                return (false, ex.Message);
            }
        }
    }
}