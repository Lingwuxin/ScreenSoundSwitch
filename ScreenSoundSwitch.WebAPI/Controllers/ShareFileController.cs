using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ScreenSoundSwitch.WebAPI.Controllers
{
    public class ShareFileController
    {
        [r]
        [Route("admin/[controller]")]
        [ApiController]
        public class ShareAudioReviewController : ControllerBase
        {
            private readonly ApplicationDbContext _context;

            public ShareAudioReviewController(ApplicationDbContext context)
            {
                _context = context;
            }

            // GET: admin/shareaudioreview/pending
            [HttpGet("pending")]
            public async Task<IActionResult> GetPendingFiles()
            {
                var pendingFiles = await _context.shareAudioFIles
                    .Where(f => f.Review == 0)
                    .ToListAsync();

                return Ok(pendingFiles);
            }

            // POST: admin/shareaudioreview/approve/5
            [HttpPost("approve/{id}")]
            public async Task<IActionResult> ApproveFile(int id)
            {
                var file = await _context.shareAudioFIles.FindAsync(id);
                if (file == null)
                    return NotFound();

                file.Review = 1; // 审核通过
                await _context.SaveChangesAsync();

                return Ok(new { message = "审核通过" });
            }

            // POST: admin/shareaudioreview/reject/5
            [HttpPost("reject/{id}")]
            public async Task<IActionResult> RejectFile(int id)
            {
                var file = await _context.shareAudioFIles.FindAsync(id);
                if (file == null)
                    return NotFound();

                file.Review = 2; // 下架
                await _context.SaveChangesAsync();

                return Ok(new { message = "文件已下架" });
            }
        }
    }
}
