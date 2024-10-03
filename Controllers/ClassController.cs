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

        // Add a new class
        [HttpPost("add")]
        public async Task<IActionResult> AddClass([FromBody] Class newClass)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            // Add the class to the database
            _context.Classes.Add(newClass);
            await _context.SaveChangesAsync();

            return Ok(new { message = "Class added successfully", classId = newClass.ClassId });
        }

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

        // DTO for editing class details
        public class EditClassDto
        {
            public string? ClassName { get; set; }
            public string? ClassDescription { get; set; }
            public int? TeacherId { get; set; }
        }

    }
}
