using CloudFullStackAPI.Models;

namespace CloudFullStackAPI.Models
{
    public class Bay
    {
        public int Id { get; set; }
        public string BayCode { get; set; }
        public string Type { get; set; }
        public string ChargerSpeed { get; set; }
        public decimal HourlyRate { get; set; }
        public bool IsActive { get; set; }
        public int ParkingLotId { get; set; }
        public ParkingLot? ParkingLot { get; set; }
        public List<Booking> Bookings { get; set; } = new();

    }
}