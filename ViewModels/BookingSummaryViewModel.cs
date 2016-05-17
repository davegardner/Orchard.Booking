using Cascade.Booking.Models;
using System.Collections.Generic;
using System.Linq;

namespace Cascade.Booking.ViewModels
{
    public class BookingSummaryViewModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public IEnumerable<Guest> Guests{ get; set; }
        //public int Year { get; set; }
        public BookingState BookingState { get; set; }
        public decimal? Total
        {
            get { return Guests.Sum(g => g.TotalCost); }
        }

        public bool IsChecked { get; set; }
    }
}
