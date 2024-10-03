using System.ComponentModel.DataAnnotations;

namespace QPGS.Models
{
    public class AppUser
    {
        [Key]
        public int UserId { get; set; }
        
        [Required]
        [MaxLength(255)]
        public string Username { get; set; }
        
        [Required]
        public string Password { get; set; }
        
        [Required]
        [MaxLength(20)]
        public string CNIC { get; set; }
        
        [Required]
        public string Role { get; set; } // Consider using an Enum for roles

        [MaxLength(15)]
        public string Cellphone { get; set; }
    }
}
