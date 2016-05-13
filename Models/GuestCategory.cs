﻿using System.ComponentModel;

namespace Cascade.Booking.Models
{
    [TypeConverter(typeof(PascalCaseWordSplittingEnumConverter))]
    public enum GuestCategory : int
    {
        Adult,
        Child
    }
}

