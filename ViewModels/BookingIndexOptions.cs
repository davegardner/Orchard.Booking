﻿using System.ComponentModel;

namespace Cascade.Booking.ViewModels
{
    public class BookingIndexOptions 
    {
        public BookingBulkFilter Filter { get; set; }
        public BookingBulkAction BulkAction { get; set; }
    }


    [TypeConverter(typeof(PascalCaseWordSplittingEnumConverter))]
    public enum BookingBulkFilter
    {
        All,
        Tentative,
        Firm,
        Cancelled,
        Standby
    }
    [TypeConverter(typeof(PascalCaseWordSplittingEnumConverter))]
    public enum BookingBulkAction
    {
        None,
        Delete,
        ChangeStateToTentative,
        ChangeStateToFirm,
        ChangeStateToCancelled,
        ChangeStateToStandby
    }
}
