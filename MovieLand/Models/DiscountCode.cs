using System.ComponentModel.DataAnnotations;

namespace MovieLand.Models
{
    public class DiscountCode
    {
        [Key]
        public string TheDiscountCode { get; set; }
        [Required]
        public DateTime ExpireDateTime { get; set; }
        [Required]
        public int TotalUsed { get; set; }
        [Required]
        public int MaxUsers { get; set; }
        [Required]
        public int DiscountPercent { get; set; }
    }
}
