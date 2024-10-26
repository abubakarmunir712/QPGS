using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using QPGS.Models;

namespace QPGS.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ClassController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public ClassController(ApplicationDbContext context)
        {
            _context = context;
        }
        [Authorize]
        // Add a new class
        [HttpPost("add")]
        public async Task<IActionResult> AddClass([FromBody] Class newClass)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            // Check if the teacher (admin) with the specified AdminId exists
            var admin = await _context.AppUsers.FirstOrDefaultAsync(u => u.UserId == newClass.AdminId && u.Role == "teacher");
            if (admin == null)
            {
                return BadRequest(new { error = "Teacher not found or invalid role" });
            }

            // Start a database transaction
            using (var transaction = await _context.Database.BeginTransactionAsync())
            {
                try
                {
                    // Add the class to the database
                    _context.Classes.Add(newClass);
                    await _context.SaveChangesAsync();

                    // Commit the transaction
                    await transaction.CommitAsync();

                    return Ok(new { message = "Class added successfully", classId = newClass.ClassId });
                }
                catch (Exception ex)
                {
                    // Rollback the transaction in case of any failure
                    await transaction.RollbackAsync();
                    return StatusCode(500, new { error = "An error occurred while adding the class", details = ex.Message });
                }
            }
        }

        [Authorize]
        // Delete a class
        [HttpDelete("delete/{id}")]
        public async Task<IActionResult> DeleteClass(int id)
        {
            var classEntity = await _context.Classes.FindAsync(id);
            if (classEntity == null)
            {
                return NotFound(new { error = "Class not found" });
            }

            _context.Classes.Remove(classEntity);
            await _context.SaveChangesAsync();

            return Ok(new { message = "Class deleted successfully" });
        }

        // Edit class details
        [HttpPut("edit-class/{classId}")]
        public async Task<IActionResult> EditClass(int classId, [FromBody] EditClassDto editClassDto)
        {
            var classEntity = await _context.Classes.Include(c => c.Admin).FirstOrDefaultAsync(c => c.ClassId == classId);
            if (classEntity == null)
            {
                return NotFound(new { error = "Class not found" });
            }

            // Update class properties
            if (!string.IsNullOrWhiteSpace(editClassDto.ClassName))
            {
                classEntity.ClassName = editClassDto.ClassName;
            }

            if (!string.IsNullOrWhiteSpace(editClassDto.ClassDescription))
            {
                classEntity.ClassDescription = editClassDto.ClassDescription;
            }

            if (editClassDto.TeacherId.HasValue)
            {
                var teacher = await _context.AppUsers.FirstOrDefaultAsync(u => u.UserId == editClassDto.TeacherId && u.Role == "teacher");
                if (teacher == null)
                {
                    return BadRequest(new { error = "Teacher not found or invalid role" });
                }

                // Assign the new teacher to the class
                classEntity.AdminId = teacher.UserId;
            }

            await _context.SaveChangesAsync();

            return Ok(new { message = "Class details updated successfully" });
        }
        [HttpGet("my-classes")]
        public async Task<IActionResult> GetClassesForCurrentUser()
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

                // Query classes where AdminId matches the userId
                var classes = await _context.Classes
                    .Where(c => c.AdminId == userId)
                    .ToListAsync();

                if (classes == null || classes.Count == 0)
                {
                    return NotFound(new { message = "No classes found for the current user" });
                }

                return Ok(classes);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = "An error occurred while retrieving classes", details = ex.Message });
            }
        }
        [Authorize]
        [HttpGet("all-classes")]
        public async Task<IActionResult> GetAllClasses()
        {
            try
            {
                // Retrieve all classes from the database
                var classes = await _context.Classes
                    .Select(c => new
                    {
                        c.ClassId,
                        c.ClassName,
                        c.ClassDescription,
                        c.AdminId
                    })
                    .ToListAsync();

                if (classes == null || classes.Count == 0)
                {
                    return NotFound(new { message = "No classes found." });
                }

                return Ok(classes);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = "An error occurred while retrieving classes", details = ex.Message });
            }
        }

        // DTO for editing class details
        public class EditClassDto
        {
            public string? ClassName { get; set; }
            public string? ClassDescription { get; set; }
            public int? TeacherId { get; set; }
        }

    }
}
