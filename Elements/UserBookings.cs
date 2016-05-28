using Cascade.Booking.Models;
using Orchard.Layouts.Framework.Elements;
using Orchard.Layouts.Helpers;
using System.Collections.Generic;

namespace Cascade.Booking.Elements
{
    // Render the booking element form on the frontend

    public class UserBookings: Element
    {
        public override string Category
        {
            get { return "Forms"; }
        }

        public override string ToolboxIcon
        {
            // fa-user-plus
            get { return "\uf234"; }
        }

        public IEnumerable<BookingPart> Bookings
        {
            get;set;
        }

    }
}