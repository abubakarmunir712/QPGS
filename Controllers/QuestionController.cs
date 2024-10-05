using Microsoft.AspNetCore.Authorization;
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

        // POST: api/question/add
        [HttpPost("add")]
        [Authorize]
        public async Task<IActionResult> AddQuestion([FromBody] Question question)
        {
            // Check if the chapter exists
            var chapterEntity = await _context.Chapters.FirstOrDefaultAsync(c => c.ChapterId == question.ChapterId);
            if (chapterEntity == null)
            {
                return NotFound(new { error = "Chapter not found" });
            }

            // Validate the model
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            // Add the question to the database
            _context.Questions.Add(question);
            await _context.SaveChangesAsync();

            return Ok(new { message = "Question added successfully", questionId = question.QuestionId });
        }
    }
}
