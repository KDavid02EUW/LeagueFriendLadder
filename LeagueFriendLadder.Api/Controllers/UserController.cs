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
    }
}