using System.ComponentModel;

namespace Cascade.Booking.Models
{
    [TypeConverter(typeof(PascalCaseWordSplittingEnumConverter))]
    public enum BookingState : int
    {
        Tentative,
        Firm,
        Standby,
        Cancelled
    }
}

