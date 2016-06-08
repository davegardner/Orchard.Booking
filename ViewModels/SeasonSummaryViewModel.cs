using Cascade.Booking.Models;
using Orchard.Core.Common.ViewModels;
using System;

namespace Cascade.Booking.ViewModels
{
    public class SeasonSummaryViewModel
    {
        public int Id { get; set; }
        public  string Title { get; set; }
        public  DateTimeEditor From { get; set; } 
        public  DateTimeEditor To { get; set; } 
        public  Decimal Rate { get; set; }

        public bool IsChecked { get; set; }
    }
}
