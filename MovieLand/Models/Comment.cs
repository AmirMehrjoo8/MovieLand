using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MovieLand.Models
{
    public class Comment
    {
        [Key]
        public int CommentId { get; set; }
        [Required]
        public int UserId { get; set; }

        [ForeignKey("UserId")]
        public User User { get; set; }

        [Required]
        public string PostFormat { get; set; } // فیلم:movie ; سریال:tv
        [Required]
        public int PostId { get; set; }
        [Required]
        public int RepliedCommentId { get; set; } //اگه ریپلای نکرده بود0
        [Required]
        public DateTime AddedDateTime { get; set; }
        [Required]
        public string Text { get; set; }
    }
}
