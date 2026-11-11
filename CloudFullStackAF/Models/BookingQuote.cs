using System;
using System.Collections.Generic;
using System.Text;

namespace CloudFullStackAF.Models
{
    internal class BookingQuote
    {
        public int BayId { get; set; }
        public string BayCode { get; set; } = string.Empty;
        public decimal HourlyRate { get; set; }
        public decimal Hours { get; set; }
        public decimal TotalCost { get; set; }
    }
}
