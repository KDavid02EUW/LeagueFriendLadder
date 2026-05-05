using Microsoft.AspNetCore.Components;
    using LeagueFriendLadder.Services;
using LeagueFriendLadder.Models;

namespace LeagueFriendLadder.Services
{


    public class ClientNavigationService
    {
        private readonly NavigationManager _nav;
        private readonly PlayerSessionService _session;

        public ClientNavigationService(NavigationManager nav, PlayerSessionService session)
        {
            _nav = nav;
            _session = session;
        }
        public void viewProfile(LeagueEntryDTO p)
        {
            if (p == null) return;

            _session.SelectedPlayer = p;
            _nav.NavigateTo($"/profile/{p.SummonerName}/{p.Tag}");
        }
    }
}
