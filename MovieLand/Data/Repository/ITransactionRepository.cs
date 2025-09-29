using MovieLand.Components;
using MovieLand.Models;
using MovieLand.Models.ViewModels;

namespace MovieLand.Data.Repository
{
    public interface ITransactionRepository
    {
        public IEnumerable<Transaction> GetAll();
        public IEnumerable<Transaction> GetSuccessTrxs();
        public IEnumerable<Transaction> GetNotSuccessTrxs();
        public bool AddTransaction(int userId, int subCardId, bool isSuccess, decimal amount);
        public MonthlyInComeChartVM MonthlyTransaction();
        public YearlyInComeChartVM YearlyTransaction();
        public MultiYearIncomeChartVM MultiYearTransaction();
        public void Save();
    }
}
