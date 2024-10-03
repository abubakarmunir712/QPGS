using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using QPGS.Models;

[Route("api/[controller]")]
[ApiController]
public class SubjectController : ControllerBase
{
    private readonly ApplicationDbContext _context;

    public SubjectController(ApplicationDbContext context)
    {
        _context = context;
    }

    // POST: api/subjects/add
    [HttpPost("add")]
    public async Task<IActionResult> AddSubject([FromBody] Subject newSubject)
    {
        // Check if the Class exists
        var classEntity = await _context.Classes.FirstOrDefaultAsync(c => c.ClassId == newSubject.ClassId);
        if (classEntity == null)
        {
            return NotFound(new { error = "Class not found" });
        }

        // Validate the model
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        // Add the subject to the database
        _context.Subjects.Add(newSubject);
        await _context.SaveChangesAsync();

        return Ok(new { message = "Subject added successfully", subjectId = newSubject.SubjectId });
    }
}
