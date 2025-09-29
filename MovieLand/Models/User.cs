using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MovieLand.Models
{
    public class User
    {
        [Key]
        public int UserId { get; set; }
        [Required]
        public int SubCardId { get; set; }

        [ForeignKey("SubCardId")]
        public SubCard SubCard { get; set; } 


        [Required]
        public string Username { get; set; }
        [Required]
        public string Name { get; set; }
        [Required]
        public string Password { get; set; }
        [Required]
        public string Email { get; set; }
        [Required]
        public string Phone { get; set; }
        [Required]
        public DateTime RegisterDate { get; set; }
        [Required]
        public int Type { get; set; }
        public DateTime SubStartDate { get; set; }
        public DateTime SubExpireDate { get; set; }



        public ICollection<Comment> Comments { get; set; } = new List<Comment>();
        public ICollection<Transaction> Transactions { get; set; } = new List<Transaction>();
    }
}
