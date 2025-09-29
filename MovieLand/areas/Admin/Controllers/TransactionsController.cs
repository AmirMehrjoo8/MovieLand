using Microsoft.AspNetCore.Mvc;
using MovieLand.Classes;
using MovieLand.Data.Repository;
using MovieLand.Data.Service;
using MovieLand.Models;
using MovieLand.Models.Context;
using MovieLand.Models.ViewModels;
using Stimulsoft.Base;
using Stimulsoft.Report;
using Stimulsoft.Report.Mvc;

namespace MovieLand.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class TransactionsController : Controller
    {
        private MovieLandDbContext _context;
        private ITransactionRepository _transactionRepository;
        public TransactionsController(MovieLandDbContext context)
        {
            _context = context;
            _transactionRepository = new TransactionRepository(_context);
            StiLicense.LoadFromString("6vJhGtLLLz2GNviWmUTrhSqnOItdDwjBylQzQcAOiHl2AD0gPVknKsaW0un+3PuM6TTcPMUAWEURKXNso0e5OJN40hxJjK5JbrxU+NrJ3E0OUAve6MDSIxK3504G4vSTqZezogz9ehm+xS8zUyh3tFhCWSvIoPFEEuqZTyO744uk+ezyGDj7C5jJQQjndNuSYeM+UdsAZVREEuyNFHLm7gD9OuR2dWjf8ldIO6Goh3h52+uMZxbUNal/0uomgpx5NklQZwVfjTBOg0xKBLJqZTDKbdtUrnFeTZLQXPhrQA5D+hCvqsj+DE0n6uAvCB2kNOvqlDealr9mE3y978bJuoq1l4UNE3EzDk+UqlPo8KwL1XM+o1oxqZAZWsRmNv4Rr2EXqg/RNUQId47/4JO0ymIF5V4UMeQcPXs9DicCBJO2qz1Y+MIpmMDbSETtJWksDF5ns6+B0R7BsNPX+rw8nvVtKI1OTJ2GmcYBeRkIyCB7f8VefTSOkq5ZeZkI8loPcLsR4fC4TXjJu2loGgy4avJVXk32bt4FFp9ikWocI9OQ7CakMKyAF6Zx7dJF1nZw");
        }
        public IActionResult Index()
        {
            return View(_transactionRepository.GetAll());
        }

        public IActionResult PrintPage()
        {
            return View();
        }

        public IActionResult Print(string status)
        {
            StiReport report = new StiReport();
            report.Load(StiNetCoreHelper.MapPath(this, "wwwroot/TransactionsReports/Report.mrt"));

            List<TransactionVM> transactions = new List<TransactionVM>();
            switch (status)
            {
                case "All":
                    {
                        foreach (var i in _transactionRepository.GetAll())
                        {
                            string cardName = "";
                            if (i.SubCardId == 4)
                                cardName = "طلایی";
                            else if (i.SubCardId == 3)
                                cardName = "نقره‌ای";
                            else if (i.SubCardId == 2)
                                cardName = "برنز";
                            transactions.Add(new TransactionVM()
                            {
                                FactorId = i.TrxId,
                                Username = i.User.Username,
                                CardName = cardName,
                                Price = (i.Amount / 10).ToString("#,0"),
                                DateTime = ConvertDate.SimpleDateTimeByLanguage(i.TrxDateTime),
                                Status = i.IsSuccess ? "موفق" : "ناموفق"
                            });
                        }
                        break;
                    }
                case "true":
                    {
                        foreach (var i in _transactionRepository.GetSuccessTrxs())
                        {
                            string cardName = "";
                            if (i.SubCardId == 4)
                                cardName = "طلایی";
                            else if (i.SubCardId == 3)
                                cardName = "نقره‌ای";
                            else if (i.SubCardId == 2)
                                cardName = "برنز";
                            transactions.Add(new TransactionVM()
                            {
                                FactorId = i.TrxId,
                                Username = i.User.Username,
                                CardName = cardName,
                                Price = (i.Amount / 10).ToString("#,0"),
                                DateTime = ConvertDate.SimpleDateTimeByLanguage(i.TrxDateTime),
                                Status = i.IsSuccess ? "موفق" : "ناموفق"
                            });
                        }
                        break;
                    }
                case "false":
                    {
                        foreach (var i in _transactionRepository.GetNotSuccessTrxs())
                        {
                            string cardName = "";
                            if (i.SubCardId == 4)
                                cardName = "طلایی";
                            else if (i.SubCardId == 3)
                                cardName = "نقره‌ای";
                            else if (i.SubCardId == 2)
                                cardName = "برنز";
                            transactions.Add(new TransactionVM()
                            {
                                FactorId = i.TrxId,
                                Username = i.User.Username,
                                CardName = cardName,
                                Price = (i.Amount / 10).ToString("#,0"),
                                DateTime = ConvertDate.SimpleDateTimeByLanguage(i.TrxDateTime),
                                Status = i.IsSuccess ? "موفق" : "ناموفق"
                            });
                        }
                        break;
                    }
                default:
                    {
                        foreach (var i in _transactionRepository.GetAll())
                        {
                            string cardName = "";
                            if (i.SubCardId == 4)
                                cardName = "طلایی";
                            else if (i.SubCardId == 3)
                                cardName = "نقره‌ای";
                            else if (i.SubCardId == 2)
                                cardName = "برنز";
                            transactions.Add(new TransactionVM()
                            {
                                FactorId = i.TrxId,
                                Username = i.User.Username,
                                CardName = cardName,
                                Price = (i.Amount / 10).ToString("#,0"),
                                DateTime = ConvertDate.SimpleDateTimeByLanguage(i.TrxDateTime),
                                Status = i.IsSuccess ? "موفق" : "ناموفق"
                            });
                        }
                        break;
                    }
            }
            report.RegData("dt", transactions);
            return StiNetCoreViewer.GetReportResult(this, report);
        }
        public IActionResult ViewerEvent()
        {
            return StiNetCoreViewer.ViewerEventResult(this);
        }
    }


}
