using System.ComponentModel.DataAnnotations;

namespace MovieLand.Models.ViewModels
{
    public class LoginVM
    {
        [Required(ErrorMessage = "❗")]
        public string Username { get; set; }


        [Required(ErrorMessage = "❗")]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        public bool RememberMe { get; set; }
    }
}
