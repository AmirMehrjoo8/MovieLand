namespace MovieLand.Models.ViewModels
{
    public class YearlyInComeChartVM
    {
        public List<string> Months { get; set; }
        public List<ChartSubCards> TotalBuyInMonth { get; set; }
    }
}
