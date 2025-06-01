using System.ComponentModel.DataAnnotations;

namespace ScreenSoundSwitch.WebAPI.Models
{
    public class Admin
    {
        [Key]
        public string UserId { get; set; } = Guid.NewGuid().ToString();

        [Required]
        public string Username { get; set; } = string.Empty;
        [Required]
        public string Email { get; set; } = string.Empty;
        [Required]
        public string PasswordHash { get; set; } = string.Empty;
        [Required]
        public string Role { get; set; } = "Adimin";
    }
    public class AdiminRegisterDto
    {
        public string Username { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
    }

    public class AdiminLoginDto
    {
        public string Email { get; set; }
        public string Password { get; set; }
    }

    public class AdiminJwtResponseDto
    {
        public string Username { get; set; }
        public string Token { get; set; }
        public string Role { get; set; }
    }
}
