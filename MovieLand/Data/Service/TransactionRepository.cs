using Microsoft.EntityFrameworkCore;
using MovieLand.Data.Repository;
using MovieLand.Models;
using MovieLand.Models.Context;
using MovieLand.Models.ViewModels;
using System.Globalization;

namespace MovieLand.Data.Service
{
    public class TransactionRepository : ITransactionRepository
    {
        private MovieLandDbContext _context;
        private IUserRepository _userRepository;
        public TransactionRepository(MovieLandDbContext context)
        {
            _context = context;
            _userRepository = new UserRepository(context);
        }
        public bool AddTransaction(int userId, int subCardId, bool isSuccess, decimal amount)
        {
            try
            {
                var trx = new Transaction() { UserId = userId, SubCardId = subCardId, TrxDateTime = DateTime.Now, IsSuccess = isSuccess, Amount = amount };
                _context.Transactions.Add(trx);

                if (isSuccess)
                {
                    var user = _userRepository.GetById(userId);
                    user.SubCardId = subCardId;
                    user.SubStartDate = DateTime.Now;
                    user.SubExpireDate = DateTime.Now.AddMonths(_context.SubCards.Find(subCardId).Credit);
                    if (user.Type == 0)
                        user.Type = 2;
                    _userRepository.EditAccount(user);
                }
                return true;
            }
            catch { return false; }
        }

        public MonthlyInComeChartVM MonthlyTransaction()
        {
            List<int> days = new List<int>();
            List<ChartSubCards> totalBuyInDay = new List<ChartSubCards>();

            var date = DateTime.Now;
            var dateMonth = date.Month;
            for (int i = 1; i <= date.Day; i++)
                days.Add(i);
            date = date.AddDays(1);
            while (date.Month == dateMonth)
            {
                days.Add(date.Day);
                date = date.AddDays(1);
            }

            foreach (var day in days)
            {
                for (int i = 2; i <= 4; i++)
                {
                    var totalBuy = _context.Transactions.Where(t => t.IsSuccess == true && t.TrxDateTime.Month == dateMonth && t.TrxDateTime.Day == day && t.SubCardId == i).Count();
                    totalBuyInDay.Add(new ChartSubCards() { CardId = i, TotalBuy = totalBuy });
                }
            }
            var monthlyInComeChartVM = new MonthlyInComeChartVM() { Days = days, TotalBuyInDay = totalBuyInDay };
            return monthlyInComeChartVM;
        }

        public YearlyInComeChartVM YearlyTransaction()
        {
            var pc = new PersianCalendar();
            var now = DateTime.Now;
            int currentYear = pc.GetYear(now);

            string[] persianMonthNames = new[]
            {
        "فروردین", "اردیبهشت", "خرداد", "تیر", "مرداد", "شهریور",
        "مهر", "آبان", "آذر", "دی", "بهمن", "اسفند"
    };

            List<string> months = new();
            List<ChartSubCards> totalBuyInMonth = new();

            // تمام تراکنش‌ها را می‌گیریم و در حافظه فیلتر می‌کنیم
            var transactions = _context.Transactions.ToList();

            for (int month = 1; month <= 12; month++)
            {
                months.Add(persianMonthNames[month - 1]);

                for (int cardId = 2; cardId <= 4; cardId++)
                {
                    var totalBuy = transactions
                        .Where(t =>
                        t.IsSuccess == true &&
                            t.SubCardId == cardId &&
                            pc.GetYear(t.TrxDateTime) == currentYear &&
                            pc.GetMonth(t.TrxDateTime) == month
                        )
                        .Count();

                    totalBuyInMonth.Add(new ChartSubCards()
                    {
                        CardId = cardId,
                        TotalBuy = totalBuy
                    });
                }
            }

            return new YearlyInComeChartVM()
            {
                Months = months,
                TotalBuyInMonth = totalBuyInMonth
            };
        }

        public MultiYearIncomeChartVM MultiYearTransaction()
        {
            var pc = new PersianCalendar();
            var transactions = _context.Transactions.ToList(); // تمام تراکنش‌ها را از دیتابیس می‌گیریم

            var startYear = pc.GetYear(transactions.Min(t => t.TrxDateTime)); // سال شروع بر اساس قدیمی‌ترین تراکنش
            var currentYear = pc.GetYear(DateTime.Now);

            List<int> years = new();
            List<ChartSubCards> totalBuyInYear = new();

            for (int year = startYear; year <= currentYear; year++)
            {
                years.Add(year);

                for (int cardId = 2; cardId <= 4; cardId++) // کارت‌های برنزی، نقره‌ای، طلایی
                {
                    var totalBuy = transactions
                        .Where(t =>
                        t.IsSuccess == true &&
                            t.SubCardId == cardId &&
                            pc.GetYear(t.TrxDateTime) == year
                        )
                        .Count();

                    totalBuyInYear.Add(new ChartSubCards
                    {
                        CardId = cardId,
                        TotalBuy = totalBuy
                    });
                }
            }

            return new MultiYearIncomeChartVM
            {
                Years = years,
                TotalBuyInYear = totalBuyInYear
            };
        }

        public IEnumerable<Transaction> GetAll()
        {
            return _context.Transactions.Include(t => t.User).OrderByDescending(t =>t.TrxDateTime);
        }

        public IEnumerable<Transaction> GetSuccessTrxs()
        {
            return _context.Transactions.Where(t => t.IsSuccess == true).Include(t => t.User).OrderByDescending(t => t.TrxDateTime);
        }

        public IEnumerable<Transaction> GetNotSuccessTrxs()
        {
            return _context.Transactions.Where(t => t.IsSuccess == false).Include(t => t.User).OrderByDescending(t => t.TrxDateTime);
        }


        public void Save()
        {
            _context.SaveChanges();
        }
    }
}
