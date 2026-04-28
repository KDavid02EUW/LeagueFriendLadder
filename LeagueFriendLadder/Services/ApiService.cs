using LeagueFriendLadder.Models;
using System.Net.Http.Json;

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
        public async Task<(bool Success, string? Message)> SendFriendRequest(int senderId, string targetPuuid)
        {
            try
            {
                var response = await _http.PostAsync($"api/User/friend-request?senderId={senderId}&targetPuuid={targetPuuid}", null);
                var message = await response.Content.ReadAsStringAsync();

                return (response.IsSuccessStatusCode, message);
            }
            catch (Exception ex)
            {
                return (false, ex.Message);
            }
        }
        public async Task<bool> LinkSummonerToUser(int userId, LeagueEntryDTO summoner)
        {
            var response = await _http.PostAsJsonAsync($"api/User/{userId}/link-summoner", summoner);
            return response.IsSuccessStatusCode;
        }
    }
}