using System.ComponentModel.DataAnnotations;

namespace LeagueFriendLadder.Api.Models
{
    public class User
    {
        public int Id { get; set; }
        public string Username { get; set; } = string.Empty;
        public string PasswordHash { get; set; } = string.Empty;
        public bool IsAdmin { get; set; }
        public string[] Summoners { get; set; } = Array.Empty<string>();
    }

    public class RegisterModel
    {
        [Required(ErrorMessage = "The Username is required")]
        [MinLength(3, ErrorMessage = "Username must be at least 3 characters")]
        public string Username { get; set; } = "";

        [Required(ErrorMessage = "Password required")]
        [MinLength(6, ErrorMessage = "The password must be at least 6 characters")]
        public string Password { get; set; } = "";

        [Compare(nameof(Password), ErrorMessage = "The passwords do not match")]
        public string ConfirmPassword { get; set; } = "";
    }
    public class LoginModel
    {
        public string Username { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
    }
}