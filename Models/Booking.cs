using Orchard.ContentManagement;
using Orchard.ContentManagement.Records;
using System.Collections.Generic;
using System.Linq;

namespace Cascade.Booking.Models
{
    public class BookingRecord : ContentPartRecord
    {
        public virtual string Name { get; set; }
        public virtual string RawGuests { get; set; }
        public virtual int Year { get; set; }
        public virtual BookingState BookingState { get; set;}
    }

    public class BookingPart : ContentPart<BookingRecord>
    {
        public string Name
        {
            get { return Retrieve(r => r.Name); }
            set { Store(r => r.Name, value); }
        }
        public int Year
        {
            get { return Retrieve(r => r.Year); }
            set { Store(r => r.Year, value); }
        }
        public BookingState BookingState
        {
            get { return Retrieve(r => r.BookingState); }
            set { Store(r => r.BookingState, value); }
        }

        /// <summary>
        /// Lazyloaded guests
        /// </summary>
        private IList<Guest> _guests = null;
        public IEnumerable<Guest> Guests
        {
            get { return _guests ?? (_guests = Guest.Deserialize(Record.RawGuests)); }
            set {
                _guests = value == null ? null : value.ToList();
                Record.RawGuests = Guest.Serialize(_guests);
            }
        }

        public decimal? Total
        {
            get { return Guests.Sum(g => g.TotalCost); }
        }
        /// <summary>
        /// Lazyloaded Season Part
        /// </summary>
        //private SeasonPart _seasonPart;
        //public  SeasonPart SeasonPart
        //{
        //    get {
        //        var earliestDate = Guests.OrderBy(g => g.From).First().From;
        //        var lastDate = Guests.OrderBy(g => g.To).Last().To;

        //        return _seasonPart ?? (_seasonPart = ContentItem.ContentManager.Query<SeasonPart, SeasonRecord>()
        //            .Where(s => s.From > earliestDate && s.To < lastDate)
        //            .List()
        //            .FirstOrDefault());
        //    }
        //}
    }
}