using Microsoft.AspNetCore.Mvc;
using MovieLand.Data.Repository;
using MovieLand.Data.Service;
using MovieLand.Models.Context;

namespace MovieLand.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class InComeController : Controller
    {
        private MovieLandDbContext _context;
        private ITransactionRepository _trxRepository;
        public InComeController(MovieLandDbContext context)
        {
            _context = context;
            _trxRepository = new TransactionRepository(_context);
        }
        public IActionResult Index()
        {
            ViewBag.monthly = _trxRepository.MonthlyTransaction();
            ViewBag.yearly = _trxRepository.YearlyTransaction();
            ViewBag.Total = _trxRepository.MultiYearTransaction();
            return View();
        }
    }
}
