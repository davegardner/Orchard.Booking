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
        protected override void GetItemMetadata(GetContentItemMetadataContext context)
        {
            var part = context.ContentItem.As<BookingPart>();

            if (part != null)
            {
                context.Metadata.Identity.Add("Booking.Name", part.Name);
                context.Metadata.DisplayText = part.Name;
            }
        }

    }
}