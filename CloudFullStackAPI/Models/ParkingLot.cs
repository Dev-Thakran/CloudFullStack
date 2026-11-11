using CloudFullStackAPI.Models;

namespace CloudFullStackAPI.Models
{
    public class ParkingLot
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Address { get; set; }
        public decimal Latitude { get; set; }
        public decimal Longitude { get; set; }
        public string Description { get; set; }
        public bool IsApproved { get; set; }
        public string OwnerId { get; set; }
        public List<Bay> Bays { get; set; } = new();
        public List<Review> Reviews { get; set; } = new();
    }
}