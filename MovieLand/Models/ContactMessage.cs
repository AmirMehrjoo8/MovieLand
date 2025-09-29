using System.ComponentModel.DataAnnotations;

namespace MovieLand.Models
{
    public class ContactMessage
    {
        [Key]
        public int MessageId { get; set; }

        [Required]
        public DateTime SentDateTime { get; set; }

        [Required]
        public string Text { get; set; }

        [Required]
        public string Name { get; set; }

        [Required]
        public string Email { get; set; }
    }
}
