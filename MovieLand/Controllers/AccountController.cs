using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using Microsoft.VisualStudio.Web.CodeGenerators.Mvc.Templates.BlazorIdentity.Pages;
using MovieLand.Data.Repository;
using MovieLand.Data.Service;
using MovieLand.Models;
using MovieLand.Models.Context;
using MovieLand.Models.ViewModels;
using System.Security.Claims;
using MovieLand.Classes;
using Microsoft.AspNetCore.Authorization;
using System.Globalization;
using MovieLand.Components;

namespace MovieLand.Controllers
{
    public class AccountController : Controller
    {
        private MovieLandDbContext _context;
        private IUserRepository _userRepository;
        public AccountController(MovieLandDbContext context)
        {
            _context = context;
            _userRepository = new UserRepository(_context);
        }

        public IActionResult Register(RegisterVM registerVM, string lan)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            if (registerVM.Password != registerVM.RePassword)
                return BadRequest(ModelState);

            if (!registerVM.Rules)
                return BadRequest(ModelState);

            if (!registerVM.Phone.IsValidPhoneNumber()) //اعتبارسنجی شماره تلفن
            {
                if (lan == "en-US")
                    TempData["RegisterError"] = "The entered phone number is not valid.";
                if (lan == "fa-IR")
                    TempData["RegisterError"] = "شماره موبایل وارد شده معتبر نیست.";
            }
            else if (_userRepository.UsernameEmailPhoneExists(registerVM.Username, registerVM.Email, registerVM.Phone, -1)) // بررسی تکراری نبودن یوزرنیم،ایمیل یا شماره
                TempData["RegisterError"] = _userRepository.UsernameEmailPhoneWhichExists(registerVM.Username, registerVM.Email, registerVM.Phone, lan, -1);

            //----------------------------------------------------------------------------------------

            if (TempData["RegisterError"] != null) // شماره معتبر نبود یا یوزرنیم، ایمیل، تلفن تکراری بود
            {
                TempData["Name"] = registerVM.Name;
                TempData["Username"] = registerVM.Username;
                TempData["Email"] = registerVM.Email;
                TempData["Phone"] = registerVM.Phone;

                return RedirectToAction("Index", "Home");
            }
            User user = new User()
            {
                Name = registerVM.Name.Trim(),
                Username = registerVM.Username.Trim(),
                Password = registerVM.Password.Trim(),
                Email = registerVM.Email.Trim(),
                Phone = registerVM.Phone.ConvertPhoneTypeTo09_9(),
                RegisterDate = DateTime.Now,
                Type = 0,
                SubCardId = 1
            };

            _userRepository.CreateAccount(user);
            _userRepository.Save();

            TempData["Username"] = user.Username;
            TempData["LoginError"] = " ";
            return Redirect("/");
        }

        public IActionResult Login(LoginVM? loginVM, string? lan)
        {
            if (loginVM.Username == null && loginVM.Password == null && lan == null)
            {
                if (CultureInfo.CurrentUICulture.Name == "en-US")
                {
                    TempData["LoginError"] = "You are not authorized to access this page.";
                    return RedirectToAction("Index", "Home");
                }
                if (CultureInfo.CurrentUICulture.Name == "fa-IR")
                {
                    TempData["LoginError"] = "شما به این صفحه دسترسی ندارید.";
                    return RedirectToAction("Index", "Home");
                }
            }
            if (!ModelState.IsValid)
                return BadRequest();
            if (!_userRepository.AccountExists(loginVM.Username.ConvertPhoneTypeTo09_9(), loginVM.Password))
            {
                if (lan == "en-US")
                {
                    TempData["Username"] = loginVM.Username;
                    TempData["LoginError"] = "Username or password is incorrect.";
                    return RedirectToAction("Index", "Home");
                }
                if (lan == "fa-IR")
                {
                    TempData["Username"] = loginVM.Username;
                    TempData["LoginError"] = "نام کاربری یا رمز عبور اشتباه است.";
                    return RedirectToAction("Index", "Home");
                }
            }
            var user = _userRepository.GetByUsernamePassword(loginVM.Username.ConvertPhoneTypeTo09_9(), loginVM.Password);
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, user.UserId.ToString()),
                new Claim(ClaimTypes.Name, user.Username),
                new Claim("Name", user.Name),
                new Claim("Type", user.Type.ToString())
            };
            var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            var principal = new ClaimsPrincipal(identity);
            var properties = new AuthenticationProperties
            {
                IsPersistent = loginVM.RememberMe
            };
            HttpContext.SignInAsync(principal, properties);
            return Redirect("/");
        }

        [Authorize]
        public IActionResult LogOut()
        {
            HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return Redirect("/");
        }

        [Authorize]
        public IActionResult Profile()
        {
            var user = _userRepository.GetById(int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value));
            ViewBag.SubCard2Credit = _context.SubCards.Find(2).Credit;
            ViewBag.SubCard3Credit = _context.SubCards.Find(3).Credit;
            ViewBag.SubCard4Credit = _context.SubCards.Find(4).Credit;
            if (user.SubExpireDate < DateTime.Now) // اگر تاریخ مهلت اشتراک کاربر تموم شده بود
            {
                if (user.Type == 2)
                    user.Type = 0;
                user.SubCardId = 1;
                _userRepository.EditAccount(user);
                _userRepository.Save();
            }
            return View(user);
        }

        [Authorize]
        [HttpPost]
        public IActionResult EditProfile(EditProfileVM profileVM, IFormFile? ProfilePicture)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            int userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value);
            var user = _userRepository.GetById(userId);
            if (_userRepository.UsernameEmailPhoneExists(profileVM.Username, profileVM.Email, "NoPhones", userId)) // بررسی تکراری نبودن یوزرنیم،ایمیل یا شماره
            {
                TempData["EditProfileError"] = _userRepository.UsernameEmailPhoneWhichExists(profileVM.Username, profileVM.Email, "NoPhones", CultureInfo.CurrentUICulture.Name, userId);
                return Redirect("/Account/Profile");
            }

            if (profileVM.Password != profileVM.RePassword)
            {
                string lan = CultureInfo.CurrentUICulture.Name;
                if (lan == "fa-IR")
                    TempData["EditProfileError"] = "رمز عبور و تکرار آن یکسان نیستند.";
                if (lan == "en-US")
                    TempData["EditProfileError"] = "Password and confirmation do not match.";
                return Redirect("/Account/Profile");
            }

            if (ProfilePicture != null && ProfilePicture.Length > 0)
            {
                UserProfile.SaveProfilePicture(userId, ProfilePicture);
            }

            user.Username = profileVM.Username;
            user.Name = profileVM.Name;
            user.Email = profileVM.Email;
            user.Password = profileVM.Password;

            _userRepository.EditAccount(user);
            _userRepository.Save();

            return Redirect("/Account/Profile");
        }
    }
}
