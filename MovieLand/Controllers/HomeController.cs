using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Localization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.SqlServer.Server;
using Microsoft.VisualStudio.Web.CodeGenerators.Mvc.Templates.BlazorIdentity.Pages.Manage;
using MovieLand.Data.Repository;
using MovieLand.Data.Service;
using MovieLand.Models;
using MovieLand.Models.Context;
using MovieLand.Models.ViewModels;
using System.Diagnostics;
using System.Globalization;
using System.Net;
using System.Security.Claims;
using System.Security.Policy;
using System.Text.Json;

namespace MovieLand.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private MovieLandDbContext _context;
        private ICommentRepository _commentRepository;
        private IUserRepository _userRepository;
        private IContactMsgRepository _contactMsgRepository;

        public HomeController(ILogger<HomeController> logger, MovieLandDbContext context)
        {
            _logger = logger;
            _context = context;
            _commentRepository = new CommentReposetory(_context);
            _userRepository = new UserRepository(_context);
            _contactMsgRepository = new ContactMsgRepository(_context);
        }

        public IActionResult SetLanguage(string culture, string? returnUrl, int? id, string? format, int? categoryId, string? categoryName)
        {
            Response.Cookies.Append(
                CookieRequestCultureProvider.DefaultCookieName,
                CookieRequestCultureProvider.MakeCookieValue(new RequestCulture(culture)),
                new CookieOptions { Expires = DateTimeOffset.UtcNow.AddYears(1) }
            );
            ViewBag.Id = id;
            ViewBag.Format = format;

            ViewBag.CategoryId = categoryId;
            ViewBag.CategoryName = categoryName;
            if (id != null && format != null)
            {
                returnUrl = $"/{format}?id={id}&format={format}";
            }
            if (categoryId != null && categoryName != null)
            {
                string encodedCategoryName = Uri.EscapeDataString(categoryName);
                returnUrl = $"/Category?categoryId={categoryId}&categoryName={encodedCategoryName}";
            }
            if (returnUrl == null)
            {
                returnUrl = "/";
            }
            return Redirect(returnUrl);
        }

        public IActionResult Index()
        {
            TempData["DownloadErr"] = "";
            return View();
        }

        [Route("AboutUs")]
        public IActionResult AboutUs()
        {
            return View();
        }

        public IActionResult ContactMsg(string name, string email, string messageText)
        {
            if (name == null || email == null || messageText == null)
                return BadRequest();
            if (User.Identity.IsAuthenticated)
            {
                var user = _userRepository.GetById(int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value));
                name = user.Name; email = user.Email;
            }
            var lan = CultureInfo.CurrentUICulture.Name;
            var model = new List<string>();
            var contactMsg = new ContactMessage() { Name = name, Email = email, SentDateTime = DateTime.Now, Text = messageText };
            if (_contactMsgRepository.AddMsg(contactMsg))
            {
                switch (lan)
                {
                    case "fa-IR":
                        {
                            model.Add($"{name} عزیز، پیام شما به ادمین وبسایت مووی لند ارسال شد. در اسرع وقت ادمین پاسخ را به ایمیل '{email}' ارسال میکند.");
                            model.Add("text-success");
                            break;
                        }
                    default:
                        {
                            model.Add($"Dear {name}, your message has been sent to the admin of the MovieLand website. The admin will reply to your email '{email}' as soon as possible.");
                            model.Add("text-success");
                            break;
                        }
                }
            }
            else
            {
                switch (lan)
                {
                    case "fa-IR":
                        {
                            model.Add("مشکلی پیش آمده.");
                            model.Add("text-danger");
                            break;
                        }
                    default:
                        {
                            model.Add("Something went wrong.");
                            model.Add("text-danger");
                            break;
                        }
                }
            }
            _contactMsgRepository.Save();
            return View(model);
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }



        [Route("Movie")]
        [Route("Tv")]
        public IActionResult SinglePage(int id, string format)
        {
            string apiKey = "3778bd430a6b5a35d815d210850d537a";
            string url = $"https://api.themoviedb.org/3/{format}/{id}?api_key={apiKey}&language={CultureInfo.CurrentUICulture.Name}";

            try
            {
                using (var client = new WebClient())
                {
                    string json = client.DownloadString(url);
                    var data = JsonDocument.Parse(json).RootElement;

                    string title = "";
                    if (data.TryGetProperty("title", out var movieTitle))
                        title = movieTitle.GetString();
                    else if (data.TryGetProperty("name", out var tvTitle))
                        title = tvTitle.GetString();

                    ViewData["Title"] = title ?? "MovieLand";
                }
            }
            catch (Exception ex)
            {
                ViewData["Title"] = "MovieLand";
            }

            ViewBag.Id = id;
            ViewBag.Format = format;

            if (User.Identity.IsAuthenticated) // اگه لاگین بود
            {
                TempData["DownloadErr"] = "";
                var user = _userRepository.GetById(Convert.ToInt32(User.FindFirst(ClaimTypes.NameIdentifier)?.Value));
                if (user.Type != 1) // اگر ادمین نبود
                {
                    if (user.Type == 2) // اگر کاربر اشتراک داشت
                    {
                        if (user.SubExpireDate < DateTime.Now) // اگر تاریخ مهلت اشتراک کاربر تموم شده بود
                        {
                            user.Type = 0;
                            user.SubCardId = 1;
                            _userRepository.EditAccount(user);
                            _userRepository.Save();
                            ViewBag.DownloadLink = "/Subscription/BuySub";
                            if (CultureInfo.CurrentUICulture.Name == "fa-IR")
                                TempData["DownloadErr"] = "دانلود فقط با اشتراک فعال امکان‌پذیر است.";
                            else
                                TempData["DownloadErr"] = "Subscribe now to unlock movie and series downloads.";
                        }
                        else if (user.SubExpireDate > DateTime.Now) // اگه هنوز کارت اشتراکش اعتبار داشت
                            ViewBag.DownloadLink = "/Download";
                    }
                    else if (user.Type == 0) // اگر کاربر معمولی بود
                    {
                        ViewBag.DownloadLink = "/Subscription/BuySub";
                        if (CultureInfo.CurrentUICulture.Name == "fa-IR")
                            TempData["DownloadErr"] = "دانلود فقط با اشتراک فعال امکان‌پذیر است.";
                        else
                            TempData["DownloadErr"] = "Subscribe now to unlock movie and series downloads.";
                    }
                }
                else if (user.Type == 1) // اگه ادمین بود
                    ViewBag.DownloadLink = "/Download";
            }
            else
            {
                ViewBag.DownloadLink = "/Subscription/BuySub";

                if (CultureInfo.CurrentUICulture.Name == "fa-IR")
                    TempData["DownloadErr"] = "دانلود فقط با اشتراک فعال امکان‌پذیر است.";
                else
                    TempData["DownloadErr"] = "Subscribe now to unlock movie and series downloads.";
            }
            return View();
        }

        [Route("Download")]
        [Authorize]
        public IActionResult Download(string format, string postName, string? season, string? episode, string quality)
        {
            var user = _userRepository.GetById(Convert.ToInt32(User.FindFirst(ClaimTypes.NameIdentifier)?.Value));
            if (user.Type != 1) // اگر ادمین نبود
            {
                if (user.Type == 2 && user.SubExpireDate < DateTime.Now) // اگر کاربر اشتراک داشت// اگر تاریخ مهلت اشتراک کاربر تموم شده بود
                {
                    user.Type = 0;
                    _userRepository.EditAccount(user);
                    _userRepository.Save();
                    return NotFound();
                }
                if (user.Type == 0) // اگر کاربر معمولی بود
                {
                    return NotFound();
                }
            }
            var downloadVM = new DownloadVM() { UserName = user.Name, Format = format, PostName = postName, Season = season, Episode = episode, Quality = quality };

            return View(downloadVM);
        }

        [HttpPost]
        public IActionResult AddComment(string postFormat, int postId, int repliedCommentId, string commentText, string lan)
        {
            if (!User.Identity.IsAuthenticated)
            {
                return BadRequest();
            }
            string url = $"/{postFormat}?id={postId}&format={postFormat}";
            if (commentText == null)
            {
                switch (lan)
                {
                    case "en-US":
                        {
                            if (repliedCommentId == 0)
                                TempData["CommentError"] = "Comment text cannot be empty.";
                            else
                            {
                                TempData["ReplyError"] = "Comment text cannot be empty.";
                                TempData["RepliedCommentId"] = repliedCommentId;
                            }
                            return Redirect(url);
                        }
                    case "fa-IR":
                        {
                            if (repliedCommentId == 0)
                                TempData["CommentError"] = "متن کامنت نمیتواند خالی باشد.";
                            else
                            {
                                TempData["ReplyError"] = "متن کامنت نمیتواند خالی باشد.";
                                TempData["RepliedCommentId"] = repliedCommentId;
                            }
                            return Redirect(url);
                        }
                }
            }

            Comment comment = new Comment()
            {
                UserId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value),
                RepliedCommentId = repliedCommentId,
                PostFormat = postFormat,
                PostId = postId,
                AddedDateTime = DateTime.Now,
                Text = commentText
            };

            _commentRepository.AddComment(comment);
            _commentRepository.Save();
            return Redirect(url);
        }

        [Route("Category")]
        public IActionResult Category(int categoryId, string categoryName)
        {
            ViewBag.CategoryId = categoryId;
            ViewBag.CategoryName = categoryName;
            return View();
        }

        [Route("Rules")]
        public IActionResult Rules() => View();
    }
}
