using Internal;
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

    [HttpGet("{classId}")]
    public async Task<IActionResult> GetSubjectsForClass(int classId)
    {
        try
        {
            // Get the user ID from claims
            var userIdClaim = User.FindFirst("id");
            if (userIdClaim == null)
            {
                return Unauthorized(new { error = "User ID not found in token" });
            }

            var userId = int.Parse(userIdClaim.Value);

            // Check if the class exists and if the user has access
            var classEntity = await _context.Classes
                .FirstOrDefaultAsync(c => c.ClassId == classId && c.AdminId == userId);

            if (classEntity == null)
            {
                return NotFound(new { error = "Class not found or you do not have access to this class" });
            }

            // Get all subjects for the class
            var subjects = await _context.Subjects
                .Where(s => s.ClassId == classId)
                .ToListAsync();

            if (subjects == null || subjects.Count == 0)
            {
                return NotFound(new { message = "No subjects found for the specified class" });
            }

            return Ok(subjects);
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { error = "An error occurred while retrieving subjects", details = ex.Message });
        }
    }

}
