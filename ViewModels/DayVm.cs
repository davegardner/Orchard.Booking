using Cascade.Booking.Models;
using Orchard.Core.Common.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Cascade.Booking.ViewModels
{
    public class DayVm
    {
        public int Id { get; set; }
        public bool Coupon { get; set; }
        public DateTimeEditor Date { get; set; }
        public Decimal Cost { get; set; }

    }
}