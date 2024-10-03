using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace QPGS.Models
{
    public class Chapter
    {
        [Key]
        public int ChapterId { get; set; }
        
        [Required]
        [MaxLength(255)]
        public string ChapterName { get; set; }
        
        [ForeignKey("Subject")]
        public int SubjectId { get; set; }
        
        public Subject Subject { get; set; } // Navigation property
    }
}
