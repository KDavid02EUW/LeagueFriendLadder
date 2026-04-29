using BCrypt.Net;
using LeagueFriendLadder.Api.Data;
using LeagueFriendLadder.Api.Models;
using LeagueFriendLadder.Models;
using LeagueFriendLadder.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using static LeagueFriendLadder.Pages.Leaderboard;

namespace LeagueFriendLadder.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly DataContext _context;
        private readonly RiotService _riotService;
        public UserController(DataContext context, RiotService riotService)
        {
            _context = context;
            _riotService = riotService;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register(RegisterModel model)
        {
            if (await _context.Users.AnyAsync(u => u.Username.ToLower() == model.Username.ToLower()))
            {
                return BadRequest("User already exists!");
            }

            var newUser = new User
            {
                Username = model.Username,
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(model.Password),
                IsAdmin = false,
                Summoners = Array.Empty<string>()
            };

            _context.Users.Add(newUser);
            await _context.SaveChangesAsync();

            return Ok(newUser);
        }
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginModel model)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Username == model.Username);

            if (user == null || !BCrypt.Net.BCrypt.Verify(model.Password, user.PasswordHash))
            {
                return Unauthorized("Wrong username or password!");
            }

            return Ok(new
            {
                user.Id,
                user.Username,
                user.IsAdmin,
                user.Summoners
            });
        }
        [HttpGet]
        public async Task<ActionResult<IEnumerable<User>>> GetUsers()
        {
            try
            {
                var users = await _context.Users
                    .Select(u => new User
                    {
                        Id = u.Id,
                        Username = u.Username, 
                        IsAdmin = u.IsAdmin,
                        Summoners = u.Summoners
                    })
                    .ToListAsync();

                return Ok(users);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error handling the request: {ex.Message}");
                return StatusCode(500, "Database error");
            }
        }
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteUser(int id)
        {
            var user = await _context.Users.FindAsync(id);

            if (user == null)
            {
                return NotFound("User not found!");
            }

            _context.Users.Remove(user);
            await _context.SaveChangesAsync();

            return NoContent();
        }
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateUser(int id, [FromBody] User updatedUser)
        {
            if (id != updatedUser.Id) return BadRequest("ID mismatch");

            var user = await _context.Users.FindAsync(id);
            if (user == null) return NotFound();

            user.Username = updatedUser.Username;
            user.IsAdmin = updatedUser.IsAdmin;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException)
            {
                return StatusCode(500, "Error saving database.");
            }

            return NoContent();
        }
        [HttpPost("{userId}/link-summoner")]
        public async Task<IActionResult> LinkSummoner(int userId, [FromBody] LeagueEntryDTO summonerDto)
        {
            var user = await _context.Users.FindAsync(userId);
            if (user == null) return NotFound("User not found.");

            if (user.Summoners.Contains(summonerDto.Puuid))
            {
                return BadRequest("This account is already linked to this user.");
            }

            var updatedSummoners = user.Summoners.ToList();
            updatedSummoners.Add(summonerDto.Puuid);
            user.Summoners = updatedSummoners.ToArray();

            await _context.SaveChangesAsync();
            return Ok(user);
        }
        public class FriendRequestDTO
        {
            public int SenderId { get; set; }
            public string TargetPuuid { get; set; } = string.Empty;
        }
        [HttpPost("friend-request")]
        public async Task<IActionResult> SendRequest(int senderId, string targetPuuid)
        {
            var targetUser = await _context.Users
                .FirstOrDefaultAsync(u => u.Summoners.Contains(targetPuuid));

            if (targetUser == null)
                return NotFound("This user has not registered to this website.");

            if (senderId == targetUser.Id)
                return BadRequest("You cannot send a request to yourself.");

            var existingRelation = await _context.Friends
                .FirstOrDefaultAsync(f => (f.SenderUserId == senderId && f.ReceiverUserId == targetUser.Id) ||
                                           (f.SenderUserId == targetUser.Id && f.ReceiverUserId == senderId));

            if (existingRelation != null)
            {
                if (existingRelation.Status == FriendshipStatus.Blocked)
                {
                    return BadRequest("Cannot send friend request. This user is blocked or you are blocked for him.");
                }

                if (existingRelation.Status == FriendshipStatus.Pending)
                {
                    return BadRequest("You already have a pending request for this user.");
                }

                if (existingRelation.Status == FriendshipStatus.Accepted)
                {
                    return BadRequest("You are already friends.");
                }
            }

            var request = new Friend
            {
                SenderUserId = senderId,
                ReceiverUserId = targetUser.Id,
                Status = FriendshipStatus.Pending 
            };

            _context.Friends.Add(request);
            await _context.SaveChangesAsync();

            return Ok("Friend request sent!");
        }
        [HttpGet("{userId}/requests")]
        public async Task<IActionResult> GetFriendRequests(int userId)
        {
            var requests = await _context.Friends
                .Where(f => f.ReceiverUserId == userId && f.Status == FriendshipStatus.Pending)
                .Select(f => new {
                    f.Id,
                    SenderId = f.SenderUserId,
                    SenderName = _context.Users
                        .Where(u => u.Id == f.SenderUserId)
                        .Select(u => u.Username)
                        .FirstOrDefault()
                })
                .ToListAsync();

            return Ok(requests);
        }

        [HttpPost("accept-request/{requestId}")]
        public async Task<IActionResult> AcceptFriendRequest(int requestId)
        {
            var friendRequest = await _context.Friends.FindAsync(requestId);

            if (friendRequest == null)
                return NotFound("Request not found");

            friendRequest.Status = FriendshipStatus.Accepted;
            await _context.SaveChangesAsync();

            return Ok();
        }
        [HttpDelete("decline-request/{requestId}")]
        public async Task<IActionResult> DeclineFriendRequest(int requestId)
        {
            var friendRequest = await _context.Friends.FindAsync(requestId);

            if (friendRequest == null)
                return NotFound("Request not found.");

            _context.Friends.Remove(friendRequest);
            await _context.SaveChangesAsync();

            return NoContent();
        }
        [HttpPost("block-user/{requestId}")]
        public async Task<IActionResult> BlockUser(int requestId)
        {
            var friendship = await _context.Friends.FindAsync(requestId);

            if (friendship == null)
                return NotFound("Connection not found");
            friendship.Status = FriendshipStatus.Blocked;

            await _context.SaveChangesAsync();
            return Ok();
        }
        [HttpGet("{userId}/leaderboard")]
        public async Task<IActionResult> GetLeaderboard(int userId)
        {
            // 1. Barátok és a saját ID összegyűjtése
            var friendships = await _context.Friends
                .Where(f => (f.SenderUserId == userId || f.ReceiverUserId == userId)
                             && f.Status == FriendshipStatus.Accepted)
                .ToListAsync();

            var userIds = friendships
                .Select(f => f.SenderUserId == userId ? f.ReceiverUserId : f.SenderUserId)
                .ToList();
            userIds.Add(userId);

            // 2. Lekérjük a felhasználókat az adatbázisból
            var users = await _context.Users
                .Where(u => userIds.Contains(u.Id))
                .ToListAsync();

            var leaderboard = new List<LeaderboardEntryVM>();

            foreach (var user in users)
            {
                if (user.Summoners == null || user.Summoners.Length == 0)
                {
                    leaderboard.Add(new LeaderboardEntryVM { Username = user.Username });
                    continue;
                }

                foreach (var puuid in user.Summoners)
                {
                    if (string.IsNullOrEmpty(puuid)) continue;

                    var dbSummoner = await _context.Summoners
                        .FirstOrDefaultAsync(s => s.puuid == puuid);
                    string region = dbSummoner?.region ?? "EUW1";

                    var rankInfo = await _riotService.GetSummonerDetailsByPuuidAsync(puuid, region);
                    var riotAccount = await _riotService.GetRiotIDByPuuid(puuid);

                    if (rankInfo != null)
                    {
                        leaderboard.Add(new LeaderboardEntryVM
                        {
                            Username = user.Username,
                            RiotName = riotAccount != null ? $"{riotAccount.GameName}#{riotAccount.TagLine}" : "Unknown",
                            Tier = rankInfo.Tier,
                            Rank = rankInfo.Rank,
                            LeaguePoints = rankInfo.LeaguePoints,
                            Winrate = rankInfo.Winrate,
                            Wins = rankInfo.Wins,
                            Losses = rankInfo.Losses,
                            GamesPlayed = rankInfo.Wins + rankInfo.Losses
                        });
                    }
                    else
                    {
                        leaderboard.Add(new LeaderboardEntryVM
                        {
                            Username = user.Username,
                            RiotName = riotAccount != null ? $"{riotAccount.GameName}#{riotAccount.TagLine}" : "Unknown",
                            Tier = "UNRANKED"
                        });
                    }
                }
            }
            return Ok(leaderboard.OrderByDescending(x => GetTierOrder(x.Tier))
                                 .ThenByDescending(x => x.LeaguePoints));
        }
        private int GetTierOrder(string tier)
        {
            return tier.ToUpper() switch
            {
                "CHALLENGER" => 9,
                "GRANDMASTER" => 8,
                "MASTER" => 7,
                "DIAMOND" => 6,
                "EMERALD" => 5,
                "PLATINUM" => 4,
                "GOLD" => 3,
                "SILVER" => 2,
                "BRONZE" => 1,
                "IRON" => 0,
                _ => -1
            };
        }
    }
}