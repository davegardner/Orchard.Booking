using Cascade.Booking.Models;
using Orchard.ContentManagement;
using Orchard.ContentManagement.Handlers;
using Orchard.Data;

namespace Cascade.Booking.Handlers
{
    public class BookingPartHandler : ContentHandler
    {
        public BookingPartHandler(IRepository<BookingRecord> bookingRepository, IContentManager contentManager)
        {
            Filters.Add(StorageFilter.For(bookingRepository));
        }

    }
}