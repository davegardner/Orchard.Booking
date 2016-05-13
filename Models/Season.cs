using Orchard.ContentManagement;
using Orchard.ContentManagement.Aspects;
using Orchard.ContentManagement.Records;
using System;

namespace Cascade.Booking.Models
{
    public class SeasonRecord : ContentPartRecord
    {
        // a season is any period (eg shoulder, high, low, etc) defined by a unique date range
        public virtual string Title { get; set; }
        public virtual DateTime? FromDate { get; set; } // don't call it 'From' nHibernate blows up
        public virtual DateTime? ToDate { get; set; } // don't call it 'To' nHibernate blows up
        public virtual Decimal Rate { get; set; }
    }

    public class SeasonPart : ContentPart<SeasonRecord> //, ITitleAspect
    {
        public string Title
        {
            get { return Retrieve(r => r.Title); }
            set { Store(r => r.Title, value); }
        }

        public DateTime From
        {
            get { return Retrieve(r => r.FromDate) ?? DateTime.MinValue; }
            set { Store(r => r.FromDate, value); }
        }

        public DateTime To
        {
            get { return Retrieve(r => r.ToDate) ?? DateTime.MinValue; }
            set { Store(r => r.ToDate, value); }
        }

        public Decimal Rate
        {
            get { return Retrieve(r => r.Rate); }
            set { Store(r => r.Rate, value); }
        }
    }
}