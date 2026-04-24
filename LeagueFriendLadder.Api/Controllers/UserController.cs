using LeagueFriendLadder.Api.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using BCrypt.Net;
using LeagueFriendLadder.Api.Data;

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
                Console.WriteLine($"User not found: {model.Username}");
                return Unauthorized("Wrong username or password!");
            }
            bool isPasswordOk = BCrypt.Net.BCrypt.Verify(model.Password, user.PasswordHash);

            if (!isPasswordOk)
            {
                return Unauthorized("Wrong password!");
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
    }
}