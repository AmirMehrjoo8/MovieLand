using Microsoft.AspNetCore.Mvc;

namespace MovieLand.areas.Admin.Controllers
{
    [Area("Admin")]
    public class DashbordController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
