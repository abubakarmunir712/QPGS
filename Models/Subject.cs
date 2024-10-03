using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace QPGS.Models
{
    public class Subject
    {
        [Key]
        public int SubjectId { get; set; }
        
        [Required]
        [MaxLength(255)]
        public string SubjectName { get; set; }
        
        [ForeignKey("Class")]
        public int ClassId { get; set; }
        
        public Class Class { get; set; } // Navigation property
    }
}
