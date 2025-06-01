using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.IdentityModel.Tokens.Jwt;

namespace ScreenSoundSwitch.WebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class FilesController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        public FilesController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpPost("upload")]
        [Authorize]
        public IActionResult UploadFile(IFormFile file)
        {
            var token = Request.Headers["Authorization"].ToString().Split(" ")[1];
            var jwtToken = new JwtSecurityTokenHandler().ReadJwtToken(token);

            if (file == null || file.Length == 0)
            {
                return BadRequest("Please select a file to upload.");
            }
            if (file.FileName.EndsWith(".mp3") || file.FileName.EndsWith(".wav") || file.FileName.EndsWith(".wma"))
            {
                return Ok(new { Message = "File uploaded successfully.", file.FileName });
            }
            return StatusCode(401, new { Message = "Please select a valid audio file.", FileType = file.ContentType });
        }

        [HttpGet("download/{fileName}")]
        [Authorize]
        public IActionResult DownloadFile(string fileName)
        {
            var filePath = Path.Combine("Uploads", fileName); // Assuming files are stored in "Uploads" folder
            if (!System.IO.File.Exists(filePath))
            {
                return NotFound(new { Message = "File not found." });
            }

            var fileBytes = System.IO.File.ReadAllBytes(filePath);
            var contentType = "application/octet-stream";
            return File(fileBytes, contentType, fileName);
        }
        [HttpPost("share")]
        [Authorize(Roles = "Admin")]
        public IActionResult ShareFile(string filename,string author)
        {
            return Ok(new {Message="not do" });
        }
    }
}
