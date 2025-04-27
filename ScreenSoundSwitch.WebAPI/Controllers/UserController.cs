using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using ScreenSoundSwitch.WebAPI.Models;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace ScreenSoundSwitch.WebAPI.Controllers
{

    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly string _secretKey; // 使用强密钥
        private readonly string _issuer;
        private readonly string _audience;

        public UserController(ApplicationDbContext context, IConfiguration configuration)
        {
            _context = context;
            _secretKey = configuration["JwtSettings:SecretKey"];
            _issuer = configuration["JwtSettings:Issuer"];
            _audience = configuration["JwtSettings:Audience"];
        }

        // 注册接口
        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] UserRegisterDto dto)
        {
            // 检查用户名是否已存在
            if (await _context.Users.AnyAsync(u => u.Username == dto.Username))
                return BadRequest("用户名已存在");

            // 加密密码
            var passwordHash = HashPassword(dto.Password);

            // 创建用户
            var user = new User
            {
                Username = dto.Username,
                PasswordHash = passwordHash,
                Email = dto.Email
            };

            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            return Ok("注册成功");
        }

        // 登录接口
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] UserLoginDto dto)
        {
            // 查找用户
            var user = await _context.Users.SingleOrDefaultAsync(u => u.Email == dto.Email);
            if (user == null || user.PasswordHash != HashPassword(dto.Password))
                return Unauthorized("用户名或密码错误");

            // 生成 JWT 令牌
            var token = GenerateJwtToken(user);

            return Ok(new JwtResponseDto { Username = user.Username ,Token = token });
        }

        // 生成 JWT 令牌
        private string GenerateJwtToken(User user)
        {
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_secretKey));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
            var claims = new[]{ new Claim(ClaimTypes.Name, user.UserId) };
            var token = new JwtSecurityToken(
                claims: claims,
                expires: DateTime.UtcNow.AddHours(100),
                signingCredentials: creds,
                issuer: _issuer,
                audience: _audience);
            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        // 哈希密码（推荐使用 BCrypt 代替）
        private string HashPassword(string password)
        {
            using var sha256 = System.Security.Cryptography.SHA256.Create();
            var bytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
            return Convert.ToBase64String(bytes);
        }
    }
}
