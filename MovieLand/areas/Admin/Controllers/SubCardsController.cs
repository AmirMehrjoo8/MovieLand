using Microsoft.AspNetCore.Mvc;
using MovieLand.Models.Context;

namespace MovieLand.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class SubCardsController : Controller
    {
        private MovieLandDbContext _context;
        public SubCardsController(MovieLandDbContext context)
        {
            _context = context;
        }
        public IActionResult Index()
        {
            return View(_context.SubCards);
        }

        public IActionResult EditSubCard(int subCardId, int credit, int price)
        {
            var card = _context.SubCards.Find(subCardId);
            card.Price = price;
            card.Credit = credit;
            _context.Entry(card).State = Microsoft.EntityFrameworkCore.EntityState.Modified;
            _context.SaveChanges();
            return Redirect("/Admin/SubCards");
        }
    }
}
