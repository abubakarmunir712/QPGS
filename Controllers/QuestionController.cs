using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using QPGS.Models;
using System.Threading.Tasks;

namespace QPGS.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class QuestionController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public QuestionController(ApplicationDbContext context)
        {
            _context = context;
        }

        // POST: api/questions
        [HttpPost("add")]
        public async Task<IActionResult> AddQuestion([FromBody] Question newQuestion)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            // Check if the chapter exists
            var chapterExists = await _context.Chapters.AnyAsync(c => c.ChapterId == newQuestion.ChapterId);
            if (!chapterExists)
            {
                return BadRequest(new { error = "Chapter not found." });
            }

            // Add the question to the database
            _context.Questions.Add(newQuestion);
            await _context.SaveChangesAsync();

            return Ok(new { message = "Question added successfully", questionId = newQuestion.QuestionId });
        }
    }
}
