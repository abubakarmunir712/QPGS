using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using Microsoft.AspNetCore.Identity;
using QPGS.Models;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.EntityFrameworkCore;

namespace QPGS.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly IConfiguration _configuration;
        private readonly PasswordHasher<AppUser> _passwordHasher;

        public AuthController(ApplicationDbContext context, IConfiguration configuration)
        {
            _context = context;
            _configuration = configuration;
            _passwordHasher = new PasswordHasher<AppUser>();
        }

        [HttpPost("signup")]
        public async Task<IActionResult> Signup([FromBody] AppUser user)
        {
            // Check if the username or CNIC already exists
            if (await _context.AppUsers.AnyAsync(u => u.Username == user.Username || u.CNIC == user.CNIC))
            {
                return BadRequest(new { error = "Username or CNIC already exists" });
            }

            // Hash the password before saving
            user.Password = _passwordHasher.HashPassword(user, user.Password);
            _context.AppUsers.Add(user);
            await _context.SaveChangesAsync();
            return Ok(new { message = "User registered successfully!" });
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginModel loginModel)
        {
            // Retrieve the user by CNIC
            var user = await _context.AppUsers.SingleOrDefaultAsync(u => u.CNIC == loginModel.CNIC);

            // Check if the user exists and verify the password
            if (user == null || _passwordHasher.VerifyHashedPassword(user, user.Password, loginModel.Password) != PasswordVerificationResult.Success)
            {
                return Unauthorized(new { error = "Invalid credentials" });
            }

            int hours = loginModel.Remember ? 48 : 4; // Set token expiry based on remember option

            // Generate JWT Token
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(_configuration["Jwt:Key"]);
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new[]
                {
                    new Claim(ClaimTypes.Name, user.Username),
                    new Claim(ClaimTypes.Role, user.Role),
                    new Claim("id", user.UserId.ToString())


                }),
                Expires = DateTime.UtcNow.AddHours(hours),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };
            var token = tokenHandler.CreateToken(tokenDescriptor);
            var tokenString = tokenHandler.WriteToken(token);

            return Ok(new { Token = tokenString, role = user.Role });
        }

        [HttpGet("teachers")]
        public async Task<IActionResult> GetTeachers()
        {
            // Fetch users with the "Teacher" role from the database
            var teachers = await _context.AppUsers
                .Where(u => u.Role == "teacher")
                .Select(u => new
                {
                    u.UserId,
                    u.Username,
                    u.CNIC,
                    u.Role
                })
                .ToListAsync();

            // Check if there are any teachers found
            if (!teachers.Any())
            {
                return NotFound(new { message = "No teachers found." });
            }

            return Ok(teachers);
        }

    }



    public class LoginModel
    {
        public string CNIC { get; set; }
        public string Password { get; set; }
        public bool Remember { get; set; } // True if user wants to stay logged in
    }
}
