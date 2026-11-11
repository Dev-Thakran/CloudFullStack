using CloudFullStackAPI.Models;

namespace CloudFullStackAPI.Models
{
    public class Review
    {
        public int Id { get; set; }
        public int ParkingLotId { get; set; }
        public string DriverId { get; set; } = string.Empty;
        public int Rating { get; set; }
        public string? Comment { get; set; }
        public DateTime CreatedUtc { get; set; } = DateTime.UtcNow;
        public ParkingLot? ParkingLot { get; set; }
    }
}