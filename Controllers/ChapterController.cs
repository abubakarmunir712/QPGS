using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using QPGS.Models;
using Microsoft.AspNetCore.Authorization;
using System.Linq;
using System.Threading.Tasks;

namespace QPGS.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ChapterController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public ChapterController(ApplicationDbContext context)
        {
            _context = context;
        }

        // POST: api/chapters/add
        [HttpPost("add")]
        // [Authorize(Roles = "admin")]
        public async Task<IActionResult> AddChapter([FromBody] Chapter newChapter)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            // Check if the subject exists
            var subjectExists = await _context.Subjects.AnyAsync(s => s.SubjectId == newChapter.SubjectId);
            if (!subjectExists)
            {
                return BadRequest(new { error = "Subject not found." });
            }

            // Add the chapter to the database
            _context.Chapters.Add(newChapter);
            await _context.SaveChangesAsync();

            return Ok(new { message = "Chapter added successfully", chapterId = newChapter.ChapterId });
        }

        // GET: api/chapters/subject/{subjectId}
        [HttpGet("subject/{subjectId}")]
        [Authorize(Roles = "teacher")]
        public async Task<IActionResult> GetChaptersBySubject(int subjectId)
        {
            // Check if the subject exists
            var subjectExists = await _context.Subjects.AnyAsync(s => s.SubjectId == subjectId);
            if (!subjectExists)
            {
                return NotFound(new { error = "Subject not found." });
            }

            // Retrieve the chapters for the given subject
            var chapters = await _context.Chapters
                .Where(c => c.SubjectId == subjectId)
                .ToListAsync();

            if (chapters == null || chapters.Count == 0)
            {
                return NotFound(new { message = "No chapters found for this subject." });
            }

            return Ok(chapters);
        }
    }
}
