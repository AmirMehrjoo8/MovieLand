using Microsoft.AspNetCore.Mvc;
using MovieLand.Models.Context;

namespace MovieLand.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class ContactMessagesController : Controller
    {
        private MovieLandDbContext _context;
        public ContactMessagesController(MovieLandDbContext context)
        {
            _context = context;
        }
        public IActionResult Index()
        {
            return View(_context.ContactMessages.OrderByDescending(c => c.SentDateTime));
        }
    }
}
