using Microsoft.AspNetCore.Localization;
using Microsoft.AspNetCore.Mvc;
using MovieLand.Data.Repository;
using MovieLand.Data.Service;
using MovieLand.Models;
using MovieLand.Models.Context;
using System.Globalization;
using System.Text.RegularExpressions;

namespace MovieLand.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class DiscountCodesController : Controller
    {
        private MovieLandDbContext _context;
        private IDiscountCodeRepository _codeRepository;
        public DiscountCodesController(MovieLandDbContext context)
        {
            _context = context;
            _codeRepository = new DiscountCodeRepository(_context);
        }
        public IActionResult Index()
        {
            return View(_codeRepository.GetAll());
        }

        public IActionResult CreateDiscountCode(string code, int percent, DateTime expireDate, int maxUse)
        { 
            var lan = CultureInfo.CurrentUICulture.Name;
            Response.Cookies.Append(
            CookieRequestCultureProvider.DefaultCookieName,
                CookieRequestCultureProvider.MakeCookieValue(new RequestCulture("en-US")),
                new CookieOptions { Expires = DateTimeOffset.UtcNow.AddYears(1) }
            );
            return Redirect($"/Admin/DiscountCodes/CreateDiscount?code={code}&percent={percent}&expireDate={expireDate}&maxUse={maxUse}&lan={lan}");
        }
        public IActionResult CreateDiscount(string code, int percent, DateTime expireDate, int maxUse, string lan)
        {
            if (code == null || percent == null || expireDate == null || maxUse == null)
            {
                if (CultureInfo.CurrentUICulture.Name == "fa-IR")
                    TempData["DiscountCodeError"] = "پر کردن همه فیلد ها الزامی است.";
                else
                    TempData["DiscountCodeError"] = "All fields are required."; 
                return Redirect("/Admin/DiscountCodes");
            } 
            code = code.Trim();
            if (_codeRepository.DiscountCodeExists(code))
            {
                if (CultureInfo.CurrentUICulture.Name == "fa-IR")
                    TempData["DiscountCodeError"] = "کد '" + code + "' از قبل وجود دارد.";
                else
                    TempData["DiscountCodeError"] = "The '" + code + "' code already exists.";
            }
            if (!_codeRepository.DiscountCodeExists(code))
            {
                var discountCode = new DiscountCode() { TheDiscountCode = code, DiscountPercent = percent, ExpireDateTime = expireDate, MaxUsers = maxUse, TotalUsed = 0 };
                _codeRepository.CreateDiscountCode(discountCode);
                _codeRepository.Save();
            }
            Response.Cookies.Append(
           CookieRequestCultureProvider.DefaultCookieName,
               CookieRequestCultureProvider.MakeCookieValue(new RequestCulture(lan)),
               new CookieOptions { Expires = DateTimeOffset.UtcNow.AddYears(1) }
           );
            return Redirect("/Admin/DiscountCodes");
        }

        public IActionResult DeleteDiscountCode(string id)
        {
            _codeRepository.DeleteDiscountCode(id);
            _codeRepository.Save();
            return Redirect("/Admin/DiscountCodes");
        }
    }
}
