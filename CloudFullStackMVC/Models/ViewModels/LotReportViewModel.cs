namespace CloudFullStackMVC.Models.ViewModels
{
    public class LotReportViewModel
    {
        public int LotId { get; set; }
        public string LotName { get; set; } = string.Empty;
        public int TotalBookings { get; set; }
        public decimal TotalRevenue { get; set; }
    }
}