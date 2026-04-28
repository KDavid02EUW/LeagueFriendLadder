using BCrypt.Net;
using LeagueFriendLadder.Api.Data;
using LeagueFriendLadder.Api.Models;
using LeagueFriendLadder.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace LeagueFriendLadder.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly DataContext _context;

        public UserController(DataContext context)
        {
            _context = context;
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
                return NotFound("This player has not registered to our website, so he cannot accept friend requests.");

            if (senderId == targetUser.Id)
                return BadRequest("You cannot send a friend request to yourself.");

            var existing = await _context.Friends
                .AnyAsync(f => (f.SenderUserId == senderId && f.ReceiverUserId == targetUser.Id) ||
                               (f.SenderUserId == targetUser.Id && f.ReceiverUserId == senderId));

            if (existing) return BadRequest($"There is already a pending request to that user. {targetUser.Username}");

            var request = new Friend { SenderUserId = senderId, ReceiverUserId = targetUser.Id };
            _context.Friends.Add(request);
            await _context.SaveChangesAsync();

            return Ok("Friend request sent!");
        }
    }
}