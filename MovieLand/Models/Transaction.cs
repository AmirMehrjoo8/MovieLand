using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MovieLand.Models
{
    public class Transaction
    {
        [Key]
        public int TrxId { get; set; }
        [Required]
        public int UserId { get; set; }

        [ForeignKey("UserId")]
        public User User { get; set; }


        public int SubCardId { get; set; }

        [ForeignKey("SubCardId")]
        public SubCard SubCard { get; set; }


        [Required]
        public DateTime TrxDateTime { get; set; }
        [Required]
        public bool IsSuccess { get; set; }

        [Required]
        public decimal Amount { get; set; }
    }
}
