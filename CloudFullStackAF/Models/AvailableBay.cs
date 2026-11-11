using System;
using System.Collections.Generic;
using System.Text;

namespace CloudFullStackAF.Models
{
    internal class AvailableBay
    {
        public int Id { get; set; }
        public string BayCode { get; set; } = string.Empty;
        public string Type { get; set; } = string.Empty;
        public string ChargerSpeed { get; set; } = string.Empty;
        public decimal HourlyRate { get; set; }
    }
}
