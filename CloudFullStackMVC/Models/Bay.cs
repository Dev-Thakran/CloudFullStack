using CloudFullStackMVC.Models;
using System.ComponentModel.DataAnnotations;

namespace CloudFullStackMVC.Models
{
    public class Bay
    {
        public int Id { get; set; }
        [Required, StringLength(20)]
        [Display(Name = "Bay code")]
        public string BayCode { get; set; } = string.Empty;

        [Required]
        public string Type { get; set; } = string.Empty;

        [Required]
        [Display(Name = "Charger speed")]
        public string ChargerSpeed { get; set; } = "None";

        [Range(0.50, 100)]
        [Display(Name = "Hourly rate ($)")]
        public decimal HourlyRate { get; set; }

        [Display(Name = "Active")]
        public bool IsActive { get; set; } = true;
        public int ParkingLotId { get; set; }
        public ParkingLot? ParkingLot { get; set; }
        public List<Booking> Bookings { get; set; } = new();

    }
}