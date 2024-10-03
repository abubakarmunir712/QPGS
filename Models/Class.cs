using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace QPGS.Models
{
    public class Class
    {
        [Key]
        public int ClassId { get; set; }
        
        [Required]
        [MaxLength(255)]
        public string ClassName { get; set; }
        
        [MaxLength(500)]
        public string ClassDescription { get; set; } // Added class description
        
        [ForeignKey("Admin")]
        public int AdminId { get; set; }
        
        public AppUser Admin { get; set; } // Navigation property
    }
}
