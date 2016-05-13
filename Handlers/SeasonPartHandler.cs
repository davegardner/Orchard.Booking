using Cascade.Booking.Models;
using Orchard.ContentManagement;
using Orchard.ContentManagement.Handlers;
using Orchard.Data;

namespace Cascade.Booking.Handlers
{
    public class SeasonPartHandler : ContentHandler
    {
        public SeasonPartHandler(IRepository<SeasonRecord> seasonRecord, IContentManager contentManager)
        {
            Filters.Add(StorageFilter.For(seasonRecord));
        }

    }
}