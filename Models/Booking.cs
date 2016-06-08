using Orchard.ContentManagement;
using Orchard.ContentManagement.Records;
using Orchard.Data.Conventions;
using System.Collections.Generic;
using System.Linq;

namespace Cascade.Booking.Models
{
    public class BookingRecord : ContentPartRecord
    {
        public virtual string Name { get; set; }
        [StringLengthMax]
        public virtual string RawGuests { get; set; }
        public virtual BookingState BookingState { get; set;}
    }

    public class BookingPart : ContentPart<BookingRecord>
    {
        public string Name
        {
            get { return Retrieve(r => r.Name); }
            set { Store(r => r.Name, value); }
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
    }
}