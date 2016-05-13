using System.Collections.Generic;

namespace Cascade.Booking.ViewModels
{
    public class BookingIndexViewModel
    {
        public BookingIndexViewModel() {
            Bookings = new List<BookingSummaryViewModel>();
        }
        public BookingIndexOptions Options { get; set; }
        public IList<BookingSummaryViewModel> Bookings { get; set; }
        public IEnumerable<string> Categories { get; set; }
        public dynamic Pager { get; set; }
    }
}
