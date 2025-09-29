using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MovieLand.Classes;
using MovieLand.Data.Repository;
using MovieLand.Data.Service;
using MovieLand.Models;
using MovieLand.Models.Context;
using MovieLand.Models.ViewModels;
using System.Globalization;
using System.Security.Claims;

namespace MovieLand.areas.Admin.Controllers
{
    [Area("Admin")]
    public class UsersController : Controller
    {
        private MovieLandDbContext _context;
        private IUserRepository _userRepository;
        public UsersController(MovieLandDbContext context)
        {
            _context = context;
            _userRepository = new UserRepository(_context);
        }
        public IActionResult Index()
        {
            return View(_userRepository.GetAll());
        }

        [HttpPost]
        public IActionResult EditProfile(User profile, IFormFile? ProfilePicture)
        {

            var user = _userRepository.GetById(profile.UserId);
            if (_userRepository.UsernameEmailPhoneExists(profile.Username, profile.Email, profile.Phone, profile.UserId)) // بررسی تکراری نبودن یوزرنیم،ایمیل یا شماره
            {
                TempData["EditProfileError"] = " ";
                TempData["EditProfileError-"+user.UserId] = _userRepository.UsernameEmailPhoneWhichExists(profile.Username, profile.Email, profile.Phone, CultureInfo.CurrentUICulture.Name, profile.UserId);
                TempData["color"] = "text-danger";
                TempData["UserId"] = profile.UserId.ToString();
                return Redirect("/Admin/Users");
            }

            if (ProfilePicture != null && ProfilePicture.Length > 0)
            {
                UserProfile.SaveProfilePicture(profile.UserId, ProfilePicture);
            }

            if(user.SubCardId != profile.SubCardId)
            {
                user.SubCardId = profile.SubCardId;
                user.SubStartDate = DateTime.Now;
                user.SubExpireDate = DateTime.Now.AddMonths(_context.SubCards.Find(profile.SubCardId).Credit);
                if (user.Type == 0)
                    user.Type = 2;
            }

            user.Username = profile.Username;
            user.Name = profile.Name;
            user.Email = profile.Email;
            user.Phone = profile.Phone;
            user.Password = profile.Password;
            user.Type = profile.Type;

            _userRepository.EditAccount(user);
            _userRepository.Save();
            if(CultureInfo.CurrentUICulture.Name == "fa-IR")
                TempData["EditProfileError-" + user.UserId] = "پروفایل کاربر با موفقیت ویرایش شد.";
            else
                TempData["EditProfileError-" + user.UserId] = "The users profile edited successfuly";

            TempData["EditProfileError"] = " ";
            TempData["color"] = "text-success";
            TempData["UserId"] = profile.UserId.ToString();
            return Redirect("/Admin/Users");
        }

        public IActionResult Search(string q)
        {
            IEnumerable<User> result =_userRepository.Search(q);
            TempData["resCount"] = result.Count();
            TempData["q"] = q;
            return View("Index", result);
        }
        public IActionResult DeleteAccount(int id)
        {
            _userRepository.DeleteAccount(id);
            _userRepository.Save();
            return Redirect("/Admin/Users");
        }
    }
}
