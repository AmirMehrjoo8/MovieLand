namespace MovieLand.Models.ViewModels
{
    public class MonthlyInComeChartVM
    {
        public List<int> Days { get; set; }
        public List<ChartSubCards> TotalBuyInDay { get; set; }
    }
    public class ChartSubCards
    {
        public int CardId { get; set; }
        public int TotalBuy { get; set; }
    }
}
