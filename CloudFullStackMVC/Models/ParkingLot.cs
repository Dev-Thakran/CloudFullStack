using CloudFullStackMVC.Models;
using System.ComponentModel.DataAnnotations;

namespace CloudFullStackMVC.Models
{
    public class ParkingLot
    {
        public int Id { get; set; }
        [Required, StringLength(150)]
        public string Name { get; set; } = string.Empty;

        [Required, StringLength(250)]
        public string Address { get; set; } = string.Empty;

        [Range(-90, 90)]
        public decimal Latitude { get; set; }

        [Range(-180, 180)]
        public decimal Longitude { get; set; }

        [StringLength(500)]
        public string Description { get; set; } = string.Empty;

        [Display(Name = "Approved")]
        public bool IsApproved { get; set; }
        public string OwnerId { get; set; }
        public List<Bay> Bays { get; set; } = new();
    }
}