using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace QPGS.Models
{
    public class Question
    {
        [Key]
        public int QuestionId { get; set; }

        [Required]
        public string? QuestionText { get; set; }

        public string AnswerText {get;set;}

        [ForeignKey("Chapter")]
        public int ChapterId { get; set; }

        public string? Type { get; set; }
        public Chapter? Chapter { get; set; } // Navigation property
    }
}
