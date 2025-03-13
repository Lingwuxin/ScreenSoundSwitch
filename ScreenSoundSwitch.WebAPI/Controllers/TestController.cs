using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
namespace ScreenSoundSwitch.WebAPI.Controllers
{
    [Route("api/test")]
    [ApiController]
    public class TestController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public TestController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet("check-connection")]
        public IActionResult CheckConnection()
        {
            try
            {
                // 尝试从数据库获取用户数据，如果成功，表示连接成功
                var users = _context.Users.ToList();
                return Ok(new { Message = "Database connection is successful.", Users = users });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Message = "Failed to connect to the database.", Error = ex.Message });
            }
        }
    }
}
