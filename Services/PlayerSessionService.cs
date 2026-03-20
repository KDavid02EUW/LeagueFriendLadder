using Microsoft.JSInterop;
using System.Text.Json;
using LeagueFriendLadder.Models;

namespace LeagueFriendLadder.Services
{
    public class PlayerSessionService
    {
        private readonly IJSRuntime _js;
        private LeagueEntryDTO? _selectedPlayer;
        private const string StorageKey = "selected_player_data";

        public PlayerSessionService(IJSRuntime js)
        {
            _js = js;
        }

        public LeagueEntryDTO? SelectedPlayer
        {
            get => _selectedPlayer;
            set
            {
                _selectedPlayer = value;
                SaveToStorage(value);
            }
        }

        private async void SaveToStorage(LeagueEntryDTO? player)
        {
            if (player == null) return;
            try
            {
                var json = JsonSerializer.Serialize(player);
                await _js.InvokeVoidAsync("sessionStorage.setItem", StorageKey, json);
            }
            catch { Exception e = new Exception("Interop error"); }
        }

        public async Task<LeagueEntryDTO?> GetPlayerAsync()
        {
            if (_selectedPlayer != null) return _selectedPlayer;

            try
            {
                var json = await _js.InvokeAsync<string>("sessionStorage.getItem", StorageKey);
                if (!string.IsNullOrEmpty(json))
                {
                    _selectedPlayer = JsonSerializer.Deserialize<LeagueEntryDTO>(json);
                }
            }
            catch { return null; }

            return _selectedPlayer;
        }
    }
}