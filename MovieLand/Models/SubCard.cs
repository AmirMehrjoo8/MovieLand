using System.ComponentModel.DataAnnotations;

namespace MovieLand.Models
{
    public class SubCard
    {
        [Key]
        public int SubCardId { get; set; }
        [Required]
        public int Credit { get; set; } // چند ماه اعتبار داره
        [Required]
        public decimal Price { get; set; }


        public ICollection<User> Users { get; set; } = new List<User>();
        public ICollection<Transaction> Transactions { get; set; } = new List<Transaction>();
    }
}
