using System.ComponentModel.DataAnnotations;
namespace MovieLand.Models.ViewModels
{
    public class EditProfileVM
    {
        [Required(ErrorMessage = "❗")]
        [RegularExpression("^[a-zA-Z0-9._]+$", ErrorMessage = "A-Z a-z 0-9 . _")]
        public string Username { get; set; }


        [Required(ErrorMessage = "❗")]
        public string Name { get; set; }


        [Required(ErrorMessage = "❗")]
        [DataType(DataType.Password)]
        public string Password { get; set; }


        [Required(ErrorMessage = "❗")]
        [DataType(DataType.Password)]
        [Compare("Password", ErrorMessage = "❌")]
        public string RePassword { get; set; }


        [EmailAddress(ErrorMessage = "❌")]
        [Required(ErrorMessage = "❗")]
        public string Email { get; set; }

    }
}
