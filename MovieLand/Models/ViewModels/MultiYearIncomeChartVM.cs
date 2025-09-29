namespace MovieLand.Models.ViewModels
{
    public class MultiYearIncomeChartVM
    {
        public List<int> Years { get; set; }
        public List<ChartSubCards> TotalBuyInYear { get; set; }
    }
}
