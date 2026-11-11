namespace CloudFullStackMVC.Models.ViewModels
{
    public class AvailabilityViewModel
    {
        public int LotId { get; set; }
        public DateTime? Start { get; set; }
        public DateTime? End { get; set; }
        public bool Searched { get; set; }
        public List<Bay> AvailableBays { get; set; } = new();
    }
}