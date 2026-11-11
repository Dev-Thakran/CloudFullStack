using System.ComponentModel.DataAnnotations;

namespace CloudFullStackAPI.Models
{
    public class Vehicle
    {
        public int Id { get; set; }
        public string DriverId { get; set; } = string.Empty;
        [Required, StringLength(10)]
        public string Plate { get; set; } = string.Empty;
        public string Make { get; set; } = string.Empty;
        public bool IsElectric { get; set; }
        public List<Booking> Bookings { get; set; } = new();
    }
}
