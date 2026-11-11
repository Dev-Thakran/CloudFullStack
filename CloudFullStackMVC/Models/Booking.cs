using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

namespace CloudFullStackMVC.Models
{
    public class Booking
    {
        public int Id { get; set; }
        public int BayId { get; set; }
        public string DriverId { get; set; }
        public DateTime StartUtc { get; set; }
        public DateTime EndUtc { get; set; }
        [ValidateNever]
        public string ConfirmationCode { get; set; } = string.Empty;
        [ValidateNever]
        public string Status { get; set; } = string.Empty;
        public decimal TotalCost { get; set; }
        public DateTime CreatedUtc { get; set; }
        public Bay? Bay { get; set; }
    }
}